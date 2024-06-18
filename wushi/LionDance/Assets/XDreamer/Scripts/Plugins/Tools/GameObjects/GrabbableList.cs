using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCameras;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.SelectionUtils;
using XCSJ.PluginXGUI.Windows.ListViews;
using static XCSJ.PluginTools.GameObjects.GrabbableModel;
using Dragger = XCSJ.PluginTools.Draggers.Dragger;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 可抓对象列表:
    /// 1、使用可抓对象模型生成列表视图
    /// 2、可从场景中拾取可抓对象，也可从当前列表中把可抓对象拖拽至场景中
    /// 3、同组对象实行先入后出的规则，也就是先放进去的可抓对象放在下部，后放入的堆叠在其上部。拿的时候先拿上部，后拿下部
    /// 4、实行克隆规则的可抓模型的原型游戏对象在程序初始化后被隐藏，其后都不能在场景中显示
    /// </summary>
    [Name("可抓对象列表")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    public class GrabbableList : ListViewModelProvider
    {
        /// <summary>
        /// 可抓对象模型列表
        /// </summary>
        [Name("可抓对象模型列表")]
        public List<SerializableGrabbableModel> _grabbableModels = new List<SerializableGrabbableModel>();

        /// <summary>
        /// 启用时加载数据
        /// </summary>
        protected override IEnumerable<ListViewItemModel> prefabModels => _grabbableModels;

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            return interactData is GrabbableListData || (base.CanInteract(interactData) && (interactData as ListViewInteractData).listViewItemModel is DraggableModel);
        }

        /// <summary>
        /// 启用时加载模型
        /// </summary>
        protected override void AddModelsOnEnable()
        {
            foreach (var item in models)
            {
                if (item is GrabbableModel grabbableModel)
                {
                    grabbableModel.grabbableList = this;
                }
            }
            base.AddModelsOnEnable();
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        public bool Contains(Grabbable grabbable)
        {
            return models.Exists(m => m is GrabbableModel gm && gm.Contains(grabbable));
        }

        /// <summary>
        /// 获取可抓对象列表
        /// </summary>
        /// <returns></returns>
        public List<Grabbable> GetGrabbables()
        {
            var list = new List<Grabbable>();
            models.ForEach(m =>
            {
                if (m is GrabbableModel gm)
                {
                    list.AddRange(gm.GetGrabbables());
                }
            });
            return list;
        }

        #region 添加和移除模型

        /// <summary>
        /// 拾取选中游戏对象
        /// </summary>
        [InteractCmd]
        [Name("拾取选择游戏对象")]
        public void PickSelectedGameObjects() => AddGameObjects(Selection.selections);

        /// <summary>
        /// 交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmdFun(nameof(PickSelectedGameObjects))]
        protected EInteractResult PickSelectedGameObjects(InteractData interactData)
        {
            return (interactData.interactable && AddGameObjects(interactData.interactable.gameObject)) ? EInteractResult.Success : EInteractResult.Fail;
        }

        /// <summary>
        /// 拾取游戏对象
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <returns></returns>
        public bool AddGameObjects(params GameObject[] gameObjects)
        {
            bool result = false;
            foreach (var go in gameObjects)
            {
                if (go && AddGrabbableListItemModelEntity(go.GetComponent<GrabbableListItemModelEntity>()))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 添加可抓列表项模型实体
        /// </summary>
        /// <param name="grabbableListItemModelEntity"></param>
        /// <returns></returns>
        public bool AddGrabbableListItemModelEntity(GrabbableListItemModelEntity grabbableListItemModelEntity)
        {
            if (grabbableListItemModelEntity)
            {
                if (!listView.IsMatchListViewNameTag(grabbableListItemModelEntity))
                {
                    Debug.LogWarningFormat("{0}与{1}标签不匹配！", grabbableListItemModelEntity.name, listView.name);
                    return false;
                }

                return AddModels(new GrabbableModel[] { (grabbableListItemModelEntity.instanceModel ?? grabbableListItemModelEntity.model) as GrabbableModel });
            }
            return false;
        }

        /// <summary>
        /// 添加模型
        /// </summary>
        /// <param name="models"></param>
        public bool AddModels(IEnumerable<GrabbableModel> models) => TryInteractModels(models, GetInCmdName(nameof(AddModel)));

        private bool TryInteractModels(IEnumerable<GrabbableModel> models, string cmdName)
        {
            var refresh = false;
            foreach (var model in models)
            {
                if (TryInteract(new GrabbableListData(model, cmdName, this, model.grabbable), out _))
                {
                    refresh = true;
                }
            }

            if (refresh) listView.Refresh();
            return refresh;
        }

        /// <summary>
        /// 添加模型：
        /// 1、优先加入到现有模型列表中（例如克隆对象或成组对象）。
        /// 2、无法加入现有模型列中时，新添加到模型列表中
        /// 3、新添加模型列表时当模型为克隆模型，使用其原型模型作为添加的对象，同时将克隆模型放回原型模型中
        /// 4、非激活添加模型的可抓游戏对象
        /// </summary>
        /// <param name="grabbableListData"></param>
        /// <returns></returns>
        [InteractCmd]
        [Name("添加模型")]
        [InteractCmdFun(nameof(AddModel))]
        public EInteractResult AddModel(GrabbableListData grabbableListData)
        {
            if (grabbableListData == null) return EInteractResult.Fail;

            var model = grabbableListData.grabbableModel;
            if (model == null || !model.valid || !model.grabbable.canInteract) return EInteractResult.Fail;

            bool add = false;
            foreach (var m in models)
            {
                if (m is GrabbableModel gm && gm.Add(model))
                {
                    add = true;
                    break;
                }
            }
            if (!add)
            {
                var realModel = model;
                // 增加克隆对象时，将其原型对象加入进去，并把克隆模型放回原型中
                if (model is CloneGrabbableModel cloneGrabbableModel)
                {
                    realModel = cloneGrabbableModel.prototypeModel;
                    realModel.Add(cloneGrabbableModel);
                }

                realModel.grabbableList = this;
                models.Add(realModel);
                listView.AddModel(realModel);
            }
            ActiveGameObject(model.gameObject, false);

            return EInteractResult.Success;
        }

        private void ActiveGameObject(GameObject go, bool active)
        {
            if (!go) return;

            if (!active)
            {
                // 取消选择
                foreach (var selectionModify in ComponentCache.GetComponents<SelectionModify>(false))
                {
                    if (selectionModify.currentSelection == go)
                    {
                        selectionModify.Unselect();
                    }
                }
            }

            // 非激活
            go.SetActive(active);
        }

        /// <summary>
        /// 移除模型
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool RemoveModels(IEnumerable<GrabbableModel> models) => TryInteractModels(models, GetInCmdName(nameof(RemoveModel)));

        /// <summary>
        /// 移除模型
        /// 1、从现有模型列表的对象上移除模型
        /// 2、当现有模型对象数量为0时，从模型列表中移除
        /// 3、激活被移除的模型对应的可抓游戏对象
        /// </summary>
        /// <param name="grabbableListData"></param>
        /// <returns></returns>
        [InteractCmd]
        [Name("移除模型")]
        [InteractCmdFun(nameof(RemoveModel))]
        public EInteractResult RemoveModel(GrabbableListData grabbableListData)
        {
            if (grabbableListData == null) return EInteractResult.Fail;

            var model = grabbableListData.grabbableModel;
            if (model == null) return EInteractResult.Fail;

            foreach (var m in models)
            {
                if (m is GrabbableModel gm && gm.Remove(model))
                {
                    if (gm.CanRemoveFromList(model))
                    {
                        if (models.Remove(gm))
                        {
                            listView.RemoveModel(gm);
                        }
                    }
                    model.grabbableList = null;
                    ActiveGameObject(model.grabbable.gameObject, true);
                    return EInteractResult.Success;
                }
            }
            return EInteractResult.Fail;
        }

        #endregion

        #region 拖拽

        private Dragger dragger = null;
        private SelectionModify selectionModify = null;
        private GrabbableModel currentDragModel = null;

        private bool TryDraggerGrab()
        {
            selectionModify = ComponentCache.GetComponent<SelectionModify>(false);
            if (!selectionModify)
            {
                Debug.LogErrorFormat("[{0}]未找到有效的选择集修改器!", CommonFun.ObjectToString(this));
                return false;
            }

            dragger = null;
            var t = selectionModify.transform;
            while (t)
            {
                dragger = t.GetComponentInChildren<Dragger>(false);
                if (dragger) break;
                t = t.parent;
            }
            if (!dragger)
            {
                Debug.LogErrorFormat("[{0}]未找到有效的主动拖拽器!", CommonFun.ObjectToString(this));
                return false;
            }
            dragger.ResetData();

            var grabbable = currentDragModel.grabbable;
            // 设置初始位置为相机后方
            var cam = CameraHelperExtension.currentCamera;
            if (cam)
            {
                grabbable.transform.position = cam.transform.position - cam.transform.forward * 5000;
            }

            // 激活游戏对象
            grabbable.gameObject.SetActive(true);

            // 设置为选中状态
            selectionModify.Select(grabbable.gameObject);

            // 启用拖拽
            dragger.TryInteract(nameof(Dragger.Grab), grabbable);

            return true;
        }

        private bool TryDraggerRelease()
        {
            var grabbable = currentDragModel.grabbable;
            if (dragger)
            {
                dragger.TryInteract(nameof(Dragger.Release), grabbable);
                selectionModify.Unselect();

                dragger = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="listViewInteractData"></param>
        internal override void OnDragStart(ListViewInteractData listViewInteractData)
        {
            base.OnDragStart(listViewInteractData);

            var grabbableModel = listViewInteractData.listViewItemModel as GrabbableModel;
            if (grabbableModel == null) return;

            if (grabbableModel.valid && models.Contains(grabbableModel))
            {
                currentDragModel = grabbableModel.Alloc();
                if (currentDragModel != null && TryDraggerGrab())
                {
                    currentDragModel.OnDragStart(listViewInteractData);
                }
            }
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="listViewInteractData"></param>
        internal override void OnDrag(ListViewInteractData listViewInteractData)
        {
            base.OnDrag(listViewInteractData);

            currentDragModel?.OnDrag(listViewInteractData);
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="listViewInteractData"></param>
        internal override void OnDragEnd(ListViewInteractData listViewInteractData)
        {
            base.OnDragEnd(listViewInteractData);

            if (currentDragModel != null && TryDraggerRelease())
            {
                // 在UI上拖拽或不运行从列表中移除时，放回列表
                if (CommonFun.IsOnUGUI() || !currentDragModel.allowRemoveFromList)
                {
                    currentDragModel.Free();
                }
                else// 对象移动到场景中
                {
                    RemoveModels(new GrabbableModel[] { currentDragModel });
                }

                currentDragModel.OnDragEnd(listViewInteractData);
                currentDragModel = null;
            }
        }

        #endregion
    }

    /// <summary>
    /// 序列化的可抓模型
    /// </summary>
    [Serializable]
    public class SerializableGrabbableModel : GrabbableModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SerializableGrabbableModel() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="grabbable"></param>
        /// <param name="allowCloneMaxCount"></param>
        public SerializableGrabbableModel(Grabbable grabbable, int allowCloneMaxCount) : base(grabbable, allowCloneMaxCount) { }
    }

    /// <summary>
    /// 可抓模型
    /// </summary>
    public class GrabbableModel : ComponentModel<Grabbable>
    {
        /// <summary>
        /// 可抓对象列表
        /// </summary>
        internal GrabbableList grabbableList { get; set; }

        #region 数据源

        /// <summary>
        /// 数据源
        /// </summary>
        public enum EDataSource
        {
            /// <summary>
            /// 禁用
            /// </summary>
            [Name("禁用")]
            Disable = -1,

            /// <summary>
            /// 使用原型
            /// </summary>
            [Name("使用原型")]
            UsePrototype = 0,

            /// <summary>
            /// 使用克隆体
            /// </summary>
            [Name("使用克隆体")]
            UseClone,
        }

        /// <summary>
        /// 数据源
        /// </summary>
        [Name("数据源")]
        [EnumPopup]
        [Readonly(EEditorMode.Runtime)]
#if UNITY_2021_3_OR_NEWER
        [DynamicLabel]
#endif
        public EDataSource _dataSource = EDataSource.UsePrototype;

        /// <summary>
        /// 允许克隆数量
        /// </summary>
        [Name("允许克隆数量")]
        [Min(1)]
        [HideInSuperInspector(nameof(_dataSource), EValidityCheckType.NotEqual, EDataSource.UseClone)]
#if UNITY_2021_3_OR_NEWER
        [DynamicLabel]
#endif
        public int _allowCloneMaxCount = 1;

        /// <summary>
        /// 启用编组:
        /// 1、克隆模式时默认就是成组。
        /// 2、非克隆模式下：为True时，【可抓对象】的标签【列表视图项组名称】相同且不为空时为同组；为False时，不编组
        /// </summary>
        [Name("启用编组")]
        [Tip("为True时，【可抓对象】的标签【列表视图项组名称】相同且不为空时为同组；为False时，不编组")]
        [Readonly(EEditorMode.Runtime)]
        [HideInSuperInspector(nameof(_dataSource), EValidityCheckType.Equal, EDataSource.UseClone)]
#if UNITY_2021_3_OR_NEWER
        [DynamicLabel]
#endif
        public bool _enableMakeGroup = true;

        #endregion

        #region 处理规则

        /// <summary>
        /// 处理规则
        /// </summary>
        public enum EHandleRule
        {
            /// <summary>
            /// 数量为0时从列表中移除
            /// </summary>
            [Name("数量为0时从列表中移除")]
            RemoveFromListViewWhenCountEqualZero,

            /// <summary>
            /// 数量为0时保持在列表中
            /// </summary>
            [Name("数量为0时保持在列表中")]
            KeepInListViewWhenCountEqualZero,
        }

        /// <summary>
        /// 处理规则
        /// </summary>
        [Name("处理规则")]
        //[Readonly(EEditorMode.Runtime)]
        [EnumPopup]
        [DynamicLabel]
        public EHandleRule _handleRule = EHandleRule.RemoveFromListViewWhenCountEqualZero;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public GrabbableModel() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="grabbable"></param>
        public GrabbableModel(Grabbable grabbable) : base(grabbable) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="grabbable"></param>
        /// <param name="allowCloneMaxCount"></param>
        public GrabbableModel(Grabbable grabbable, int allowCloneMaxCount) : base(grabbable) { _allowCloneMaxCount = allowCloneMaxCount; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="grabbable"></param>
        /// <param name="title"></param>
        /// <param name="texture2D"></param>
        public GrabbableModel(Grabbable grabbable, string title, Texture2D texture2D = null) : base(grabbable, title, texture2D) { }

        #endregion

        #region 可抓对象操作

        /// <summary>
        /// 允许从列表中移除
        /// </summary>
        public virtual bool allowRemoveFromList { get; } = true;

        /// <summary>
        /// 原型模型
        /// </summary>
        public virtual GrabbableModel prototypeModel => this;

        /// <summary>
        /// 原型可抓对象
        /// </summary>
        public virtual Grabbable prototype => unityObject;

        /// <summary>
        /// 可抓对象
        /// </summary>
        public Grabbable grabbable => unityObject as Grabbable;

        /// <summary>
        /// 可抓对象是否在场景中
        /// </summary>
        public bool grabbableInScene => grabbable && grabbable.gameObject.activeInHierarchy;

        /// <summary>
        /// 总数
        /// </summary>
        public int totalCount
        {
            get
            {
                switch (_dataSource)
                {
                    case EDataSource.UsePrototype: return (grabbableInScene ? 0 : 1) + groupMemberCount;
                    case EDataSource.UseClone: return allowCloneCount + groupMemberCount;
                    default: return 0;
                }
            }
        }

        /// <summary>
        /// 包含可抓模型
        /// </summary>
        /// <param name="grabbableModel"></param>
        /// <returns></returns>
        public bool Contains(GrabbableModel grabbableModel) => this == grabbableModel || cloneModels.Contains(grabbableModel) || groupModels.Contains(grabbableModel);

        /// <summary>
        /// 包含可抓对象
        /// </summary>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        public bool Contains(Grabbable grabbable)
        {
            return this.grabbable == grabbable || cloneModels.Exists(cm => cm.Contains(grabbable)) || groupModels.Exists(gm => gm.Contains(grabbable));
        }

        /// <summary>
        /// 返回可抓对象列表
        /// </summary>
        /// <returns></returns>
        public List<Grabbable> GetGrabbables()
        {
            var list = new List<Grabbable>();
            list.Add(grabbable);
            cloneModels.ForEach(cm => list.Add(cm.grabbable));
            groupModels.ForEach(gm => list.Add(gm.grabbable));
            return list;
        }

        #region 克隆

        private int allowCloneMaxCount => _dataSource == EDataSource.UseClone ? _allowCloneMaxCount : 0;

        /// <summary>
        /// 允许克隆的数量
        /// </summary>
        public int allowCloneCount => allowCloneMaxCount - cloneModels.Count;

        /// <summary>
        /// 已克隆的模型
        /// </summary>
        public List<GrabbableModel> cloneModels { get; private set; } = new List<GrabbableModel>();

        /// <summary>
        /// 获取克隆模型：此时允许克隆模型数量减少
        /// </summary>
        /// <returns></returns>
        private GrabbableModel AllocClone()
        {
            if (allowCloneCount > 0)
            {
                var newGO = prototype.gameObject.XCloneObject();
                if (newGO)
                {
                    newGO.XSetParent(prototype.transform.parent);
                    newGO.XSetUniqueName(prototype.name);
                    var grabbableModel = new CloneGrabbableModel(this, newGO.GetComponent<Grabbable>());
                    cloneModels.Add(grabbableModel);
                    return grabbableModel;
                }
            }
            return default;
        }

        /// <summary>
        /// 克隆模型添加到列表中：此时允许克隆模型数量增加
        /// </summary>
        /// <param name="grabbableModel"></param>
        /// <returns></returns>
        internal bool CloneModelToList(GrabbableModel grabbableModel)
        {
            if (grabbableModel is CloneGrabbableModel cloneModel && this == cloneModel.prototypeModel && this.cloneModels.Remove(cloneModel))
            {
                UnityEngine.Object.Destroy(cloneModel.gameObject);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 克隆模型添加到世界中
        /// </summary>
        /// <param name="grabbableModel"></param>
        /// <returns></returns>
        private bool CloneModelToWorld(GrabbableModel grabbableModel) => cloneModels.Contains(grabbableModel);

        #endregion

        #region 组

        /// <summary>
        /// 组成员数量:不包含自身
        /// </summary>
        public int groupMemberCount => groupModels.Count;

        /// <summary>
        /// 同组根对象
        /// </summary>
        public GrabbableModel groupRoot { get; private set; }

        /// <summary>
        /// 同组的模型
        /// </summary>
        public List<GrabbableModel> groupModels { get; private set; } = new List<GrabbableModel>();

        private GrabbableModel GetGroupModel()
        {
            if (_enableMakeGroup)
            {
                var groupCount = groupModels.Count;
                if (groupCount > 0)
                {
                    return groupModels[groupCount - 1];
                }
            }
            return default;
        }

        /// <summary>
        /// 加入组中
        /// </summary>
        /// <param name="grabbableModel"></param>
        /// <returns></returns>
        private bool AddGroup(GrabbableModel grabbableModel)
        {
            if (groupModels.Contains(grabbableModel)) return false;

            if (!_enableMakeGroup || grabbableModel == null || grabbableModel == this || !grabbableModel._enableMakeGroup) return false;
            if (!valid || !grabbableModel.valid) return false;

            var entity1 = grabbable.GetComponent<GrabbableListItemModelEntity>();
            var entity2 = grabbableModel.grabbable.GetComponent<GrabbableListItemModelEntity>();
            
            if (entity1 && entity2 && !entity1.ExistsSameTagKeyValue(grabbableList.listView._listViewItemGroupTagNameKeys, entity2)) return false;

            grabbableModel.groupRoot = this;
            groupModels.Add(grabbableModel);
            return true;
        }

        /// <summary>
        /// 从组内移除
        /// </summary>
        private bool RemoveGroup(GrabbableModel grabbableModel) => groupModels.Remove(grabbableModel);

        #endregion

        /// <summary>
        /// 分配：此时模型还在本对象内
        /// </summary>
        /// <returns></returns>
        public virtual GrabbableModel Alloc()
        {
            if (!prototype || totalCount <= 0) return default;

            // 优先分配组内模型
            var model = GetGroupModel();

            // 无组内模型时，使用原型或克隆模型
            if (model == null)
            {
                switch (_dataSource)
                {
                    case EDataSource.UsePrototype:
                        {
                            return this;
                        }
                    case EDataSource.UseClone: // 克隆
                        {
                            return AllocClone();
                        }
                }
            }
            return model;
        }

        /// <summary>
        /// 释放:没有移动到场景中，返回到列表中
        /// </summary>
        public virtual void Free()
        {
            if (grabbable)
            {
                grabbable.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 添加：将可抓模型添加到本对象中
        /// 1、可抓模型等于自身
        /// 2、可抓模型为当前模型的克隆模型
        /// 3、可抓模型与当前模型为同组模型
        /// </summary>
        /// <param name="grabbableModel"></param>
        public bool Add(GrabbableModel grabbableModel)
        {
            if (grabbableModel == null) return false;

            return grabbableModel == this || grabbableModel.unityObject == unityObject || CloneModelToList(grabbableModel) || AddGroup(grabbableModel);
        }

        /// <summary>
        /// 移除：将可抓模型从本对象中移除
        /// 1、等于自身
        /// 2、等于克隆模型
        /// 3、编组模型
        /// </summary>
        /// <param name="grabbableModel"></param>
        /// <returns></returns>
        public bool Remove(GrabbableModel grabbableModel)
        {
            if (grabbableModel == null) return false;

            var rs = grabbableModel == this || grabbableModel.unityObject == unityObject || CloneModelToWorld(grabbableModel) || RemoveGroup(grabbableModel);
            if (rs)
            {
                grabbableModel.AddGrabbableListItemModelEntity();
            }
            return rs;
        }

        /// <summary>
        /// 是否从列表中移除
        /// </summary>
        /// <param name="willRemoveModel"></param>
        /// <returns></returns>
        public bool CanRemoveFromList(GrabbableModel willRemoveModel)
        {
            return _handleRule == GrabbableModel.EHandleRule.RemoveFromListViewWhenCountEqualZero && (totalCount == 0 || (totalCount == 1 && this == willRemoveModel));
        }

        /// <summary>
        /// 添加【可抓列表项模型实体】组件
        /// </summary>
        private void AddGrabbableListItemModelEntity()
        {
            var entity = grabbable.GetComponent<GrabbableListItemModelEntity>();
            if (!entity)
            {
                entity = grabbable.XAddComponent<GrabbableListItemModelEntity>();
                if (entity)
                {
                    if (listViewItem)
                    {
                        entity.AddTagWithDistinct(ListView.ListViewNameTag, listViewItem.listView.listViewTagFirstValue);
                    }
                    entity.AddTagWithDistinct(ListView.ListViewItemGroupNameTag, title);
                }
            }

            if (entity)
            {
                entity.instanceModel = this;
            }
        }

        #endregion

        #region 模型键值

        /// <summary>
        /// 键列表
        /// </summary>
        public override IEnumerable<string> keys
        {
            get
            {
                if (_keyArray == null)
                {
                    var list = new List<string>();
                    list.AddRange(base.keys);
                    list.Add(nameof(allowCloneCount));
                    _keyArray = list.ToArray();
                }
                return _keyArray;
            }
        }

        /// <summary>
        /// 尝试获取模型键值类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool TryGetModelKeyValueType(string key, out Type type)
        {
            if (base.TryGetModelKeyValueType(key, out type)) return true;

            switch (key)
            {
                case nameof(totalCount):
                    {
                        type = typeof(int);
                        return true;
                    }
                default:
                    {
                        type = default;
                        return false;
                    }
            }
        }

        /// <summary>
        /// 尝试获取模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetModelKeyValue(string key, out object value)
        {
            if (base.TryGetModelKeyValue(key, out value)) return true;

            switch (key)
            {
                case nameof(totalCount):
                    {
                        value = totalCount;
                        return true;
                    }
                default:
                    {
                        value = default;
                        return false;
                    }
            }
        }

        #endregion
    }

    /// <summary>
    /// 克隆可抓模型
    /// </summary>
    public class CloneGrabbableModel : GrabbableModel
    {
        /// <summary>
        /// 构造函数:克隆原型不会赋值给基类的component对象
        /// </summary>
        /// <param name="grabbableModel"></param>
        /// <param name="cloneGrabbable"></param>
        public CloneGrabbableModel(GrabbableModel grabbableModel, Grabbable cloneGrabbable) : base(null, grabbableModel.title, grabbableModel.texture2D)
        {
            _prototypeModel = grabbableModel;
            _cloneGrabbable = cloneGrabbable;
        }
        private GrabbableModel _prototypeModel;
        private Grabbable _cloneGrabbable;

        /// <summary>
        /// 原型模型
        /// </summary>
        public override GrabbableModel prototypeModel => _prototypeModel;

        /// <summary>
        /// 原型可抓对象
        /// </summary>
        public override Grabbable prototype => _prototypeModel.prototype;

        /// <summary>
        /// 标题
        /// </summary>
        public override string title => prototypeModel.title;

        /// <summary>
        /// 可抓对象
        /// </summary>
        public override Grabbable unityObject => _cloneGrabbable;

        /// <summary>
        /// 列表视图项
        /// </summary>
        public override ListViewItem listViewItem { get => prototypeModel.listViewItem; internal set => prototypeModel.listViewItem = value; }

        /// <summary>
        /// 释放
        /// </summary>
        public override void Free() => prototypeModel.CloneModelToList(this);
    }

    /// <summary>
    /// 可抓列表数据
    /// </summary>
    public class GrabbableListData : InteractData<GrabbableListData>
    {
        /// <summary>
        /// 可抓模型
        /// </summary>
        public GrabbableModel grabbableModel { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public GrabbableListData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="grabbableModel"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public GrabbableListData(GrabbableModel grabbableModel, string cmdName, Interactor interactor, params InteractObject[] interactables) : base(cmdName, interactor, interactables)
        {
            this.grabbableModel = grabbableModel;
        }

        /// <summary>
        /// 复制函数
        /// </summary>
        /// <param name="interactData"></param>
        protected override void CopyTo(GrabbableListData interactData)
        {
            interactData.grabbableModel = grabbableModel;
        }
    }
}

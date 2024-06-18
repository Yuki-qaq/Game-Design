using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.Toolkit.PathWindowCore
{
    /// <summary>
    /// 路径信息管理器
    /// </summary>
    [Name("路径信息管理器")]
    [System.Serializable]
    public class PathInfoManager
    {
        /// <summary>
        /// 路径数量
        /// </summary>
        public int pathCount => pathInfos.Count;

        /// <summary>
        /// 路径信息
        /// </summary>
        public List<PathInfo> pathInfos = new List<PathInfo>();

        /// <summary>
        /// 当选择的路径信息切换时回调；参数1为新路径信息对象，参数2未旧路径信息对象；
        /// </summary>
        public static Action<PathInfo, PathInfo> onSelectedPathInfoChanged;

        /// <summary>
        /// 已选择的路径信息
        /// </summary>
        public PathInfo selectedPathInfo
        {
            get
            {
                return _selectedPathInfo;
            }
            set
            {
                var tmp = _selectedPathInfo;
                value?.OnSelected();
                if (_selectedPathInfo == value && _selectedPathInfo != null)
                {
                    _selectedPathInfo.effective = true;

                    SetSceneView();
                }
                _selectedPathInfo = value;

                if (_selectedPathInfo != null)
                {
                    var sc = _selectedPathInfo.pathObj as StateComponent;
                    if (sc)
                    {
                        selectedPathInfoString = SMSHelper.StateToString(sc.parent);
                    }
                }

                onSelectedPathInfoChanged?.Invoke(_selectedPathInfo, tmp);
            }
        }

        /// <summary>
        /// 已选择的路径信息字符串
        /// </summary>
        public string selectedPathInfoString = "";

        private PathInfo _selectedPathInfo = null;

        /// <summary>
        /// 关联状态机
        /// </summary>
        public SubStateMachine subSM
        {
            get
            {
                return _subSM;
            }
            set
            {
                if (!_subSM || _subSM != value)
                {
                    _subSM = value;

                    Clear();
                    LoadPathFromSM();

                    var state = SMSHelper.StringToState(selectedPathInfoString);
                    if (state)
                    {
                        selectedPathInfo = pathInfos.Find(p =>
                        {
                            if (p.pathObj != null)
                            {
                                var sc = p.pathObj as StateComponent;
                                if (sc && sc.parent == state)
                                {
                                    return true;
                                }
                            }
                            return false;
                        });
                    }
                    else
                    {
                        selectedPathInfo = null;
                    }
                }
            }
        }

        private SubStateMachine _subSM = null;

        /// <summary>
        /// 状态机有效
        /// </summary>
        public bool smValid => subSM && !(subSM is RootStateMachine);

        /// <summary>
        /// 状态数量
        /// </summary>
        public int stateCount = 0;

        /// <summary>
        /// 有效的路径信息数量
        /// </summary>
        public int effectivePathInfoCount => pathInfos.Count(p => p.effective);

        /// <summary>
        /// 路径类型
        /// </summary>
        public List<Type> pathTypes { get; private set; } = new List<Type>();

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            subSM = null;
            stateCount = 0;
            _selectedPathInfo = null;
            Clear();
        }

        /// <summary>
        /// 当启用
        /// </summary>
        public void OnEnable()
        {
            Reset();
            pathTypes.Clear();
            TypeHelper.FindTypeInAppWithInterface(typeof(IPath)).ForEach(t => pathTypes.Add(t));
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (subSM && stateCount != subSM.states.Count)
            {
                LoadPathFromSM();
            }
        }

        /// <summary>
        /// 导入输入
        /// </summary>
        public void ImportData()
        {
            pathInfos.ForEach(p => p.ImportData());
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        public void ExportData()
        {
            pathInfos.ForEach(p => p.ExportData());
        }

        /// <summary>
        /// 从状态机导入数据
        /// </summary>
        public void LoadPathFromSM()
        {
            if (!subSM) return;
            try
            {
                stateCount = subSM.states.Count;

                // 从状态机中获取路径动画
                List<IPath> pathMotions = new List<IPath>();
                if (PathWindowOption.weakInstance.onlyRecordMoveInCurrentStateMachine)
                {
                    subSM.states.Foreach(s =>
                    {
#if CSHARP_7_3_OR_NEWER
                        if (s.GetComponent<IPath>() is IPath p && p != null)
#else
                        var p = s.GetComponent<IPath>();
                        if (p != null)
#endif
                        {
                            pathMotions.Add(p);
                        }
                    });
                }
                else
                {
                    pathMotions = subSM.GetComponentsInChildren<IPath>().ToList();
                }

                // 删除状态机中已经不存在的路径动画
                for (int i = pathInfos.Count - 1; i >= 0; --i)
                {
                    var path = pathInfos[i].pathObj;
                    if (!pathMotions.Contains(path))
                    {
                        pathInfos.RemoveAt(i);
                    }
                }

                // 添加新的路径动画
                foreach (var path in pathMotions)
                {
                    Add(path);
                }
            }
            catch (Exception e)
            {
                Debug.Log(nameof(PathInfoManager) + ":" + e.ToString());
            }
        }

        /// <summary>
        /// 床架多状态
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<PathInfo> CreateMultipleState(IEnumerable<GameObject> gameObjects, Type type)
        {
            List<PathInfo> pathInfos = new List<PathInfo>();
            gameObjects.Foreach(go =>
            {
                if (Create(new GameObject[] { go }, type) is PathInfo pathInfo)
                {
                    pathInfos.Add(pathInfo);
                }
            });
            return pathInfos;
        }

        /// <summary>
        /// 创建单状态
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public PathInfo CreateSingleState(IEnumerable<GameObject> gameObjects, Type type, string name = "")
        {
            return Create(gameObjects, type, name);
        }

        /// <summary>
        /// 当记录修改
        /// </summary>
        /// <param name="recording"></param>
        public void OnRecordChange(bool recording)
        {
            // 启动录制
            if (recording)
            {
                pathInfos.ForEach(data => data.OnBeginRecord());
            }
            else // 停止录制
            {
                pathInfos.ForEach(data => data.OnEndRecord());
            }
        }

        /// <summary>
        /// 记录选择的路径
        /// </summary>
        public void RecordSelectedPath()
        {
            selectedPathInfo?.Record();
        }

        /// <summary>
        /// 记录
        /// </summary>
        public void Record()
        {
            pathInfos.ForEach(data => data.Record());
        }

        private void SetSceneView()
        {
            // 场景视图聚焦
            if (SceneView.lastActiveSceneView)
            {
                if (_selectedPathInfo.firstTransform)
                {
                    SceneView.lastActiveSceneView.FrameSelected();
                }
                else
                {
                    SceneView.lastActiveSceneView.Frame(new Bounds(_selectedPathInfo.virtualPointPosition, Vector3.one), false);
                }
            }
        }

        /// <summary>
        /// 设置默认选择的路径信息
        /// </summary>
        public void SetDefaultSelectedPathInfo()
        {
            selectedPathInfo = pathInfos.FirstOrDefault();
        }

        /// <summary>
        /// 已选择的路径信息
        /// </summary>
        /// <param name="gameObject"></param>
        public void SelectedPathInfo(GameObject gameObject)
        {
            if (gameObject)
            {
                selectedPathInfo = Find(gameObject);
            }
        }

        #region 数据创建，添加，删除和查询操作

        /// <summary>
        /// 默认状态名
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="pathType"></param>
        /// <returns></returns>
        public static string DefaultStateName(GameObject gameObject, Type pathType)
        {
            return gameObject.name + "_" + CommonFun.Name(pathType);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="pathType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public PathInfo Create(IEnumerable<GameObject> gameObjects, Type pathType, string name = "")
        {
            if (gameObjects == null || gameObjects.Count() == 0 || !subSM) return null;

            var state = subSM.CreateNormalState(string.IsNullOrEmpty(name) ? DefaultStateName(gameObjects.First(), pathType) : name, pathType);
            var path = state.GetComponent<IPath>();
            if (path != null)
            {
                gameObjects.Foreach(go =>
                {
                    if (go)
                    {
                        path.AddTransform(go.transform);
                    }
                });
            }

            return Add(state.GetComponent<IPath>());
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="pathObj"></param>
        /// <returns></returns>
        public PathInfo Add(IPath pathObj)
        {
            if (pathObj == null || Contain(pathObj)) return null;

            var mi = new PathInfo(pathObj);
            pathInfos.Add(mi);
            return mi;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Remove(PathInfo data)
        {
            if (selectedPathInfo == data)
            {
                selectedPathInfo = null;
            }
            data.Delete();
            return pathInfos.Remove(data);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            selectedPathInfo = null;
            pathInfos.ForEach(p => p.Delete());
            Clear();
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            pathInfos.Clear();
        }

        /// <summary>
        /// 清理路径数据
        /// </summary>
        public void ClearPathData()
        {
            pathInfos.ForEach(d => d.Clear());
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="pathObj"></param>
        /// <returns></returns>
        public bool Contain(IPath pathObj)
        {
            return pathInfos.Exists(data => data.pathObj == pathObj);
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="pathObj"></param>
        /// <returns></returns>
        public PathInfo Find(IPath pathObj)
        {
            return pathInfos.Find(data => data.pathObj == pathObj);
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public PathInfo Find(GameObject go)
        {
            if (!go) return null;

            return pathInfos.Find(data => data.pathObj.transforms.Contains(go.transform));
        }

        /// <summary>
        /// 设置生效
        /// </summary>
        /// <param name="effective"></param>
        public void SetEffective(bool effective)
        {
            pathInfos.ForEach(m => m.effective = effective);
        }

        /// <summary>
        /// 设置显示路径
        /// </summary>
        /// <param name="display"></param>
        public void SetDisplayPath(bool display)
        {
            pathInfos.ForEach(m => m.displayPath = display);
        }

        /// <summary>
        /// 存在路径的的变换出
        /// </summary>
        /// <returns></returns>
        public bool ExistsTransformOutOfPath()
        {
            return pathInfos.Exists(pi => pi.IsTransformOutOfPathWhenRecording());
        }

        #endregion

        #region 路径基础调整算法

       /// <summary>
       /// 逆转动作
       /// </summary>
        public void ReverseMotion() => pathInfos.ForEach(p => p.ReverseMotion());

        /// <summary>
        /// 反转路径 起点不变，路径方向和原来呈现对称负方向
        /// </summary>
        public void OppositeDirectionPath() => pathInfos.ForEach(pathData => pathData.OppositeDirectionPath());

        /// <summary>
        /// 移动到开始
        /// </summary>
        public void MoveToBegin() => pathInfos.ForEach(data => data.MoveToBegin());

        /// <summary>
        /// 移动到结束
        /// </summary>
        public void MoveToEnd() => pathInfos.ForEach(data => data.MoveToEnd());

        #endregion
    }
}

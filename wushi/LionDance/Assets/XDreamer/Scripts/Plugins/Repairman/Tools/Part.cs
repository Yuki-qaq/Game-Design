using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMMO;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginRepairman.Tools
{
    /// <summary>
    /// 零件
    /// 1、具有可替换零件名称标签，依赖可抓对象组件，天然具有抓放功能
    /// 2、在设备组织层级中处于叶子层级上，其下不能再存在零件
    /// 3、当零件与插槽位置超过吸附距离时为拆卸态，或者零件所在游戏对象非激活时，也为拆卸态
    /// 4、被拆卸下来的零件父级默认设置为拆装管理器。保证不会随着设备或模块移动
    /// 5、零件启用时，父级如果没有模块，则为拆卸态
    /// 6、零件有拆卸约束或装配约束时，零件不能交互
    /// 7、零件有健康度属性表达其是否可用
    /// </summary>
    [Name("零件")]
    [XCSJ.Attributes.Icon(nameof(Part))]
    [RequireManager(typeof(RepairmanManager))]
    [Owner(typeof(RepairmanManager))]
    [Tool(RepairmanCategory.Model, nameof(InteractableVirtual), nameof(Grabbable), rootType = typeof(RepairmanManager))]
    public class Part : GrabbableHost, IPropertyKeyProvider
    {
        /// <summary>
        /// 替换零件标签
        /// </summary>
        [PropertyKey]
        public const string ReplacePartTagKey = "可替换零件标签";

        /// <summary>
        /// 可替换零件标签关键字列表
        /// </summary>
        [Name("可替换零件标签关键字列表")]
        public List<string> _replacePartTagKeys = new List<string>();

        /// <summary>
        /// 属性关键字信息
        /// </summary>
        public List<PropertyKeyInfo> propertyKeyInfos
        {
            get
            {
                var className = CommonFun.Name(typeof(Part));
                var propertyKeyName = CommonFun.Name(typeof(Part), nameof(Part._replacePartTagKeys));

                var list = new List<PropertyKeyInfo>();
                foreach (var item in _replacePartTagKeys)
                {
                    list.Add(new PropertyKeyInfo(className, propertyKeyName, item));
                }
                return list;
            }
        }

        /// <summary>
        /// 第一个可替换零件标签值
        /// </summary>
        public string firstReplacePartTagValue
        {
            get
            {
                if (string.IsNullOrEmpty(_replacePartTagValue))
                {
                    _replacePartTagValue = _tagProperty.GetFirstValue(_replacePartTagKeys);
                }
                return _replacePartTagValue;
            }
        }
        private string _replacePartTagValue;

        /// <summary>
        /// 匹配可替换零件标签
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool IsMatchRepacePartTag(Part part) => this.ExistsSameTagKeyValue(_replacePartTagKeys, part);

        /// <summary>
        /// 所属模块
        /// </summary>
        public Module parentModule => transform.parent ? transform.parent.GetComponentInParent<Module>() : default;

        /// <summary>
        /// 所属设备
        /// </summary>
        public virtual Device parentDevice => transform.parent ? transform.parent.GetComponentInParent<Device>() : default;

        /// <summary>
        /// 零件装配节点：当零件被装配到模块中时，该节点对象有效，否则为空
        /// </summary>
        public PartAssemblyNode partAssemblyNode { get; set; }

        /// <summary>
        /// 健康值:表达其是否可用，可用于修理时替换破损零件
        /// </summary>
        [Name("健康值")]
        [Tip("表达其是否可用，可用于修理时替换破损零件", "Express whether it is usable and can be used to replace damaged parts during repair")]
        [Range(0, 1)]
        public float _health = 1;

        /// <summary>
        /// 健康值:表达其是否可用，可用于修理时替换破损零件
        /// </summary>
        public float health => _health;

        /// <summary>
        /// 健康值百分比
        /// </summary>
        public float healthPercent => _health*100;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _replacePartTagKeys.Add(ReplacePartTagKey);
            this.AddTagWithDistinct(ReplacePartTagKey, name);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!parentModule)
            {
                assembleState = EAssembleState.Disassembled;
            }
            _replacePartTagValue = null;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        public override bool canInteract
        {
            get 
            {
                switch (assembleState)
                {
                    case EAssembleState.Assembled:
                        {
                            return canDisassembly;
                        }
                }
                return base.canInteract; 
            }
        }

        /// <summary>
        /// 能否抓:有父级，并且当前零件被装配约束，则使用父级对象进行拖拽
        /// </summary>
        /// <param name="dragger"></param>
        /// <returns></returns>
        protected virtual bool CanGrabbed(Dragger dragger)
        {
            return canDisassembly;
        }

        /// <summary>
        /// 零件状态
        /// </summary>
        [Name("零件状态")]
        [EnumPopup]
        [Tip("启用时，零件的父级如果没有模块，则设定为拆卸态", "When enabled, if the parent of the part does not have a module, it is set to the disassembled state")]
        public EAssembleState _assembleState = EAssembleState.None;

        /// <summary>
        /// 零件状态
        /// </summary>
        public virtual EAssembleState assembleState
        {
            get => _assembleState;
            internal set
            {
                if (_assembleState != value)
                {
                    _assembleState = value;
                    switch (_assembleState)
                    {
                        case EAssembleState.None:None(); break;
                        case EAssembleState.Assembled: Assembled(); break;
                        case EAssembleState.Disassembled: Disassembled(); break;
                        case EAssembleState.AssemblyInProgress: AssemblyInProgress(); break;
                        case EAssembleState.DisassemblyInProgress: DisassemblyInProgress(); break;
                    }
                }
            }
        }

        /// <summary>
        /// 能否装配
        /// </summary>
        public bool canAssembly => assembleState == EAssembleState.Disassembled;

        /// <summary>
        /// 能否拆卸
        /// </summary>
        public bool canDisassembly => partAssemblyNode == null || partAssemblyNode.canDisassembly;

        /// <summary>
        /// 无状态
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("无状态")]
        public void None() => CallFinished(nameof(None));

        /// <summary>
        /// 装配态
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("装配态")]
        public void Assembled() => CallFinished(nameof(Assembled));

        /// <summary>
        /// 拆卸态
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("拆卸态")]
        public void Disassembled() => CallFinished(nameof(Disassembled));

        /// <summary>
        /// 装配中
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("装配中")]
        public void AssemblyInProgress() => CallFinished(nameof(AssemblyInProgress));

        /// <summary>
        /// 拆卸中
        /// </summary>
        [InteractCmd(ECmdType.Out)]
        [Name("拆卸中")]
        public void DisassemblyInProgress() => CallFinished(nameof(DisassemblyInProgress));

        #region MMO对象

        /// <summary>
        /// 上一次数据:零件状态
        /// </summary>
        [Readonly]
        [Name("上一次数据")]
        [Tip("零件状态", "Part State")]
        [EnumPopup]
        [Group("MMO同步数据", textEN ="MMO Syn Data")]
        public EAssembleState _lastData = EAssembleState.None;

        /// <summary>
        /// 原始数据:零件状态
        /// </summary>
        [Readonly]
        [Name("原始数据")]
        [Tip("零件状态", "Part State")]
        [EnumPopup]
        public EAssembleState _originalData = EAssembleState.None;

        /// <summary>
        /// 当序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool OnSerialize(out string data)
        {
            if (_lastData!= assembleState || _dirty)
            {
                _dirty = false;
                _lastData = assembleState;
                data = _lastData.ToString();
                return true;
            }
            data = default;
            return false;
        }

        /// <summary>
        /// 当反序列化
        /// </summary>
        /// <param name="data"></param>
        public override void OnDeserialize(Data data)
        {
            version = data.version;

            assembleState = _lastData = (EAssembleState)EnumValueCache.Get(typeof(EAssembleState), data.data, EEnumStringType.Default);
        }

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            version = 0;
            _originalData = _lastData = assembleState;
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            version = 0;
            assembleState = _originalData;
        }

        #endregion
    }

    /// <summary>
    /// 装配状态
    /// </summary>
    public enum EAssembleState
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 装配态
        /// </summary>
        [Name("装配态")]
        Assembled,

        /// <summary>
        /// 拆卸态
        /// </summary>
        [Name("拆卸态")]
        Disassembled,

        /// <summary>
        /// 装配中
        /// </summary>
        [Name("装配中")]
        AssemblyInProgress,

        /// <summary>
        /// 拆卸中
        /// </summary>
        [Name("拆卸中")]
        DisassemblyInProgress
    }
}

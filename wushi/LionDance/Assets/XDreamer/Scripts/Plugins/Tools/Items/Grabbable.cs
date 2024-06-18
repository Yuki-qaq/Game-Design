using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Base.Recorders;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Draggers.TRSHandles;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 可抓对象:
    /// 1、可抓对象在三维空间中被抓取器改变位置、角度或形状称为“受力”
    /// 2、抓取器有多个时，采用“合力”方式作用于可抓对象。（例如：两个手搬起石头）
    /// 3、可抓对象可将作用“力”传递给其主体受力对象。（例如：拉门把手，把手将力传递给门，门转动。）
    /// </summary>
    [Name("可抓对象")]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    [Tool("常用", rootType = typeof(ToolsManager))]
    [Tool(ToolsCategory.InteractCommon, nameof(InteractableVirtual), rootType = typeof(ToolsManager), index = 1)]
    public class Grabbable : InteractableVirtual, IUsageHost
    {
        /// <summary>
        /// 抓用途关键字
        /// </summary>
        public const string GrabbedUsageKey = nameof(Dragger.Grab);

        /// <summary>
        /// 插槽标签
        /// </summary>
        public string[] socketTags => _tagProperty.GetTagValues(SingleSocket.SocketTag);

        /// <summary>
        /// 能否交互
        /// </summary>
        public bool canInteract => grabbableHosts.Count == 0 || grabbableHosts.All(h => h.canInteract);
        internal List<GrabbableHost> grabbableHosts { get; private set; } = new List<GrabbableHost>();

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => canInteract;

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteractAsInteractable(InteractData interactData)
        {
            return canInteract && base.CanInteractAsInteractable(interactData);
        }

        #region 抓设置

        /// <summary>
        /// 移动轴
        /// </summary>
        [Group("抓设置", textEN = "Grab Settings", defaultIsExpanded = false)]
        [Name("移动轴")]
        public EAxis _moveAxis = EAxis.X | EAxis.Y | EAxis.Z;

        /// <summary>
        /// 旋转轴
        /// </summary>
        [Name("旋转轴")]
        public EAxis _rotateAxis = EAxis.X | EAxis.Y | EAxis.Z;

        /// <summary>
        /// 缩放轴
        /// </summary>
        [Name("缩放轴")]
        public EAxis _scaleAxis = EAxis.X | EAxis.Y | EAxis.Z;

        /// <summary>
        /// 被抓目标对象规则
        /// </summary>
        [Name("被抓目标对象规则")]
        public enum EGrabbabledTargetObjectRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 自动
            /// </summary>
            [Name("自动")]
            [Tip("可抓对象上存在刚体，则使用刚体，否则使用变换", "If there is a rigid body on the grabable object, use the rigid body; otherwise, use the transformation")]
            Auto,

            /// <summary>
            /// 刚体
            /// </summary>
            [Name("刚体")]
            Rigidbody,

            /// <summary>
            /// 变换
            /// </summary>
            [Name("变换")]
            Transform,
        }

        /// <summary>
        /// 被抓目标对象规则
        /// </summary>
        [Name("被抓目标对象规则")]
        [EnumPopup]
        public EGrabbabledTargetObjectRule _grabbabledTargetObjectRule = EGrabbabledTargetObjectRule.Auto;

        /// <summary>
        /// 受控变换替换对象
        /// </summary>
        [Name("受控变换替换对象")]
        [HideInSuperInspector(nameof(_grabbabledTargetObjectRule), EValidityCheckType.Equal, EGrabbabledTargetObjectRule.Rigidbody)]
        public Transform _transformOverride = null;

        /// <summary>
        /// 刚体替换对象
        /// </summary>
        [Name("受控刚体替换对象")]
        [HideInSuperInspector(nameof(_grabbabledTargetObjectRule), EValidityCheckType.Equal, EGrabbabledTargetObjectRule.Transform)]
        public Rigidbody _rigidbodyOverride = null;

        /// <summary>
        /// 被抓参考点
        /// </summary>
        [Name("被抓参考点")]
        public Transform _grabPoint;

        /// <summary>
        /// 拖拽器最小与最大数量
        /// </summary>
        [Name("拖拽器最小与最大数量")]
        [Tip("当拖拽器数量未达到最小值时，可抓对象无法受控移动；当拖拽器数量已经达到最大值，则无法接受新的拖拽器对其进行控制；", "When the number of drags does not reach the minimum value, the grabable object cannot be controlled to move; When the number of drags has reached the maximum, it is not acceptable to accept new drags for control;")]
        [LimitRange(1, 100)]
        public Vector2Int _draggerMinMaxCount = new Vector2Int(1, 2);

        /// <summary>
        /// 自动添加碰撞体到子级
        /// </summary>
        [Name("自动添加碰撞体到子级")]
        public bool _autoAddColliderToChildren = true;

        /// <summary>
        /// 最小拖拽器数量：当作用在本对象上的拖拽器大于等于本值时，才可拖拽对象
        /// </summary>
        public int minCountOfDragger => _draggerMinMaxCount.x;

        /// <summary>
        /// 最大拖拽器数量：当作用在本对象上的拖拽器等于本值时，不可再接受新拖拽器控制
        /// </summary>
        public int maxCountOfDragger => _draggerMinMaxCount.y;

        /// <summary>
        /// 抓取点
        /// </summary>
        public Transform grabPoint => _grabPoint ? _grabPoint : transform;

        /// <summary>
        /// 抓点与可抓对象位置偏移量
        /// </summary>
        public Vector3 grabPointPositionOffset => _grabPoint ? _grabPoint.position - targetTransform.position : Vector3.zero;

        /// <summary>
        /// 抓点与可抓对象角度偏移量
        /// </summary>
        public Quaternion grabPointRotationOffset => _grabPoint ? _grabPoint.localRotation : Quaternion.identity;

        /// <summary>
        /// 刚体记录器
        /// </summary>
        private RigidbodyRecorder rigidbodyRecorder = new RigidbodyRecorder();

        /// <summary>
        /// 刚体
        /// </summary>
        public Rigidbody ownRigidbody
        {
            get
            {
                if (!_rigidbody)
                {
                    _rigidbody = GetComponentInParent<Rigidbody>();
                }
                return _rigidbody;
            }
        }
        private Rigidbody _rigidbody;

        #endregion

        #region 连接体及变换信息

        /// <summary>
        /// 与其他对象关联
        /// </summary>
        public virtual bool isConnectToOther { get => _isConnectToOther; set => _isConnectToOther = value; }
        private bool _isConnectToOther;

        /// <summary>
        /// 受控目标对象
        /// </summary>
        public virtual Transform targetTransform => _transformOverride ? _transformOverride : transform;

        /// <summary>
        /// 受控目标刚体
        /// </summary>
        public virtual Rigidbody targetRigidbody => _rigidbodyOverride ? _rigidbodyOverride : ownRigidbody;

        /// <summary>
        /// 位置
        /// </summary>
        public virtual Vector3 position
        {
            get => targetTransform.position;
            set
            {
                value += grabPointPositionOffset;

                Vector3 pos = position;
                if ((_moveAxis & EAxis.X) == EAxis.X)
                {
                    pos.x = value.x;
                }

                if ((_moveAxis & EAxis.Y) == EAxis.Y)
                {
                    pos.y = value.y;
                }

                if ((_moveAxis & EAxis.Z) == EAxis.Z)
                {
                    pos.z = value.z;
                }

                SetPositionByRule(_grabbabledTargetObjectRule, pos);
            }
        }

        private void SetPositionByRule(EGrabbabledTargetObjectRule grabbabledTargetObjectRule, Vector3 pos)
        {
            switch (grabbabledTargetObjectRule)
            {
                case EGrabbabledTargetObjectRule.Auto:
                    {
                        if (targetRigidbody)
                        {
                            SetPositionByRule(EGrabbabledTargetObjectRule.Rigidbody, pos);
                        }
                        else
                        {
                            SetPositionByRule(EGrabbabledTargetObjectRule.Transform, pos);
                        }
                        break;
                    }
                case EGrabbabledTargetObjectRule.Rigidbody:
                    {
                        // 使用当前方法有插值效果（直接设置targetRigidbody.position无插值效果）
                        if(targetRigidbody) targetRigidbody.MovePosition(pos);
                        break;
                    }
                case EGrabbabledTargetObjectRule.Transform:
                    {
                        targetTransform.position = pos;
                        break;
                    }
            }
        }

        /// <summary>
        /// 本地坐标
        /// </summary>
        public Vector3 localPosition
        {
            get => targetTransform.localPosition;
            set => targetTransform.localPosition = value;
        }

        /// <summary>
        /// 旋转量
        /// </summary>
        public virtual Quaternion rotation
        {
            get => targetTransform.rotation;
            set
            {
                value = value * grabPointRotationOffset;
                var eularAngle = targetTransform.rotation.eulerAngles;
                var newEularAngle = value.eulerAngles;
                if ((_rotateAxis & EAxis.X) == EAxis.X)
                {
                    eularAngle.x = newEularAngle.x;
                }

                if ((_rotateAxis & EAxis.Y) == EAxis.Y)
                {
                    eularAngle.y = newEularAngle.y;
                }

                if ((_rotateAxis & EAxis.Z) == EAxis.Z)
                {
                    eularAngle.z = newEularAngle.z;
                }

                SetRotationByRule(_grabbabledTargetObjectRule, Quaternion.Euler(eularAngle));
            }
        }

        private void SetRotationByRule(EGrabbabledTargetObjectRule grabbabledTargetObjectRule, Quaternion newRotation)
        {
            switch (grabbabledTargetObjectRule)
            {
                case EGrabbabledTargetObjectRule.Auto:
                    {
                        if (targetRigidbody)
                        {
                            SetRotationByRule(EGrabbabledTargetObjectRule.Rigidbody, newRotation);
                        }
                        else
                        {
                            SetRotationByRule(EGrabbabledTargetObjectRule.Transform, newRotation);
                        }
                        break;
                    }
                case EGrabbabledTargetObjectRule.Rigidbody:
                    {
                        // 使用当前方法有插值效果（直接设置targetRigidbody.rotation无插值效果）
                        if (targetRigidbody) targetRigidbody.MoveRotation(newRotation);
                        break;
                    }
                case EGrabbabledTargetObjectRule.Transform:
                    {
                        targetTransform.rotation = newRotation;
                        break;
                    }
            }
        }

        /// <summary>
        /// 本地坐标
        /// </summary>
        public Quaternion localRotation
        {
            get => targetTransform.localRotation;
            set => targetTransform.localRotation = value;
        }

        /// <summary>
        /// 本地缩放
        /// </summary>
        public Vector3 localScale
        {
            get => targetTransform.localScale;
            set
            {
                var scale = targetTransform.localScale;
                if ((_scaleAxis & EAxis.X) == EAxis.X)
                {
                    scale.x = value.x;
                }

                if ((_scaleAxis & EAxis.Y) == EAxis.Y)
                {
                    scale.y = value.y;
                }

                if ((_scaleAxis & EAxis.Z) == EAxis.Z)
                {
                    scale.z = value.z;
                }
                targetTransform.localScale = scale;
            }
        }

        /// <summary>
        /// 设置姿态
        /// </summary>
        /// <param name="postion"></param>
        /// <param name="rotation"></param>
        /// <param name="sendTransformChanged"></param>
        public void SetPose(Vector3 postion, Quaternion rotation, bool sendTransformChanged = true)
        {
            this.rotation = rotation;
            SetPosition(postion, sendTransformChanged);
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="postion"></param>
        /// <param name="sendTransformChanged"></param>
        public void SetPosition(Vector3 postion, bool sendTransformChanged = true)
        {
            this.position = postion;
            if (sendTransformChanged) SendTransformChanged(); 
        }

        /// <summary>
        /// 设置旋转
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="sendTransformChanged"></param>
        public void SetRotation(Quaternion rotation, bool sendTransformChanged = true)
        {
            this.rotation = rotation;
            if (sendTransformChanged) SendTransformChanged();
        }

        /// <summary>
        /// 设置本地尺寸
        /// </summary>
        /// <param name="size"></param>
        /// <param name="sendTransformChanged"></param>
        public void SetLocalSize(Vector3 size, bool sendTransformChanged = true)
        {
            localScale = size;
            if (sendTransformChanged) SendTransformChanged();
        }

        private void SendTransformChanged() => CallFinished(GetOutCmdName(nameof(Hold)));

        #endregion

        #region Unity 消息

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _rigidbodyOverride = ownRigidbody;
            if (_rigidbodyOverride)// 默认初始化刚体属性为连续检测
            {
                _rigidbodyOverride.interpolation = RigidbodyInterpolation.Interpolate;
            }

            _transformOverride = _rigidbodyOverride ? _rigidbodyOverride.transform : transform;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            // 对象受铰链控制
            if (!_isConnectToOther && GetComponentInParent<Joint>())
            {
                _isConnectToOther = true;
            }

            if (_autoAddColliderToChildren) AddMeshCollider();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            draggerInteractDatas.Clear();
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected virtual void FixedUpdate()
        {
            // 可更新抓对象变换数据
            if (_doMotion)
            {
                _doMotion = false;

                OnHold(draggerInteractDatas);

                draggerInteractDatas.Clear();
            }
        }

        /// <summary>
        /// 碰撞进入
        /// </summary>
        /// <param name="collision"></param>
        protected void OnCollisionEnter(Collision collision)
        {
            //Debug.Log(name + "=> :" + collision.collider.name);

            AddCollider(collision.collider);
        }

        /// <summary>
        /// 碰撞退出
        /// </summary>
        /// <param name="collision"></param>
        protected void OnCollisionExit(Collision collision)
        {
            //Debug.Log(name + "<= :" + collision.collider.name);
            RemoveCollider(collision.collider);
        }

        /// <summary>
        /// 触发进入
        /// </summary>
        /// <param name="other"></param>
        protected void OnTriggerEnter(Collider other)
        {
            //Debug.Log(other.name + "触发进入 :" + name);
            AddCollider(other);
        }

        /// <summary>
        /// 触发退出
        /// </summary>
        /// <param name="other"></param>
        protected void OnTriggerExit(Collider other)
        {
            //Debug.Log(other.name + "触发离开 :" + name);
            RemoveCollider(other);
        }

        #endregion

        #region 拖拽

        private void AddMeshCollider()
        {
            // 自动添加MeshCollider
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                if (!renderer.GetComponent<Collider>())
                {
                    renderer.XAddComponent<MeshCollider>();
                }
            }
        }

        private Dictionary<Dragger, GrabInteractData> draggerInteractDatas = new Dictionary<Dragger, GrabInteractData>();

        /// <summary>
        /// 做运动
        /// </summary>
        [Name("做运动")]
        [Readonly]
        public bool _doMotion = false;

        /// <summary>
        /// 能否被抓
        /// </summary>
        /// <param name="dragger"></param>
        /// <returns></returns>
        protected virtual bool CanGrabbed(Dragger dragger) => !IsUsageContain(dragger) && usage.GetCount(GrabbedUsageKey) < maxCountOfDragger;

        /// <summary>
        /// 能否保持
        /// </summary>
        /// <param name="dragger"></param>
        /// <returns></returns>
        protected virtual bool CanHold(Dragger dragger) => IsUsageContain(dragger) && usage.GetCount(GrabbedUsageKey) >= minCountOfDragger;

        /// <summary>
        /// 能否释放
        /// </summary>
        /// <param name="dragger"></param>
        /// <returns></returns>
        protected virtual bool CanRelease(Dragger dragger) => IsUsageContain(dragger);

        private bool IsUsageContain(Dragger dragger) => usage.Contains(GrabbedUsageKey, dragger);

        /// <summary>
        /// 抓
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmd]
        [Name("抓")]
        [InteractCmdFun(nameof(Grab))]
        public EInteractResult Grab(InteractData interactData)
        {
            if (interactData.interactor is Dragger dragger && dragger)
            {
                if (CanGrabbed(dragger) && AddGrabUsage(dragger))
                {
                    CallFinished(new HoldedInteractData(GetOutCmdName(nameof(Grab)), this, draggerInteractDatas));
                    return EInteractResult.Success;
                }
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 保持
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmd]
        [Name("保持")]
        [InteractCmdFun(nameof(Hold))]
        public EInteractResult Hold(InteractData interactData)
        {
            if (interactData.interactor is Dragger dragger && dragger)
            {
                if (CanHold(dragger) && interactData is GrabInteractData data)
                {
                    draggerInteractDatas[dragger] = data;

                    // 获取了所有拖拽器保持数据后，才更新被抓对象
                    if (draggerInteractDatas.Count >= grabberCount)
                    {
                        _doMotion = true;
                    }
                }
                return EInteractResult.Success;
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 放
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        [InteractCmd]
        [Name("放")]
        [InteractCmdFun(nameof(Release))]
        public EInteractResult Release(InteractData interactData)
        {
            if (interactData.interactor is Dragger dragger && dragger)
            {
                if (CanRelease(dragger) && RemoveGrabUsage(dragger))
                {
                    _doMotion = false;
                    draggerInteractDatas.Clear();
                    CallFinished(new HoldedInteractData(GetOutCmdName(nameof(Release)), this, draggerInteractDatas));

                    //CallFinished(CloneInteractData(interactData));

                    return EInteractResult.Success;
                }
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 保持回调
        /// </summary>
        /// <param name="interactDatas"></param>
        protected virtual void OnHold(Dictionary<Dragger, GrabInteractData> interactDatas)
        {
            var totalPosition = Vector3.zero;
            var totalRotation = Quaternion.identity;
            var totalLocalScale = Vector3.zero;

            int positionDataCount = 0;
            int rotationDataCount = 0;
            int scaleDataCount = 0;
            var hitDatas = new List<(Vector3, Vector3)>();
            int hitDataCount = 0;

            var applyRigidbody = targetRigidbody;
            foreach (var data in draggerInteractDatas.Values)
            {
                if (data.position.HasValue)
                {
                    ++positionDataCount;
                    totalPosition += data.position.Value;
                }

                if (data.rotation.HasValue)
                {
                    ++rotationDataCount;
                    totalRotation *= data.rotation.Value;
                }

                if (data.scale.HasValue)
                {
                    ++scaleDataCount;
                    totalLocalScale += data.scale.Value;
                }

                if (applyRigidbody && data.hitOffset.HasValue && data.hitPosition.HasValue)
                {
                    ++hitDataCount;
                    hitDatas.Add((data.hitOffset.Value * data.hitOffsetScale.Value, data.hitPosition.Value));
                }
            }

            // 优先设置撞击数据
            if (hitDataCount > 0)
            {
                if (isConnectToOther)// 施力
                {
                    foreach (var data in hitDatas)
                    {
                        applyRigidbody.AddForceAtPosition(data.Item1, data.Item2, ForceMode.Force);
                    }
                }
                else // 设置速度
                {
                    var totalVelocity = Vector3.zero;
                    hitDatas.ForEach(data => totalVelocity += data.Item1);
                    applyRigidbody.velocity = totalVelocity;
                }
            }
            else
            {
                // 设置位置
                if (positionDataCount > 0 && _moveAxis != 0)
                {
                    position = totalPosition / positionDataCount;
                }

                // 设置旋转量
                if (rotationDataCount > 0 && _rotateAxis != 0)
                {
                    rotation = totalRotation;
                }

                // 设置缩放量
                if (scaleDataCount > 0 && _scaleAxis != 0)
                {
                    localScale = totalLocalScale / scaleDataCount;
                }

            }

            if (hitDataCount > 0 || positionDataCount > 0 || rotationDataCount > 0 || scaleDataCount > 0)
            {
                CallFinished(new HoldedInteractData(GetOutCmdName(nameof(Hold)), this, draggerInteractDatas));

                SendTransformChanged();
            }
        }

        /// <summary>
        /// 抓取器数量
        /// </summary>
        public int grabberCount => usage.GetCount(GrabbedUsageKey);

        /// <summary>
        /// 是否被拖拽器拖拽
        /// </summary>
        /// <param name="grabber"></param>
        /// <returns></returns>
        public bool IsGrabbedBy(ExtensionalInteractObject grabber) => usage.Contains(GrabbedUsageKey, grabber);

        /// <summary>
        /// 是否仅仅被拖拽器拖拽
        /// </summary>
        /// <param name="grabber"></param>
        /// <returns></returns>
        public bool IsOnlyGrabbedBy(ExtensionalInteractObject grabber) => usage.Contains(GrabbedUsageKey, u => u.users.Count == 1 && u.users[0] == grabber);

        /// <summary>
        /// 添加抓用途: 使用固定抓关键字
        /// </summary>
        /// <param name="grabber"></param>
        /// <returns></returns>
        public bool AddGrabUsage(ExtensionalInteractObject grabber)
        {
            if (grabber && usage.Add(GrabbedUsageKey, grabber))
            {
                if (grabberCount == 1 && targetRigidbody)
                {
                    rigidbodyRecorder.Record(targetRigidbody);
                    targetRigidbody.useGravity = false; // 被抓对象的刚体停止使用重力
                    //targetRigidbody.isKinematic = true;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///  移除抓用途: 使用固定抓关键字
        /// </summary>
        /// <param name="grabber"></param>
        public bool RemoveGrabUsage(ExtensionalInteractObject grabber)
        {
            if (grabber && usage.Remove(GrabbedUsageKey, grabber))
            {
                if (grabberCount == 0)
                {
                    rigidbodyRecorder.RecoverExcludeVelocity();
                    rigidbodyRecorder.Clear();
                }
                return true;
            }
            return false;
        }

        #endregion

        #region 碰撞判断

        /// <summary>
        /// 是否产生碰撞
        /// </summary>
        public bool isCollision => entercolliders.Count > 0;

        /// <summary>
        /// 与可抓对象产生碰撞进入或触发器进入的碰撞体
        /// </summary>
        public List<Collider> entercolliders { get; private set; } = new List<Collider>();

        private void AddCollider(Collider collider)
        {
            if (!entercolliders.Contains(collider))
            {
                entercolliders.Add(collider);
            }
        }

        private void RemoveCollider(Collider collider)
        {
            entercolliders.Remove(collider);
        }

        #endregion
    }

    #region 可抓处理器

    /// <summary>
    /// 可抓对象宿主
    /// </summary>
    [RequireComponent(typeof(Grabbable))]
    public abstract class GrabbableHost : Interactor
    {
        /// <summary>
        /// 可抓对象
        /// </summary>
        public Grabbable grabbable => this.XGetComponentInParent(ref _grabbable);

        private Grabbable _grabbable;

        /// <summary>
        /// 能否交互
        /// </summary>
        public virtual bool canInteract => true;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            grabbable.grabbableHosts.Add(this);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            grabbable.grabbableHosts.Remove(this);
        }
    }

    #endregion

    #region 可抓交互数据

    /// <summary>
    /// 持有交互数据
    /// </summary>
    public class HoldedInteractData : InteractData<HoldedInteractData>
    {
        /// <summary>
        /// 拖拽器与抓数据
        /// </summary>
        public Dictionary<Dragger, GrabInteractData> draggerInteractDatas { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public HoldedInteractData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactObject"></param>
        /// <param name="draggerInteractDatas"></param>
        public HoldedInteractData(string cmdName, InteractObject interactObject, Dictionary<Dragger, GrabInteractData> draggerInteractDatas) : base(cmdName, interactObject)
        {
            this.draggerInteractDatas = draggerInteractDatas;
        }

        /// <summary>
        /// 复制到
        /// </summary>
        /// <param name="interactData"></param>
        protected override void CopyTo(HoldedInteractData interactData)
        {
            interactData.draggerInteractDatas = this.draggerInteractDatas;
        }
    }

    /// <summary>
    /// 抓交互数据
    /// </summary>
    public class GrabInteractData : InteractData<GrabInteractData>
    {
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3? position { get; private set; } = null;

        /// <summary>
        /// 旋转
        /// </summary>
        public Quaternion? rotation { get; private set; } = null;

        /// <summary>
        /// 缩放
        /// </summary>
        public Vector3? scale { get; private set; } = null;

        /// <summary>
        /// 撞击偏差量
        /// </summary>
        public Vector3? hitOffset { get; private set; } = null;

        /// <summary>
        /// 撞击偏差缩放量
        /// </summary>
        public float? hitOffsetScale { get; private set; } = null;

        /// <summary>
        /// 撞击用点
        /// </summary>
        public Vector3? hitPosition { get; private set; } = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GrabInteractData() { }

        /// <summary>
        /// 构造函数:使用位置、旋转和缩放等数据构建交互数据
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public GrabInteractData(Vector3? position, Quaternion? rotation, Vector3? scale, string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, interactor, interactables)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        /// <summary>
        /// 构造函数:使用撞击偏差量和撞击点构建交互数据
        /// </summary>
        /// <param name="hitOffset"></param>
        /// <param name="hitOffsetScale"></param>
        /// <param name="hitPosition"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public GrabInteractData(Vector3? hitOffset, float? hitOffsetScale, Vector3? hitPosition, string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, interactor, interactables)
        {
            this.hitOffset = hitOffset;
            this.hitOffsetScale = hitOffsetScale;
            this.hitPosition = hitPosition;
        }

        /// <summary>
        /// 构造函数:使用目标位置、撞击偏差量和撞击点构建交互数据
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="hitOffset"></param>
        /// <param name="hitOffsetScale"></param>
        /// <param name="hitPosition"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public GrabInteractData(Vector3? position, Quaternion? rotation, Vector3? hitOffset, float? hitOffsetScale, Vector3? hitPosition, string cmdName, InteractObject interactor, params InteractObject[] interactables) : this(hitOffset, hitOffsetScale, hitPosition, cmdName, interactor, interactables)
        {
            this.position = position;
            this.rotation = rotation;
        }

        /// <summary>
        /// 复制到
        /// </summary>
        /// <param name="interactData"></param>
        protected override void CopyTo(GrabInteractData interactData)
        {
            interactData.position = position;
            interactData.rotation = rotation;
            interactData.scale = scale;
            interactData.hitOffset = hitOffset;
            interactData.hitOffsetScale = hitOffsetScale;
            interactData.hitPosition = hitPosition;
        }
    }

    #endregion
}

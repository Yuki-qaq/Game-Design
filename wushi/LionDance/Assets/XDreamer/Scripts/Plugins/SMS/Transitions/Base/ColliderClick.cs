using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.Transitions.Base
{
    /// <summary>
    /// 碰撞体点击
    /// </summary>
    [ComponentMenu("基础/碰撞体点击", typeof(SMSManager))]
    [Name("碰撞体点击")]
    public class ColliderClick : TransitionComponent
    {
        /// <summary>
        /// 点击类型
        /// </summary>
        [Name("点击类型")]
        public enum EClickType
        {
            /// <summary>
            /// 按下并弹起
            /// </summary>
            [Name("按下并弹起")]
            DownAndUp = 0,

            /// <summary>
            /// 按下
            /// </summary>
            [Name("按下")]
            Down,

            /// <summary>
            /// 弹起
            /// </summary>
            [Name("弹起")]
            Up,
        }

        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject go;

        private Collider collider;

        /// <summary>
        /// 点击类型
        /// </summary>
        [Name("点击类型")]
        [EnumPopup]
        public EClickType clickType = EClickType.DownAndUp;

        /// <summary>
        /// 最大距离:射线检测的最大距离
        /// </summary>
        [Name("最大距离")]
        [Tip("射线检测的最大距离", "Maximum distance of radiographic testing")]
        [Min(0.01f)]
        public float _maxDistance = 1000f;

        /// <summary>
        /// 图层遮罩:射线检测时的图层遮罩
        /// </summary>
        [Name("图层遮罩")]
        [Tip("射线检测时的图层遮罩", "Layer mask during radiographic testing")]
        public LayerMask _layerMask = Physics.DefaultRaycastLayers;

        /// <summary>
        /// 自动添加碰撞体
        /// </summary>
        [Name("自动添加碰撞体")]
        [Tip("没有碰撞体，点击事件就不会产生！", "Without collision body, click event will not occur!")]
        public bool addCollider = true;

        private bool isDown = false;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            if (go && addCollider)
            {
                collider = go.GetComponent<Collider>();
                if (!collider)
                {
                    if (go.GetComponent<MeshRenderer>())
                    {
                        collider = go.XAddComponent<MeshCollider>();
                    }
                    else
                    {
                        collider = go.XAddComponent<BoxCollider>();
                    }
                }
            }
            return base.Init(data);
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            isDown = false;
        }

        private bool IsOnCollider()
        {
            if (!collider) return false;
            var cam = Camera.main;
            if (!cam)
            {
                Log.Warning("主相机缺失!");
                return false;
            }

            if (Physics.Raycast(cam.ScreenPointToRay(XInput.mousePosition), out RaycastHit hitInfo, _maxDistance, _layerMask))
            {
                return hitInfo.collider == collider;
            }
            return false;
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            if (XInput.GetMouseButtonDown(0))
            {
                switch (clickType)
                {
                    case EClickType.DownAndUp:
                        {
                            isDown = IsOnCollider();
                            break;
                        }
                    case EClickType.Down:
                        {
                            finished = IsOnCollider();
                            break;
                        }
                }
            }
            else if (XInput.GetMouseButtonUp(0))
            {
                switch (clickType)
                {
                    case EClickType.DownAndUp:
                        {
                            finished = isDown && IsOnCollider();
                            SkipHelper.Skip(data, parent);
                            break;
                        }
                    case EClickType.Up:
                        {
                            finished = IsOnCollider();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return go;
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return go ? go.name : "";
        }
    }
}

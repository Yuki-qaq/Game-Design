using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCameras.Characters.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Windows.ListViews;

namespace XCSJ.PluginXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 导航图移动:驱动导航图中设定的玩家在导航图上移动
    /// </summary>
    [Name("导航图移动")]
    [DisallowMultipleComponent]
    public sealed class MiniMapMovement : MiniMapCompent
    {
        /// <summary>
        /// 移动规则
        /// </summary>
        [Name("移动规则")]
        [Tip("在导航图中点击，可将玩家移动到对应的三维空间上的地点的规则", "Click in the navigation map to move the player to the place in the corresponding three-dimensional space")]
        [EnumPopup]
        public List<EMovementRule> _movementRules = new List<EMovementRule>();

        #region 移动规则

        /// <summary>
        /// 移动规则
        /// </summary>
        [Name("移动规则")]
        public enum EMovementRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [EnumFieldName("无")]
            None,

            /// <summary>
            /// 无碰撞传送
            /// </summary>
            [EnumFieldName("无碰撞传送")]
            [Tip("导航图UI坐标换算到世界坐标进行传送", "Convert navigation map UI coordinates to world coordinates for transmission")]
            Teleport,

            /// <summary>
            /// 使用射线撞击传送
            /// </summary>
            [EnumFieldName("使用射线撞击传送")]
            [Tip("从地图上方垂直发射一条射线，与地面物体有碰撞后才能传送", "Emit a ray vertically from the top of the map, only after colliding with ground objects can it be transmitted")]
            TeleportUseRayHit,

            /// <summary>
            /// 使用导航网格移动
            /// </summary>
            [EnumFieldName("使用导航网格移动")]
            [Tip("使用玩家游戏对象自身的导航网格代理实现移动", "Using the player's game object's own navigation grid proxy to achieve mobility")]
            UseNavMesh,
        }

        #endregion

        /// <summary>
        /// 偏移高度
        /// </summary>
        [Name("偏移高度")]
        [Tip("传送到目标点的偏移高度", "Offset height transmitted to target point")]
        public float _heightOffset = 0.1f;

        private ContentArea miniMapContentArea = null;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _movementRules.Add(EMovementRule.UseNavMesh);
            _movementRules.Add(EMovementRule.TeleportUseRayHit);
            _movementRules.Add(EMovementRule.Teleport);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (miniMap._UGUIWindow) 
            {
                miniMapContentArea = miniMap._UGUIWindow.content.GetComponent<ContentArea>();
            }
        }

        /// <summary>
        /// 当输入交互时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="interactData"></param>
        protected override void OnInputInteract(InteractObject sender, InteractData interactData)
        {
            base.OnInputInteract(sender, interactData);

            if (miniMapContentArea && sender == miniMapContentArea && interactData.isInteractSuccessAndExit
                && interactData is ViewInteractData viewInteractData && viewInteractData.pointerEventData != null)
            {
                if (miniMap.TryGetClickPointInMap(viewInteractData.pointerEventData.position, out Vector2 uiLocalPointNormalized))
                {
                    OnClick(miniMap.miniMapCamera.ViewportToWorldPoint(uiLocalPointNormalized));
                }
            }
        }

        private void OnClick(Vector3 pointWorld)
        {
            foreach (var item in _movementRules)
            {
                switch (item)
                {
                    case EMovementRule.Teleport:
                        {
                            if (Teleport(pointWorld, false)) return;
                            break;
                        }
                    case EMovementRule.TeleportUseRayHit: 
                        {
                            if (Teleport(pointWorld, true)) return;
                            break;
                        }
                    case EMovementRule.UseNavMesh:
                        {
                            if (UseNavMesh(pointWorld)) return;
                            break;
                        }
                }
            }
        }

        private bool Teleport(Vector3 pointWorld, bool needHit)
        {
            var hit = Physics.Raycast(new Ray(pointWorld, Vector3.down), out RaycastHit hitInfo);
            if (needHit && !hit) return false;

            miniMap.player.position = GetMovePosition(miniMap.player.transform, hit ? hitInfo.point : pointWorld);
            return true;
        }

        private Vector3 GetMovePosition(Transform moveTransform, Vector3 clickPoint)
        {
            float heightOffset = _heightOffset;
            if (CommonFun.GetBounds(out Bounds bounds, moveTransform))
            {
                heightOffset += bounds.size.y / 2;
            }
            if (Physics.Raycast(new Ray(moveTransform.position, Vector3.down), out RaycastHit hitInfo))
            {
                heightOffset += moveTransform.position.y - hitInfo.point.y;
            }

            return new Vector3(clickPoint.x, clickPoint.y + heightOffset, clickPoint.z);
        }

        private bool UseNavMesh(Vector3 pointWorld)
        {
            var hit = Physics.Raycast(new Ray(pointWorld, Vector3.down), out RaycastHit hitInfo);
            if (!hit) return false;

            var ctrl = miniMap.player.GetComponentInChildren<INavMeshAgentController>();
            if (ctrl != null)
            {
                ctrl.SetAgentDestination(hitInfo.point);
                return true;
            }
            return false;
        }
    }
}

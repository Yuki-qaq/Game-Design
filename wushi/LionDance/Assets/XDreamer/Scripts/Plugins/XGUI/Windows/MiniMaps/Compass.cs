using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 指南针:通过设置地图中的三维参考对象来指示方向
    /// </summary>
    [Name("指南针")]
    [Tip("通过设置地图中的三维参考对象来指示方向", "Indicates the direction by setting up 3D reference objects in the map")]
    [RequireComponent(typeof(RectTransform))]
    public class Compass : MiniMapCompent
    {
        /// <summary>
        /// 关联UI
        /// </summary>
        [Name("关联UI")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RectTransform _ui = null;

        /// <summary>
        /// 位于导航图边沿
        /// </summary>
        [Name("位于导航图边沿")]
        public bool _onMapEdge = true;

        /// <summary>
        /// 旋转设定
        /// </summary>
        [Name("自转")]
        public bool _selfRotation = false;

        /// <summary>
        /// UI Z轴旋转偏移量
        /// </summary>
        [Name("旋转偏移量")]
        [HideInSuperInspector(nameof(_selfRotation), EValidityCheckType.False)]
        [LimitRange(0, 360)]
        public float _rotationOffset = 0f;

        /// <summary>
        /// 唤醒
        /// </summary>
        private void Awake()
        {
            if (!_ui)
            {
                _ui = GetComponent<RectTransform>();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            if (!_ui || !miniMap._UGUIWindow) return;

            var uiSize = miniMap.size;

            if (_selfRotation)
            {
                _ui.rotation = Quaternion.Euler(0, 0, miniMap.miniMapCamera.transform.eulerAngles.y + _rotationOffset + miniMap.GetDirectionYAngle());
            }

            // 更新位置
            if (_onMapEdge)
            {
                var dir = Quaternion.Euler(0, -miniMap.miniMapCamera.transform.eulerAngles.y, 0) * miniMap.GetDirection();

                var point = new Vector2(dir.x, dir.z);
                point.Normalize();

                switch (miniMap._minimapType)
                {
                    case MiniMap.EMiniMapType.Circle:
                        {
                            point.Scale(uiSize);
                            _ui.anchoredPosition = point / 2;
                            break;
                        }
                    case MiniMap.EMiniMapType.Rect:
                        {
                            if (point.x == 0 || point.y == 0)
                            {
                                point.Scale(uiSize);
                            }
                            else
                            {
                                // 比较斜率, 当前向量斜率大于地图矩形斜率
                                var k = point.y / point.x;
                                if (Mathf.Abs(k) > Mathf.Abs(uiSize.y / uiSize.x))
                                {
                                    point.y = point.y > 0 ? uiSize.y : -uiSize.y;
                                    point.x = point.y / k;
                                }
                                else
                                {
                                    point.x = point.x > 0 ? uiSize.x : -uiSize.x;
                                    point.y = point.x * k;
                                }
                            }
                            _ui.anchoredPosition = point / 2;
                            break;
                        }
                }
            }
        } 
    }
}

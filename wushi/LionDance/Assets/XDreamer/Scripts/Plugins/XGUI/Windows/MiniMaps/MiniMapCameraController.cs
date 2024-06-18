using Unity.Collections;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 导航图相机控制器 : 用于控制相机同步玩家的位置和角度，以及控制正交相机看到的范围
    /// </summary>
    [Name("导航图相机控制器")]
    public class MiniMapCameraController : MiniMapCompent
    {
        /// <summary>
        /// 关联相机
        /// </summary>
        [Name("关联相机")]
        public Camera _camera = null;

        /// <summary>
        /// 关联相机
        /// </summary>
        public Camera linkCamera => _camera;

        /// <summary>
        /// 运动模式
        /// </summary>
        public enum EFollowPlayerMode
        {
            /// <summary>
            /// 无:导航图不移动也不转动
            /// </summary>
            [Name("无")]
            [Tip("导航图不移动也不转动", "The navigation map does not move or rotate")]
            None,

            /// <summary>
            /// 移动:导航图随着玩家移动
            /// </summary>
            [Name("移动")]
            [Tip("导航图随着玩家移动", "Navigation map as player moves")]
            Move,

            /// <summary>
            /// 转动:导航图随着玩家转动
            /// </summary>
            [Name("转动")]
            [Tip("导航图随着玩家转动", "Navigation map rotates with the player")]
            Rotate,

            /// <summary>
            /// 移动且转动:导航图随着玩家移动和转动
            /// </summary>
            [Name("移动且转动")]
            [Tip("导航图随着玩家移动和转动", "The navigation map moves and rotates with the player")]
            MoveAndRotate,
        }

        /// <summary>
        /// 导航图跟随玩家模式
        /// </summary>
        [Name("导航图跟随玩家模式")]
        [EnumPopup]
        [Readonly(EEditorMode.Runtime)]
        public EFollowPlayerMode _followPlayerMode = EFollowPlayerMode.Move;

        /// <summary>
        /// 缩放
        /// </summary>
        [Name("缩放")] 
        public float _zoom = 1;

        /// <summary>
        /// 缩放
        /// </summary>
        public float zoom
        {
            get => _zoom;
            set
            {
                if (value < 0) value = 0;

                _zoom = value;
                linkCamera.orthographicSize = _orgOrthographicSize * _zoom;
            }
        }

        private float _orgOrthographicSize = 0;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            if (!_camera) _camera = GetComponentInChildren<Camera>();
            _orgOrthographicSize = linkCamera.orthographicSize;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (miniMap.player)
            {
                FollowPlayer(_followPlayerMode, miniMap.player);
            }
        }

        private void FollowPlayer(EFollowPlayerMode followPlayerMode, Transform player)
        {
            switch (followPlayerMode)
            {
                case EFollowPlayerMode.Move:
                    {
                        transform.position = player.position;
                        break;
                    }
                case EFollowPlayerMode.Rotate:
                    {
                        var angle = transform.eulerAngles;
                        angle.y = player.eulerAngles.y;
                        transform.eulerAngles = angle;
                        break;
                    }
                case EFollowPlayerMode.MoveAndRotate:
                    {
                        FollowPlayer(EFollowPlayerMode.Move, player);
                        FollowPlayer(EFollowPlayerMode.Rotate, player);
                        break;
                    }
            }
        }
    }
}

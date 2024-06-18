using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// UnityEditor.PlayModeView关联类
    /// </summary>
    [LinkType("UnityEditor.PlayModeView")]
    public class PlayModeView_LinkType:EditorWindow_LinkType<PlayModeView_LinkType>
    {
        #region GetMainPlayModeViewTargetSize

        /// <summary>
        /// 获取主播放模式视图目标尺寸 方法信息
        /// </summary>
        public static XMethodInfo GetMainPlayModeViewTargetSize_MethodInfo { get; } = GetXMethodInfo(nameof(GetMainPlayModeViewTargetSize));

        /// <summary>
        /// 获取主播放模式视图目标尺寸
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetMainPlayModeViewTargetSize()
        {
            return GetMainPlayModeViewTargetSize_MethodInfo.InvokeStaticEmpty<Vector2>();
        }
        #endregion 
    }
}

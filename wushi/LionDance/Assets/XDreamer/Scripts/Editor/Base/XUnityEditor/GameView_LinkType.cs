using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 类UnityEditor.GameView关联类型
    /// </summary>
    [LinkType("UnityEditor.GameView")]
    public class GameView_LinkType : EditorWindow_LinkType<GameView_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public GameView_LinkType() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public GameView_LinkType(EditorWindow obj) : base(obj) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj"></param>
        public GameView_LinkType(object obj) : base(obj) { }

        #region GetMainGameViewTargetSize

        /// <summary>
        /// 获取主游戏视图目标尺寸 方法信息
        /// </summary>
        public static XMethodInfo GetMainGameViewTargetSize_MethodInfo { get; } = GetXMethodInfo(nameof(GetMainGameViewTargetSize));

        /// <summary>
        /// 获取主游戏视图目标尺寸
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetMainGameViewTargetSize() => (Vector2)GetMainGameViewTargetSize_MethodInfo.Invoke(null, null);

        #endregion
    }
}

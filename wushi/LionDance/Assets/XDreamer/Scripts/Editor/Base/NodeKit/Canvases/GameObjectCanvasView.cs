using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.Extension.Base.NodeKit;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.NodeKit.Canvases
{
    /// <summary>
    /// 游戏对象画布视图
    /// </summary>
    [CanvasView(typeof(GameObjectCanvasModel))]
    public class GameObjectCanvasView : CanvasView<GameObjectCanvasModel>
    {
        /// <summary>
        /// 当绘制检查器
        /// </summary>
        [Languages.LanguageTuple("Name", "名称")]
        protected internal override void OnGUIInspector()
        {
            var name = canvasModel.displayName;
            var nameNew = EditorGUILayout.TextField("Name".Tr(typeof(GameObjectCanvasView)), name);
            if (nameNew != name)
            {
                canvasModel.gameObject.XSetName(nameNew);
            }
            EditorGUILayout.Separator();

            base.OnGUIInspector();           
        }
    }
}

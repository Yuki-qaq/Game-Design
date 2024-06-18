using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base.EditorWindows;
using XCSJ.EditorTools.Windows.RichTexts;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// 富文本编辑器
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Text)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class RichTextEditorWindow : XEditorWindowWithScrollView<RichTextEditorWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "富文本编辑器";

        /// <summary>
        /// 初始化
        /// </summary>
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        /// <summary>
        /// 文本
        /// </summary>
        public string text = Product.Name;

        /// <summary>
        /// 颜色
        /// </summary>
        public Color color = Color.red;

        /// <summary>
        /// 尺寸
        /// </summary>
        public int size = 58;

        /// <summary>
        /// 当绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(UICommonOption.Copy, UICommonOption.WH24x16))
            {
                CommonFun.CopyTextToClipboardForPC(text);
            }
            EditorGUILayout.EndHorizontal();
            text = EditorRichText.TextAreaHandle(text, ref color, ref size, GUILayout.ExpandWidth(true));

            base.OnGUI();
        }

        /// <summary>
        /// 当绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            EditorRichText.SelectableLabel(text);
        }      
    }
}

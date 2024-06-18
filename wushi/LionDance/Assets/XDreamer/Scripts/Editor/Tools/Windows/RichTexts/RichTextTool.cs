using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.RichTexts;

namespace XCSJ.EditorTools.Windows.RichTexts
{
    /// <summary>
    /// 富文本工具
    /// </summary>
    public class RichTextTool : TICache<RichTextTool, string, RichTextTool.Info>
    {
        #region Cache机制

        /// <summary>
        /// 信息
        /// </summary>
        public class Info
        {
            /// <summary>
            /// 唯一编号
            /// </summary>
            public string guid = "";

            /// <summary>
            /// 编辑模式
            /// </summary>
            public bool editMode = false;

            /// <summary>
            /// 富文本样式
            /// </summary>
            public RichTextStyle richTextStyle = new RichTextStyle();

            /// <summary>
            /// 检查编辑模式
            /// </summary>
            public void CheckEditMode()
            {
                if (!richTextStyle.active)
                {
                    richTextStyle.active = true;
                    editMode = false;
                }
            }
        }

        /// <summary>
        /// 创建值
        /// </summary>
        /// <param name="key1"></param>
        /// <returns></returns>
        protected override KeyValuePair<bool, Info> CreateValue(string key1)
        {
            return new KeyValuePair<bool, Info>(true, new Info() { guid = key1 });
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="text"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static Info Get(string text, string guid)
        {
            if (string.IsNullOrEmpty(guid)) return Default<Info>.Instance;
            if (!Cache.TryGetValue(guid, out Info info))
            {
                Cache[guid] = info = new Info() { guid = guid };
                info.richTextStyle.Set(text);
            }
            return info;
        }

        #endregion

        /// <summary>
        /// 可选择的标签
        /// </summary>
        /// <param name="element"></param>
        /// <param name="editMode"></param>
        /// <param name="style"></param>
        public static void SelectableLabel(Element element, bool editMode, GUIStyle style = null)
        {
            SelectableLabel(ref element.value, element.guid, editMode, style);
        }

        /// <summary>
        /// 可选择的标签
        /// </summary>
        /// <param name="text"></param>
        /// <param name="guid"></param>
        /// <param name="editMode"></param>
        /// <param name="style"></param>
        public static void SelectableLabel(ref string text, string guid, bool editMode, GUIStyle style = null)
        {
            if (editMode)
            {
                var info = Get(text, guid);
                if (EditorRichText.Button(text, style))
                {
                    info.editMode = !info.editMode;
                }
                if (info.editMode)
                {
                    text = EditorRichText.TextAreaHandle(text, info.richTextStyle);
                    info.CheckEditMode();
                }
            }
            else
            {
                EditorRichText.SelectableLabel(text, style);
            }
        }

        /// <summary>
        /// 标签
        /// </summary>
        /// <param name="content"></param>
        /// <param name="editMode"></param>
        /// <param name="style"></param>
        public static void Label(Content content, bool editMode, GUIStyle style = null)
        {
            if (editMode)
            {
                SelectableLabel(content.name, editMode, style);
                SelectableLabel(content.tip, editMode);
            }
            else
            {
                EditorRichText.Label(content.content, style);
            }
        }

        /// <summary>
        /// 按钮
        /// </summary>
        /// <param name="element"></param>
        /// <param name="editMode"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static bool Button(Element element, bool editMode, GUIStyle style = null)
        {
            return Button(ref element.value, element.guid, editMode, style);
        }

        /// <summary>
        /// 按钮
        /// </summary>
        /// <param name="text"></param>
        /// <param name="guid"></param>
        /// <param name="editMode"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static bool Button(ref string text, string guid, bool editMode, GUIStyle style = null)
        {
            if (editMode)
            {
                var info = Get(text, guid);
                if (EditorRichText.Button(text, style))
                {
                    info.editMode = !info.editMode;
                }
                if (info.editMode)
                {
                    text = EditorRichText.TextAreaHandle(text, info.richTextStyle);
                    info.CheckEditMode();
                }
                return false;
            }
            else
            {
                return EditorRichText.Button(text, style);
            }
        }
    }
}

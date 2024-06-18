using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集窗口通过IMGUI
    /// </summary>
    [Name("选择集窗口通过IMGUI")]
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [Tool(ToolsCategory.SelectionSet, disallowMultiple = true, rootType = typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public class SelectionWindowByIMGUI : InteractProvider
    {
        /// <summary>
        /// 选择集窗口
        /// </summary>
        [Name("选择集窗口")]
        public SelectionWindow _selectionWindow = new SelectionWindow();

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Selection.selectionChanged += OnSelectionChanged;
            OnSelectionChanged(null, false);
            _selectionWindow.SetWindowAligin();
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            Selection.selectionChanged -= OnSelectionChanged;
        }

        /// <summary>
        /// 当选择集已修改
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected void OnSelectionChanged(GameObject[] oldSelections, bool flag)
        {
            _selectionWindow.gameObjects = Selection.selections.Where(go => go);
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        protected void OnGUI()
        {
            _selectionWindow.OnGUI();
        }
    }

    /// <summary>
    /// 选择集窗口
    /// </summary>
    [Serializable]
    [Name("选择集窗口")]
    public class SelectionWindow : BaseGUIWindow
    {
        /// <summary>
        /// 自动布局
        /// </summary>
        public override bool autoLayout => true;

        /// <summary>
        /// 游戏对象列表
        /// </summary>
        public IEnumerable<GameObject> gameObjects { get; internal set; }

        /// <summary>
        /// 当绘制内容布局
        /// </summary>
        protected override void OnDrawContentLayout()
        {
            if (gameObjects == null) return;
            foreach(var go in gameObjects)
            {
                if(go && GUILayout.Button(go.name))
                {
                    Debug.Log(CommonFun.GameObjectToString(go));
#if UNITY_EDITOR
                    UnityEditor.EditorGUIUtility.PingObject(go);
#endif
                }
            }
        }
    }
}

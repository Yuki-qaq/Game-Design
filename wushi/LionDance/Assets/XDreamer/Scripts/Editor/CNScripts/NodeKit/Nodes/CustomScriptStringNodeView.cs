using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCNScripts.NodeKit.Canvases;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.EditorCNScripts.NodeKit.Nodes
{
    /// <summary>
    /// 自定义脚本字符串节点视图
    /// </summary>
    [NodeView(typeof(CustomScriptString))]
    public class CustomScriptStringNodeView : NodeView<CustomScriptString>
    {
        /// <summary>
        /// 脚本管理器
        /// </summary>
        static ScriptManager scriptManager => ScriptManager.instance;

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            scriptListDrawer.onVisible += OnVisible;
            scriptListDrawer.onDrawScript += OnDrawScript;

            content = (index + 1).ToString();
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            _scriptStringDrawer = default;
            scriptListDrawer.onVisible -= OnVisible;
            scriptListDrawer.onDrawScript -= OnDrawScript;
        }

        Vector2 inspectorScrollValue = Vector2.zero;

        /// <summary>
        /// 当绘制检查器
        /// </summary>
        protected internal override void OnGUIInspector()
        {
            //base.OnGUIInspector();

            toolbar = UICommonFun.Toolbar(toolbar, ENameTip.Image, UICommonOption.Height24);
            EditorGUILayout.Space(2);
            inspectorScrollValue = EditorGUILayout.BeginScrollView(inspectorScrollValue);

            switch (toolbar)
            {
                case EToolbar.ScriptList:
                    {
                        DrawSriptList();
                        break;
                    }
                default:
                    {
                        DrawSriptEdit();
                        selectScript = currentScript;
                        break;
                    }
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 当聚焦
        /// </summary>
        protected override void OnFocus()
        {
            base.OnFocus();
            _scriptStringDrawer = default;
        }

        /// <summary>
        /// 当撤销重做已执行
        /// </summary>
        protected override void OnUndoRedoPerformed()
        {
            base.OnUndoRedoPerformed();
            _scriptStringDrawer = default;
        }

        /// <summary>
        /// 出节点视图列表
        /// </summary>
        public override IEnumerable<NodeView> outNodeViews
        {
            get
            {
                var index = canvasView.childrenNodeViews.IndexOf(this) + 1;
                if (index > 0 && index < canvasView.childrenNodeViews.Count)
                {
                    yield return canvasView.childrenNodeViews[index];
                }
            }
        }

        string content = "";

        /// <summary>
        /// 绘制内容
        /// </summary>
        protected override void DrawContent()
        {
            base.DrawContent();
            GUI.Label(contentRect, CommonFun.TempContent(content, content), customStyle.contentTextStyle);
        }

        #region 工具栏

        EToolbar toolbar = EToolbar.ScriptEdit;

        /// <summary>
        /// 工具栏
        /// </summary>
        [Name("工具栏")]
        public enum EToolbar
        {
            /// <summary>
            /// 脚本编辑
            /// </summary>
            [Name("脚本编辑")]
            [XCSJ.Attributes.Icon(EIcon.Edit)]
            ScriptEdit,

            /// <summary>
            /// 脚本列表
            /// </summary>
            [Name("脚本列表")]
            [XCSJ.Attributes.Icon(EIcon.List)]
            ScriptList,
        }

        #endregion

        #region 脚本列表


        Script _selectScript;

        /// <summary>
        /// 选择脚本
        /// </summary>
        Script selectScript
        {
            get => _selectScript;
            set
            {
                _selectScript = value;
                if (_selectScript == default)
                {
                    currentScript = default;
                }
            }
        }

        ScriptListDrawer _scriptListDrawer;

        ScriptListDrawer scriptListDrawer => _scriptListDrawer ?? (_scriptListDrawer = new ScriptListDrawer(() => selectScript, value => selectScript = value));

        /// <summary>
        /// 左侧 滚动条区域的位置信息
        /// </summary>
        Vector2 leftAreaScrollPos = new Vector2(0, 0);

        void OnVisible(float y) => leftAreaScrollPos.y = y;

        void OnDrawScript(ScriptCategory scriptCategory)
        {
            if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonLeft, UICommonOption.Width20, UICommonOption.Height20))
            {
                selectScript= scriptCategory.script;

                var ss = new ScriptStringDrawer();
                ss.script = scriptCategory.script;
                nodeModel.XAddScriptString(ss.ToScriptString(), false);
            }
            if (GUILayout.Button(UICommonOption.InsertChild, EditorStyles.miniButtonMid, UICommonOption.Width20, UICommonOption.Height20))
            {
                selectScript = scriptCategory.script;

                var ss = new ScriptStringDrawer();
                ss.script = scriptCategory.script;
                nodeModel.XAddScriptString(ss.ToScriptString(), true);
            }
            if (GUILayout.Button(UICommonOption.Script, EditorStyles.miniButtonRight, UICommonOption.Width20, UICommonOption.Height20))
            {
                selectScript = scriptCategory.script;
                currentScript = scriptCategory.script;
            }
        }

        private void DrawSriptList()
        {
            scriptListDrawer.HandleEvent();
            scriptListDrawer.DrawSearch();

            leftAreaScrollPos = EditorGUILayout.BeginScrollView(leftAreaScrollPos, false, false, GUILayout.ExpandHeight(true));
            scriptListDrawer.DrawScriptList();
            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region 脚本编辑

        ScriptStringDrawer _scriptStringDrawer;

        ScriptStringDrawer scriptStringDrawer => _scriptStringDrawer ?? (_scriptStringDrawer = ScriptStringDrawer.FromScriptString(nodeModel, scriptManager));

        /// <summary>
        /// 当前脚本对象
        /// </summary>
        Script currentScript
        {
            get => scriptStringDrawer.script;
            set
            {
                if (scriptStringDrawer.script != value || scriptStringDrawer.script == null)
                {
                    CommonFun.FocusControl(ScriptListDrawer.SearchScriptTextControlName);
                    if (value != null && scriptManager)
                    {
                        scriptStringDrawer.script = scriptManager.IDScripts[value.id];
                    }
                    else
                    {
                        scriptStringDrawer.script = value;
                    }

                    //设置后更新脚本字符串
                    nodeModel.XSetScriptString(scriptStringDrawer.ToScriptString());
                }
            }
        }

        private void DrawSriptEdit()
        {
            EditorGUI.BeginChangeCheck();

            var scriptStringDrawer = this.scriptStringDrawer;

            //脚本命令
            scriptStringDrawer.DrawScriptCmd();

            //脚本参数
            scriptStringDrawer.DrawScriptParams();

            //脚本返回值
            scriptStringDrawer.DrawScriptReturnValue();

            //脚本字符串
            scriptStringDrawer.DrawScriptString();

            //脚本描述
            scriptStringDrawer.DrawScriptDescription();

            if (EditorGUI.EndChangeCheck())
            {
                UICommonFun.DelayCall(() => nodeModel.XSetScriptString(scriptStringDrawer.ToScriptString()));
            }
        }

        #endregion
    }
}


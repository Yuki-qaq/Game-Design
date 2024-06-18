using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Effects;
using XCSJ.PluginTools.Motions;
using XCSJ.PluginXGUI.Widgets;

namespace XCSJ.EditorXGUI.Widgets
{
    /// <summary>
    /// 对话框检查器
    /// </summary>
    [Name("对话框检查器")]
    [CustomEditor(typeof(DialogBox), true)]
    public class DialogBoxInspector : InteractorInspector<DialogBox>
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            InitTextTypewriter();
        }

        private TextTypewriter textTypewriter = null;
        private EffectController effectController = null;

        private void InitTextTypewriter()
        {
            if (!targetObject) return;

            textTypewriter = targetObject.GetComponentInChildren<TextTypewriter>();
            if (textTypewriter)
            {
                enableTextTypewriter = textTypewriter.enabled;
                effectController = targetObject.GetComponentInChildren<EffectController>();
                if (effectController)
                {
                    enableEffectController = effectController.enabled;
                }
            }
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawTypeWriter();
        }

        /// <summary>
        /// 启用打字特效
        /// </summary>
        [Name("启用打字特效")]
        private bool enableTextTypewriter = false;

        /// <summary>
        /// 打字完后显示交互按钮
        /// </summary>
        [Name("打字完后显示交互按钮")]
        private bool enableEffectController = true;

        private void DrawTypeWriter()
        {
            if (!textTypewriter) return;

            EditorGUI.BeginChangeCheck();
            enableTextTypewriter = EditorGUILayout.Toggle(CommonFun.NameTip(typeof(DialogBoxInspector), nameof(enableTextTypewriter)), enableTextTypewriter);
            if (EditorGUI.EndChangeCheck())
            {
                textTypewriter.XModifyProperty(() => textTypewriter.enabled = enableTextTypewriter);
            }
            if (enableTextTypewriter)
            {
                EditorGUI.BeginChangeCheck();
                var speed = EditorGUILayout.IntField(CommonFun.NameTip(typeof(TextTypewriter), nameof(TextTypewriter._speed)), textTypewriter._speed);
                if (EditorGUI.EndChangeCheck())
                {
                    textTypewriter.XModifyProperty(() => textTypewriter._speed = speed);
                }

                if (effectController)
                {
                    EditorGUI.BeginChangeCheck();
                    enableEffectController = EditorGUILayout.Toggle(CommonFun.NameTip(typeof(DialogBoxInspector), nameof(enableEffectController)), enableEffectController);
                    if (EditorGUI.EndChangeCheck())
                    {
                        effectController.XModifyProperty(() => effectController.enabled = enableEffectController);
                    }
                }
            }
        }
    }
}

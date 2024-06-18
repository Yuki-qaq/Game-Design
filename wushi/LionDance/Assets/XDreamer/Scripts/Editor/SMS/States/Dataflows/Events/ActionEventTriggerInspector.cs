using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Base;
using XCSJ.Extension.Base.Events;
using XCSJ.PluginSMS.States.Dataflows.Events;

namespace XCSJ.EditorSMS.States.Dataflows.Events
{
    /// <summary>
    /// Action事件触发器检查器
    /// </summary>
    [Name("Action事件触发器检查器")]
    [CustomEditor(typeof(ActionEventTrigger))]
    public class ActionEventTriggerInspector : TriggerInspector<ActionEventTrigger>
    {
        /// <summary>
        /// 显示辅助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            if (stateComponent.actionEventListener.eventMethodInfo is MethodInfo method)
            {
                var parameters = method.GetParameters();
                info.AppendFormat("事件参数数目:\t{0}", parameters.Length.ToString());
                for (int i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];
                    info.AppendFormat("\n事件参数[{0}]类型:\t{1}", i.ToString(), p.ParameterType.FullName);
                }
                info.AppendFormat("\n动作方法类型:\t{0}", stateComponent.actionEventListener.actionMethodType?.FullName ?? "<未知>");
            }
            else
            {
                info.AppendFormat("<color=#FF0000FF>事件字段无效</color>");
            }
            return info;
        }

        /// <summary>
        /// 当绘制辅助信息
        /// </summary>
        public override void OnDrawHelpInfo()
        {
            base.OnDrawHelpInfo();
            if (!Application.isPlaying && stateComponent.actionEventListener.eventMethodInfo is MethodInfo method)
            {
                var parameters = method.GetParameters();
                if (parameters.Length > 0)
                {
                    if (GUILayout.Button("生成【动作方法类型】脚本"))
                    {
                        var codeCreater = new EditorExtension.GenericTypeCodeCreater(UICommonFun.GetAssetsPath(EFolder._Tmp_Scripts), stateComponent.actionEventListener.fieldInfo, ActionMethodHelper.GetActionMethodGenericType(parameters.Length), parameters.Cast(p => p.ParameterType).ToArray());
                        if (codeCreater.MakeCS(true, out var newCreate) != null || newCreate)
                        {
                            if (newCreate) Debug.Log("成功生成【动作方法类型】脚本文件：" + codeCreater.filePath);
                            else Debug.Log("【动作方法类型】脚本文件已存在：" + codeCreater.filePath);
                        }
                        else
                        {
                            Debug.LogWarning("生成【动作方法类型】脚本文件失败：可能当前应用无法处理某些特殊的参数类型");
                        }
                    }
                    if (GUILayout.Button("移除【动作方法类型】脚本"))
                    {
                        var codeCreater = new EditorExtension.GenericTypeCodeCreater(UICommonFun.GetAssetsPath(EFolder._Tmp_Scripts), stateComponent.actionEventListener.fieldInfo, ActionMethodHelper.GetActionMethodGenericType(parameters.Length), parameters.Cast(p => p.ParameterType).ToArray());
                        if (codeCreater.DeleteCS())
                        {
                            Debug.Log("成功移除【动作方法类型】脚本文件："+ codeCreater.filePath);
                        }
                    }
                    if (GUILayout.Button("编辑【动作方法类型】脚本"))
                    {
                        UICommonFun.OpenMonoScript(stateComponent.actionEventListener.actionMethodType);
                    }
                }                
            }
            if (GUILayout.Button("打开临时脚本文件夹"))
            {
                EditorUtility.RevealInFinder(UICommonFun.GetAssetsPath(EFolder._Tmp_Scripts));
            }
            if (GUILayout.Button("移除临时脚本文件夹"))
            {
                var dir = UICommonFun.GetAssetsPath(EFolder._Tmp_Scripts);
                FileUtil.DeleteFileOrDirectory(dir);
                FileUtil.DeleteFileOrDirectory(dir + ".meta");
                AssetDatabase.Refresh();
            }
        }
    }
}

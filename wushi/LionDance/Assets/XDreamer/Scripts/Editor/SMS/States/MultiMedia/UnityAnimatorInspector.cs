using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.MultiMedia;
using static XCSJ.PluginSMS.States.MultiMedia.UnityAnimator;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    /// <summary>
    /// Unity动画检查器
    /// </summary>
    [Name("Unity动画检查器")]
    [CustomEditor(typeof(UnityAnimator))]
    public class UnityAnimatorInspector : WorkClipInspector<UnityAnimator>
    {
        /// <summary>
        /// 动画
        /// </summary>
        public Animator animator => workClip._animator;

        /// <summary>
        /// 动画控制器
        /// </summary>
        public AnimatorController animatorController => animator ? animator.runtimeAnimatorController as AnimatorController : null;

        /// <summary>
        /// 层数量
        /// </summary>
        public int layerCount => animatorController ? animatorController.layers.Length : 0;

        /// <summary>
        /// 当前动画控制器层
        /// </summary>
        public AnimatorControllerLayer currrentLayer => (animatorController && animatorController.layers != null && workClip._layerIndex >= 0 && workClip._layerIndex < layerCount) ? animatorController.layers[workClip._layerIndex] : null;

        /// <summary>
        /// 动画状态机
        /// </summary>
        public AnimatorStateMachine currrentStateMachine => currrentLayer == null ? null : currrentLayer.stateMachine;

        /// <summary>
        /// 动画状态名称列表
        /// </summary>
        public List<string> motionStateNameList => currrentStateMachine ? currrentStateMachine.states.Where(s => s.state.motion).ToList(s => s.state.name) : new List<string>();

        /// <summary>
        /// 当前状态
        /// </summary>
        public AnimatorState currentState => currrentStateMachine ? currrentStateMachine.states.FirstOrDefault(s => s.state.name == workClip._stateName).state : null;

        /// <summary>
        /// 当前动画
        /// </summary>
        public Motion currentMotion => currentState ? currentState.motion : null;

        /// <summary>
        /// 当前动画剪辑
        /// </summary>
        public AnimationClip currentAnimationClip => currentMotion as AnimationClip;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            RecordStateNames();
        }

        /// <summary>
        /// 当动画信息变化
        /// </summary>
        /// <param name="memberProperty"></param>
        protected virtual void OnAnimatorInfoChanged(SerializedProperty memberProperty) 
        {
            if (workClip._playMode == UnityAnimator.EPlayMode.Range)
            {
                UICommonFun.DelayCall(0.01f, this, o =>
                {
                    if (o is UnityAnimatorInspector inspector && inspector)
                    {
                        inspector.updatePlayRange = true;
                    }
                });
            }
            RecordStateNames();
        }

        private bool updatePlayRange = false;

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(WorkClip.useInitData):
                    {
                        return;
                    }
                case nameof(workClip._layerIndex):
                    {
                        var layerCount = this.layerCount;
                        EditorGUI.BeginChangeCheck();
                        serializedProperty.intValue = EditorGUILayout.IntSlider(CommonFun.NameTooltip(workClip.GetType(), serializedProperty.name), serializedProperty.intValue, 0, (layerCount > 0 ? (layerCount - 1) : 0));
                        if (EditorGUI.EndChangeCheck())
                        {
                            OnAnimatorInfoChanged(serializedProperty);
                        }
                        return;
                    }
                case nameof(workClip._stateName):
                case nameof(workClip._playStateNameOnExit):
                    {
                        var list = motionStateNameList;
                        list.Sort();

                        EditorGUI.BeginChangeCheck();
                        serializedProperty.stringValue = UICommonFun.Popup(CommonFun.NameTooltip(workClip.GetType(), serializedProperty.name), serializedProperty.stringValue, list.ToArray(), GUILayout.Width(100));
                        if (EditorGUI.EndChangeCheck())
                        {
                            OnAnimatorInfoChanged(serializedProperty);
                        }
                        return;
                    }
                case nameof(workClip._takeRange):
                    {
                        serializedProperty.vector2IntValue = GetFrameRange(currentAnimationClip);
                        EditorGUI.BeginDisabledGroup(true);
                        UICommonFun.MinMaxSliderLayout(CommonFun.NameTooltip(workClip, serializedProperty.name), serializedProperty.vector2IntValue, serializedProperty.vector2IntValue);
                        EditorGUI.EndDisabledGroup();
                        return;
                    }
                case nameof(workClip._playRange):
                    {
                        serializedProperty.vector2IntValue = UICommonFun.MinMaxSliderLayout(CommonFun.NameTooltip(workClip, serializedProperty.name), serializedProperty.vector2IntValue, workClip._takeRange);

                        //当前状态组件内Animator属性信息修改时，自动更新播放区间为Take区间
                        if (updatePlayRange && Event.current.type == EventType.Repaint)
                        {
                            updatePlayRange = false;
                            serializedProperty.vector2IntValue = workClip._takeRange;
                        }
                        return;
                    }
            }
            EditorGUI.BeginChangeCheck();
            base.OnDrawMember(serializedProperty, propertyData);
            if (EditorGUI.EndChangeCheck())
            {
                switch (serializedProperty.name)
                {
                    case nameof(workClip._animator):
                    case nameof(workClip._layerIndex):
                    case nameof(workClip._stateName):
                        {
                            OnAnimatorInfoChanged(serializedProperty);
                            break;
                        }
                }
            }
        }

        private void RecordStateNames()
        {
            if (!workClip) return;

            var names = motionStateNameList;
            var sp = serializedObject.FindProperty(nameof(workClip._stateNames));
            sp.arraySize = names.Count;
            for (int i = 0; i < names.Count; i++)
            {
                var subSP = sp.GetArrayElementAtIndex(i);
                subSP.stringValue = names[i];
            }
        }

        /// <summary>
        /// 获取模型导入器剪辑动画
        /// </summary>
        /// <param name="animationClip"></param>
        /// <returns></returns>
        public static ModelImporterClipAnimation GetModelImporterClipAnimation(AnimationClip animationClip)
        {
            return animationClip ? GetModelImporterClipAnimation(GetAssetImporter(animationClip) as ModelImporter, animationClip.name) : null;
        }

        /// <summary>
        /// 获取模型导入器剪辑动画
        /// </summary>
        /// <param name="modelImporter"></param>
        /// <param name="animationClipName"></param>
        /// <returns></returns>
        public static ModelImporterClipAnimation GetModelImporterClipAnimation(ModelImporter modelImporter, string animationClipName)
        {
            if (modelImporter)
            {
                var clip = modelImporter.clipAnimations.FirstOrDefault(ca => ca.name == animationClipName);
                return clip != null ? clip : modelImporter.defaultClipAnimations.FirstOrDefault(ca => ca.name == animationClipName);
            }
            return null;
        }

        /// <summary>
        /// 获取资产导入器
        /// </summary>
        /// <param name="animationClip"></param>
        /// <returns></returns>
        public static AssetImporter GetAssetImporter(AnimationClip animationClip)
        {
            return animationClip ? AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(animationClip)) : null;
        }

        /// <summary>
        /// 获取帧范围
        /// </summary>
        /// <param name="animationClip"></param>
        /// <returns></returns>
        public static Vector2Int GetFrameRange(AnimationClip animationClip) => GetFrameRange(GetModelImporterClipAnimation(animationClip));

        /// <summary>
        /// 获取帧范围
        /// </summary>
        /// <param name="modelImporterClipAnimation"></param>
        /// <returns></returns>
        public static Vector2Int GetFrameRange(ModelImporterClipAnimation modelImporterClipAnimation)
        {
            return modelImporterClipAnimation != null ? new Vector2Int(Mathf.RoundToInt(modelImporterClipAnimation.firstFrame), Mathf.RoundToInt(modelImporterClipAnimation.lastFrame)) : new Vector2Int();
        }

        #region 同步TL

        /// <summary>
        /// 有同步时长按钮
        /// </summary>
        /// <returns></returns>
        protected override bool HasSyncTLButton() => true;

        /// <summary>
        /// 获取同步时长按钮内容
        /// </summary>
        /// <returns></returns>
        protected override GUIContent GetSyncTLButtonContent()
        {
            if (workClip._playMode == UnityAnimator.EPlayMode.Range)
            {
                var content = CommonFun.NameTooltip(workClip, nameof(workClip.syncTL), ENameTip.EmptyTextWhenHasImage);
                content.tooltip += string.Format("\n将时长同步为播放区间帧时长");
                return content;
            }
            else
            {
                var content = base.GetSyncTLButtonContent();
                content.tooltip += string.Format("\n将时长同步为动画剪辑时长");
                return content;
            }
        }

        /// <summary>
        /// 获取预期的时长
        /// </summary>
        /// <returns></returns>
        protected override double? GetExpectedTL()
        {
            if (workClip._playMode == UnityAnimator.EPlayMode.Range)
            {
                var cac = currentAnimationClip;
                if (GetAssetImporter(cac) is ModelImporter modelImporter)
                {
                    var pl = workClip._playRange.y - workClip._playRange.x;
                    var tl = pl * cac.length / (workClip._takeRange.y - workClip._takeRange.x);

                    return tl;
                }
                return default;
            }
            else
            {
                if (currentAnimationClip)
                {
                    return currentAnimationClip.length;
                }
                return default;
            }
        }

        #endregion

        #region 同步OTL

        /// <summary>
        /// 获取同步单次时长按钮内容
        /// </summary>
        /// <returns></returns>
        protected override GUIContent GetSyncOTLButtonContent() => CommonFun.TempContent("动画时长", "将单次时长实时自动同步为动画剪辑时长");

        /// <summary>
        /// 获取预期的单次时长
        /// </summary>
        /// <returns></returns>
        protected override double? GetExpectedOTL()
        {
            var cac = currentAnimationClip;
            if (cac) return cac.length;
            return default;
        }

        #endregion

        /// <summary>
        /// 获取帮助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            info.Append("\nAnimator:\t");
            if (animator)
            {
                info.Append(CommonFun.GameObjectToString(animator.gameObject));
            }
            else
            {
                return info.Append("<color=red>数据无效!</color>"); ;
            }

            info.Append("\n动画控制器:\t");
            if (animatorController)
            {
                info.Append(AssetDatabase.GetAssetPath(animatorController));
            }
            else
            {
                return info.Append("<color=red>数据无效!</color>");
            }

            var cac = currentAnimationClip;
            info.Append("\n动画剪辑:");
            if (cac)
            {
                info.AppendFormat("\n\t名称:\t{0}", cac.name);
                info.AppendFormat("\n\t版本:\t{0}", (cac.legacy ? "Legacy" : "Mecanim"));

                var assetImporter = GetAssetImporter(cac);
                if (assetImporter)
                {
                    info.AppendFormat("\n\t路径:\t{0}", assetImporter.assetPath);
                    info.AppendFormat("\n\t导入器:\t{0}", assetImporter.GetType().ToString());
#if CSHARP_7_3_OR_NEWER
                    if (assetImporter is ModelImporter modelImporter)
                    {
#else
                    if (assetImporter is ModelImporter)
                    {
                        var modelImporter = (ModelImporter)assetImporter;
#endif
                        var clip = GetModelImporterClipAnimation(modelImporter, cac.name);
                        if (clip != null)
                        {
                            var range = GetFrameRange(clip);

                            info.AppendFormat("\n\t\t名称:\t{0}", clip.name);
                            info.AppendFormat("\n\t\tTake名:\t{0}", clip.takeName);
                            info.AppendFormat("\n\t\t帧区间:\t[{0}, {1}]", range.x, range.y);
                            info.AppendFormat("\n\t\t帧数:\t{0}", (range.y - range.x));
                        }
                        else
                        {
                            info.Append("\n\t\t<color=red>模型导入器无法识别有效的动画剪辑Take信息!</color>");
                        }
                    }
                    else
                    {
                        //可能是使用 Animation窗口 编辑的动画片段
                        var isAnim = assetImporter.assetPath.EndsWith(".anim", StringComparison.OrdinalIgnoreCase);
                        info.AppendFormat("\n\t\tAnim:\t{0}", isAnim ? "是" : "否");
                    }
                }
                else
                {
                    info.Append("\n\t<color=red>资源导入器无法识别有效的动画剪辑!</color>");
                }

                info.AppendFormat("\n\tFPS:\t{0}", cac.frameRate);
                info.AppendFormat("\n\t时长:\t{0}", cac.length);
            }
            else
            {
                info.AppendFormat("\n\t<color=red>数据无效!</color>");
            }

            if (workClip._playMode == UnityAnimator.EPlayMode.Range)
            {
                var pl = workClip._playRange.y - workClip._playRange.x;
                info.AppendFormat("\n播放区间帧长:\t{0}", pl.ToString());
                info.Append("\n播放区间帧时长:\t");

                if (GetAssetImporter(cac) is ModelImporter modelImporter)
                {
                    info.Append((pl * cac.length / (workClip._takeRange.y - workClip._takeRange.x)).ToString());
                }
                else
                {
                    info.Append("<color=red>区间动画无法处理当前类型的动画剪辑!</color>");
                }
            }

            return info;
        }
    }
}

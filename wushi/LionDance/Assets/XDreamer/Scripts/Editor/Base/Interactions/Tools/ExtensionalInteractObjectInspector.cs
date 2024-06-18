using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.PropertyDatas;

namespace XCSJ.EditorExtension.Base.Interactions.Tools
{
    /// <summary>
    /// 交互标签对象检查器
    /// </summary>
    [CustomEditor(typeof(ExtensionalInteractObject), true)]
    [CanEditMultipleObjects]
    public class ExtensionalInteractObjectInspector : ExtensionalInteractObjectInspector<ExtensionalInteractObject> { }

    /// <summary>
    /// 交互标签对象检查器模板
    /// </summary>
    public class ExtensionalInteractObjectInspector<T> : InteractObjectInspector<T> where T : ExtensionalInteractObject
    {
        private bool existRepeatInCmdName = false;
        private bool existRepeatOutCmdName = false;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!targetObject) return;

            CheckRepeatInCmdName();
            CheckRepeatOutCmdName();
        }

        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [LanguageTuple("The input command name cannot be duplicate!", "输入命令名称不能重复!")]
        [LanguageTuple("The output command name cannot be duplicate!", "输出命令名称不能重复!")]
        [LanguageTuple("Key Word", "关键字")]
        [LanguageTuple("User", "使用者")]
        [LanguageTuple("Mark Dirty", "标记脏")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(ExtensionalInteractObject._inCmds):
                    {
                        EditorGUI.BeginChangeCheck();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (EditorGUI.EndChangeCheck())
                        {
                            UICommonFun.DelayCall(CheckRepeatInCmdName);
                        }
                        if (existRepeatInCmdName)
                        {
                            UICommonFun.RichHelpBox(Tr("The input command name cannot be duplicate!"), MessageType.Error);
                        }
#if XDREAMER_EDITION_XDREAMERDEVELOPER
                        DrawTestInCmd();
#endif
                        return;
                    }
                case nameof(ExtensionalInteractObject._outCmds):
                    {
                        EditorGUI.BeginChangeCheck();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (EditorGUI.EndChangeCheck())
                        {
                            UICommonFun.DelayCall(CheckRepeatOutCmdName);
                        }
                        if (existRepeatOutCmdName)
                        {
                            UICommonFun.RichHelpBox(Tr("The output command name cannot be duplicate!"), MessageType.Error);
                        }
                        DrawUsage();
                        return;
                    }
                case nameof(ExtensionalInteractObject._dirty):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (GUILayout.Button(Tr("Mark Dirty"), UICommonOption.Width80))
                        {
                            mb.XModifyProperty(mb.MarkDirty);
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private void CheckRepeatInCmdName()
        {
            var list = targetObject.inCmdNameList;
            var hashset = new HashSet<string>();
            existRepeatInCmdName = list.Any(cmdName => !hashset.Add(cmdName));
        }

        private void CheckRepeatOutCmdName()
        {
            var list = targetObject.outCmdNameList;
            var hashset = new HashSet<string>();
            existRepeatOutCmdName = list.Any(cmdName => !hashset.Add(cmdName));
        }

        #region 测试输入命令

        /// <summary>
        /// 测试输入命令
        /// </summary>
        [Name("测试输入命令")]
        public bool _testInCmd = false;

        /// <summary>
        /// 输入命令名称
        /// </summary>
        [Name("入命令名称")]
        public string _inCmdName = "";

        /// <summary>
        /// 输入命令
        /// </summary>
        [Name("输入命令")]
        public string _inCmd = "";

        /// <summary>
        /// 输入命令参数
        /// </summary>
        [Name("输入命令参数")]
        public string _inCmdParam = "";

        /// <summary>
        /// 输入命令名称与命令参数
        /// </summary>
        [Name("输入命令名称与命令参数")]
        public string _inCmdNameAndCmdParam = "";

        /// <summary>
        /// 输入命令与命令参数
        /// </summary>
        [Name("输入命令与命令参数")]
        public string _inCmdAndCmdParam = "";

        private void DrawTestInCmd()
        {
            // 折叠
            _testInCmd = UICommonFun.Foldout(_testInCmd, TrLabel(nameof(_testInCmd)));
            if (!_testInCmd) return;
            CommonFun.BeginLayout();
            {
                EditorGUILayout.BeginHorizontal();
                _inCmdName = UICommonFun.Popup(TrLabel(nameof(_inCmdName)), _inCmdName, mb.inCmds.cmds.Cast(c => c.cmdName).ToArray());
                if (GUILayout.Button(UICommonOption.Run, UICommonOption.Width80, UICommonOption.Height20))
                {
                    mb.InvokeInteractByCmdNameAndCmdParam(_inCmdName + ":" + _inCmdParam);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                _inCmd = UICommonFun.Popup(TrLabel(nameof(_inCmd)), _inCmd, mb.GetCmds().ToArray());
                if (GUILayout.Button(UICommonOption.Run, UICommonOption.Width80, UICommonOption.Height20))
                {
                    mb.InvokeInteractByCmdAndCmdParam(_inCmd + ":" + _inCmdParam);
                }
                EditorGUILayout.EndHorizontal();

                _inCmdParam = EditorGUILayout.TextField(TrLabel(nameof(_inCmdParam)), _inCmdParam);

                EditorGUILayout.BeginHorizontal();
                _inCmdNameAndCmdParam = EditorGUILayout.TextField(TrLabel(nameof(_inCmdNameAndCmdParam)), _inCmdNameAndCmdParam);
                if (GUILayout.Button(UICommonOption.Run, UICommonOption.Width80, UICommonOption.Height20))
                {
                    mb.InvokeInteractByCmdNameAndCmdParam(_inCmdNameAndCmdParam);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                _inCmdAndCmdParam = EditorGUILayout.TextField(TrLabel(nameof(_inCmdAndCmdParam)), _inCmdAndCmdParam);
                if (GUILayout.Button(UICommonOption.Run, UICommonOption.Width80, UICommonOption.Height20))
                {
                    mb.InvokeInteractByCmdAndCmdParam(_inCmdAndCmdParam);
                }
                EditorGUILayout.EndHorizontal();
            }
            CommonFun.EndLayout();
        }

        #endregion

        #region 用途

        /// <summary>
        /// 显示用途列表
        /// </summary>
        [Name("显示用途列表")]
        [Tip("用于记录用途宿主被其它对象以指定用途占用的情况", "Used to record the situation where the host is occupied by other objects for the specified purpose")]
        public bool _displayUsages = false;

        private void DrawUsage()
        {
            // 折叠
            _displayUsages = UICommonFun.Foldout(_displayUsages, TrLabel(nameof(_displayUsages)));
            if (!_displayUsages) return;

            CommonFun.BeginLayout();
            {
                // 标题
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    GUILayout.Label(Tr("Key Word"), UICommonOption.Width200);
                    GUILayout.Label(Tr("User"));
                }
                EditorGUILayout.EndHorizontal();

                // 列表
                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));
                {
                    foreach (var item in targetObject.usage.usageMap)
                    {
                        EditorGUILayout.BeginHorizontal();

                        // 关键字
                        EditorGUILayout.LabelField(item.Key, UICommonOption.Width200);

                        // 对象列表
                        if (item.Value.userCount == 0)
                        {
                            EditorGUILayout.LabelField("");
                        }
                        else
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                foreach (var user in item.Value.users)
                                {
                                    EditorGUILayout.ObjectField(user, user ? user.GetType() : typeof(ExtensionalInteractObject), true);
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            CommonFun.EndLayout();
        }

        #endregion
    }

    /// <summary>
    /// 基础交互属性数据绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(BaseInteractPropertyData), true)]
    public class BaseInteractPropertyDataDrawer : PropertyDrawerAsArrayElement<BaseInteractPropertyDataDrawer.Data>
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public class Data : ArrayElementData
        {
            #region 序列化属性

            /// <summary>
            /// 关键字序列化属性
            /// </summary>
            public SerializedProperty keySP;

            /// <summary>
            /// 关键字值类型序列化属性
            /// </summary>
            public SerializedProperty keyValueTypeSP;

            /// <summary>
            /// 关键字值值序列化属性
            /// </summary>
            public SerializedProperty keyValueValueSP;

            /// <summary>
            /// 值序列化属性
            /// </summary>
            public SerializedProperty valueSP;

            #endregion

            /// <summary>
            /// 显示
            /// </summary>
            public bool display = true;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                keySP = property.FindPropertyRelative(nameof(InteractPropertyData._key));
                valueSP = property.FindPropertyRelative(nameof(InteractPropertyData._value));

                keyValueTypeSP = keySP.FindPropertyRelative(nameof(StringPropertyValue._propertyValueType));
                keyValueValueSP = keySP.FindPropertyRelative(nameof(StringPropertyValue._value));
            }
        }

        /// <summary>
        /// 获取对象绘制高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (base.GetPropertyHeight(property, label) + 2) * (cache.GetData(property).display ? 3 : 1);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);
            label = data.isArrayElement ? data.indexContent : label;

            // 标题
            var rect = new Rect(position.x, position.y, position.width, 18);
            GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
            data.display = GUI.Toggle(rect, data.display, label, EditorStyles.foldout);
            if (!data.display) return;

            // 匹配规则
            rect.xMin += 18;

            if (data.keyValueTypeSP.intValue == 0)// 值类型
            {
                var tmp = rect;
                tmp.width -= 100;
                tmp = PropertyDrawerHelper.DrawProperty(tmp, data.keySP);

                tmp.x += tmp.width;
                tmp.width = 100;
                EditorGUI.BeginChangeCheck();
                var value = UICommonFun.Popup(tmp, data.keyValueValueSP.stringValue, PropertyKeyCache.instance.GetKeys());
                if (EditorGUI.EndChangeCheck())
                {
                    var array = value.Split('/');
                    if (array!=null && array.Length>0)
                    {
                        data.keyValueValueSP.stringValue = array[array.Length - 1];
                    }
                }

                rect.y += PropertyDrawerHelper.singleLineHeight;
            }
            else
            {
                rect = PropertyDrawerHelper.DrawProperty(rect, data.keySP, "");
            }
            rect = PropertyDrawerHelper.DrawProperty(rect, data.valueSP, "");

        }
    }
}

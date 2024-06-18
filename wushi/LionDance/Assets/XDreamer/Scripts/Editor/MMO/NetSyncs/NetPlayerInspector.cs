using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.Scripts;

namespace XCSJ.EditorMMO.NetSyncs
{
    /// <summary>
    /// 网络玩家检查器
    /// </summary>
    [Name("网络玩家检查器")]
    [CustomEditor(typeof(NetPlayer), true)]
    public class NetPlayerInspector : NetPropertyInspector<NetPlayer>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(NetProperty._propertys):
                    {
                        EditorGUI.BeginChangeCheck();
                        var nickName = EditorGUILayout.DelayedTextField(CommonFun.NameTip(mb.GetType(), nameof(NetPlayer.nickname)), mb.nickname);
                        if (EditorGUI.EndChangeCheck())
                        {
                            mb.nickname = nickName;
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLocalProperties();
        }

        /// <summary>
        /// 显示本地属性列表
        /// </summary>
        [Name("显示本地属性列表")]
        public bool displayLocalProperties = true;

        /// <summary>
        /// 绘制本地属性列表
        /// </summary>
        private void DrawLocalProperties()
        {
            displayLocalProperties = UICommonFun.Foldout(displayLocalProperties, TrLabel(nameof(displayLocalProperties)));
            if (!displayLocalProperties) return;

            CommonFun.BeginLayout();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.LabelField("NO.", UICommonOption.Width20);
            EditorGUILayout.LabelField("名称", UICommonOption.Width120);
            EditorGUILayout.LabelField("值");
            EditorGUILayout.EndHorizontal();

            if (mb.playerInfo?.localProperties is Dictionary<string, object> dic)
            {
                var i = 0;
                foreach(var kv in dic)
                {
                    UICommonFun.BeginHorizontal(i);

                    EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width20);

                    EditorGUILayout.TextField(kv.Key, UICommonOption.Width120);

                    EditorGUILayout.TextField(kv.Value.ToScriptParamString());

                    UICommonFun.EndHorizontal();

                    i++;
                }
            }

            CommonFun.EndLayout();
        }
    }
}

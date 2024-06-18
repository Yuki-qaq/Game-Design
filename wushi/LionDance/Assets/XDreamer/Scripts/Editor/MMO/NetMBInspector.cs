using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;
using XCSJ.Scripts;
using XCSJ.Tools;

namespace XCSJ.EditorMMO
{
    /// <summary>
    /// 网络MB检查器
    /// </summary>
    [Name("网络MB检查器")]
    [CustomEditor(typeof(NetMB), true)]
    public class NetMBInspector : NetMBInspector<NetMB> { }

    /// <summary>
    /// 网络MB检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetMBInspector<T> : MMOMBInspector<T> where T : NetMB
    {
        private MMOOption option = null;
        private List<string> allSyncVars = null;

        private GUIStyle _syncVarGUIStyle;
        private GUIStyle syncVarGUIStyle
        {
            get
            {
                if (_syncVarGUIStyle == null)
                {
                    _syncVarGUIStyle = new GUIStyle(GUI.skin.box);
                    _syncVarGUIStyle.normal.background = Texture2DHelper.GetTexture2D(option.syncVarHighlightColor);
                }
                return _syncVarGUIStyle;
            }
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!targetObject) return;

            option = MMOOption.weakInstance;
            allSyncVars = mb.GetAllSyncVarNames();
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// 当选项修改
        /// </summary>
        /// <param name="option"></param>
        protected override void OnOptionModify(Option option)
        {
            base.OnOptionModify(option);
            if(option is MMOOption mMOOption)
            {
                this.option = mMOOption;
                _syncVarGUIStyle = null;
                Repaint();
            }
        }

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [LanguageTuple("Mark Dirty", "标记脏")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            if (IsSyncVar(serializedProperty) && option.syncVarHighlight)
            {
                EditorGUILayout.BeginVertical(syncVarGUIStyle);
                base.OnDrawMember(serializedProperty, propertyData);
                EditorGUILayout.EndVertical();
                return;
            }
            switch(serializedProperty.name)
            {
                case nameof(NetMB._dirty):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (GUILayout.Button(Tr("Mark Dirty"), UICommonOption.Width80))
                        {
                            mb.MarkDirty();
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private bool IsSyncVar(SerializedProperty memberProperty) => allSyncVars.Contains(memberProperty.name);

        /// <summary>
        /// 开始同步变量
        /// </summary>
        protected void BeginSyncVar()
        {
            if (option.syncVarHighlight) EditorGUILayout.BeginVertical(syncVarGUIStyle);
        }

        /// <summary>
        /// 结束同步变量
        /// </summary>
        protected void EndSyncVar()
        {
            if (option.syncVarHighlight) EditorGUILayout.EndVertical();
        }
    }
}

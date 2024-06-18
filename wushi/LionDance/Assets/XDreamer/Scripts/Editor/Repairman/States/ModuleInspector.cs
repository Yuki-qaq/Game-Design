using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.EditorRepairman.States
{
    /// <summary>
    /// 模块检查器
    /// </summary>
    [Name("模块检查器")]
    [CustomEditor(typeof(Module), true)]
    public class ModuleInspector : PartInspector
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (module)
            {
                interactModule = module.interactPart as PluginRepairman.Tools.Module;
                if (module.interactPart) { }
            }
        }

        private Module module => target as Module;
        private PluginRepairman.Tools.Module interactModule;

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Add the selected game object as [part]", "添加选中游戏对象为[零件]")]
        [LanguageTuple("Add the selected game object as [module]", "添加选中游戏对象为[模块]")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical(XGUIStyleLib.Get(EGUIStyle.Box));
            TreeView.Draw(module, TreeView.DefaultDrawExpandedFunc, TreeView.DefaultPrefixFunc, (node, content) =>
            {
                var part = node as Part;

                if (GUILayout.Button(content, GUI.skin.label))
                {
                    node.OnClick();

                    Selection.activeObject = part;
                }
            });
            EditorGUILayout.EndVertical();

            if (GUILayout.Button(new GUIContent("创建子级对象为[零件]", EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                if (module && module.gameObject)
                {
                    CreateGameObjectToComponent<Part, XCSJ.PluginRepairman.Tools.Part>(CommonFun.GetChildGameObjects(module.gameObject.transform), () => Part.CreatePart(module.parent));
                }
            }

            EditorGUI.BeginDisabledGroup(!Selection.activeGameObject);
            if (GUILayout.Button(new GUIContent("添加选中游戏对象为[零件]", EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                CreateGameObjectToComponent<Part, XCSJ.PluginRepairman.Tools.Part>(Selection.gameObjects, () => Part.CreatePart(module.parent));
            }

            // 批量添加选中游戏对象为零件
            if (GUILayout.Button(new GUIContent("添加选中游戏对象为[模块]", EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                CreateGameObjectToComponent<Module, XCSJ.PluginRepairman.Tools.Module>(Selection.gameObjects, () => Module.CreateModule(module.parent));
            }
            EditorGUI.EndDisabledGroup();
        }

        private void CreateGameObjectToComponent<TStatePart, TInteractPart>(IEnumerable<GameObject> gameObjects, Func<State> createFun) 
            where TStatePart : Part
            where TInteractPart : XCSJ.PluginRepairman.Tools.Part
        {
            gameObjects.Foreach(go =>
            {
                if (go)
                {
                    // 绑定交互零件组件
                    go.XGetOrAddComponent<TInteractPart>();

                    var state = createFun.Invoke();
                    if (state)
                    {
                        var part = state.GetComponent<TStatePart>();
                        if (part)
                        {
                            part.go = go;
                            state.XSetName(go.name);
                        }
                    }
                }
            });
        }
    }
}

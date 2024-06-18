using UnityEngine;
using System.Collections;
using XCSJ.EditorCommonUtils;
using UnityEditor;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Menus;
using XCSJ.Attributes;
using XCSJ.PluginExtensionExample.Demos;

namespace XCSJ.EditorExtensionExample.Demos
{
    /// <summary>
    /// 菜单特性
    /// </summary>
    public class XMenuAttribute : BaseMenuAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="itemName"></param>
        public XMenuAttribute(string itemName) : base(itemName) { }
    }

    /// <summary>
    /// 菜单案例检查器
    /// </summary>
    [Name("菜单案例检查器")]
    [CustomEditor(typeof(MenuDemo))]
    public class MenuDemoInspector : MBInspector<MenuDemo>
    {
        /// <summary>
        /// 功能A
        /// </summary>
        [XMenu("A")]
        [Menu("M1", "A")]
        public static void FunA()
        {
            Debug.Log("FunA()");
        }

        /// <summary>
        /// 有效功能A
        /// </summary>
        /// <returns></returns>
        [XMenu("A", isValid = true)]
        [Menu("M1", "A", isValid = true)]
        public static bool ValidFunA()
        {
            Debug.Log("ValidFunA()");
            return true;
        }

        /// <summary>
        /// 功能B
        /// </summary>
        [XMenu("B")]        
        public static void FunB()
        {
            Debug.Log("FunB()");
        }

        /// <summary>
        /// 有效功能B
        /// </summary>
        /// <returns></returns>
        [XMenu("B", isValid = true)]
        public static bool ValidFunB()
        {
            Debug.Log("ValidFunB()");
            return false;
        }

        /// <summary>
        /// 功能C
        /// </summary>
        [XMenu("C/C1", separatorType = ESeparatorType.TopUp)]
        [XMenu("C/C2", separatorType = ESeparatorType.TopDown)]
        public static void FunC()
        {
            Debug.Log("FunC()");
        }

        /// <summary>
        /// 功能D
        /// </summary>
        /// <param name="obj"></param>
        [XMenu("D", userData = "D of XMenu")]
        [XMenu("D/D1", userData = "D1 of XMenu")]
        [XMenu("D/D2", userData = "D2 of XMenu")]
        [Menu("M2", "D", userData = "D of M2 Menu")]
        [Menu("M2", "D/D1", userData = "D1 of M2 Menu")]
        [Menu("M2", "D/D2", userData = "D2 of M2 Menu")]
        public static void FunD(object obj)
        {
            Debug.Log("FunD(): " + obj);
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("XMenuAttribute"))
            {
                MenuHelper.DrawMenu<XMenuAttribute>(m =>
                {
                    m.AddMenuItem("T1", () => Debug.Log("T1 of XMenu"));
                    m.AddMenuItem("T2", () => Debug.Log("T2 of XMenu"));
                });
            }

            if (GUILayout.Button("M1 of MenuAttribute"))
            {
                MenuHelper.DrawMenu("M1", m =>
                {
                    m.AddMenuItem("T1", () => Debug.Log("T1 of M1 Menu"));
                    m.AddMenuItem("T2", () => Debug.Log("T2 of M1 Menu"));
                });
            }

            if (GUILayout.Button("M2 of MenuAttribute"))
            {
                MenuHelper.DrawMenu("M2", m =>
                {
                    m.AddMenuItem("T1", () => Debug.Log("T1 of M2 Menu"));
                    m.AddMenuItem("T2", () => Debug.Log("T2 of M2 Menu"));
                });
            }
        }
    }
}

using System;
using System.Reflection;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 自定义编辑器特性集关联类型
    /// </summary>
    [LinkType(EditorHelper.UnityEditorPrefix + "CustomEditorAttributes")]
    public class CustomEditorAttributes_LinkType : LinkType<CustomEditorAttributes_LinkType>
    {
        /// <summary>
        /// 查找自定义编辑器类型
        /// </summary>
        /// <param name="o"></param>
        /// <param name="multiEdit"></param>
        /// <returns></returns>
        public static Type FindCustomEditorType(UnityEngine.Object o, bool multiEdit) =>    FindCustomEditorTypeByType(o.GetType(), multiEdit);

        /// <summary>
        /// 通过类型查找自定义编辑器类型方法信息
        /// </summary>
        public static XMethodInfo FindCustomEditorTypeByType_MethodInfo { get; } = new XMethodInfo(Type, nameof(FindCustomEditorTypeByType), BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// 通过类型查找自定义编辑器类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="multiEdit"></param>
        /// <returns></returns>
        public static Type FindCustomEditorTypeByType(Type type, bool multiEdit)
        {
            return FindCustomEditorTypeByType_MethodInfo?.Invoke(null, new object[] { type, multiEdit }) as Type;
        }
    }
}

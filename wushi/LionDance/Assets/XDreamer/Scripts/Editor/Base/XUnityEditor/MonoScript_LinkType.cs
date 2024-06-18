using System;
using UnityEditor;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// Mono脚本关联类型
    /// </summary>
    [LinkType(typeof(MonoScript))]
    public class MonoScript_LinkType : LinkType<MonoScript_LinkType>
    {
        /// <summary>
        /// Ping对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MonoScript PingObject(Type type)
        {
            var monoScript = MonoImporter_LinkType.GetMonoScript(type);
            if (monoScript == null) return null;
            EditorGUIUtility.PingObject(monoScript);
            return monoScript;
        }
    }
}

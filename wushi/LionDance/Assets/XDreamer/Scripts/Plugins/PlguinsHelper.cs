using UnityEngine;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Kernel;
using XCSJ.PluginCNScripts;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Products;
using XCSJ.PluginGenericStandards.Managers;
using XCSJ.PluginGenericStandards;
using XCSJ.LitJson;
using System;
using System.Reflection;

namespace XCSJ.Extension
{
    /// <summary>
    /// 插件组手类
    /// </summary>
    public static class PlguinsHelper
    {
        /// <summary>
        /// UnityEngine前缀
        /// </summary>
        public const string UnityEngine_Prefix = nameof(UnityEngine) + ".";

        /// <summary>
        /// UnityEngineInternal前缀
        /// </summary>
        public const string UnityEngineInternal_Prefix = nameof(UnityEngineInternal) + ".";

        /// <summary>
        /// UnityEngine.EventSystems前缀
        /// </summary>
        public const string UnityEngine_EventSystems_Prefix = UnityEngine_Prefix + nameof(UnityEngine.EventSystems) + ".";

        /// <summary>
        /// UnityEngine.UI前缀
        /// </summary>
        public const string UnityEngine_UI_Prefix = UnityEngine_Prefix + nameof(UnityEngine.UI) + ".";

        private static bool initialized = false;

        /// <summary>
        /// 初始化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            ConverterExtension.Init();
            PluginsHandlerExtension.Init();
            HelperInit();

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            XDreamer.getLocalDogServerPath += GetLocalDogServerPath;
#endif
        }

        private static void HelperInit()
        {
            CNScriptHelper.Init();
        }

        /// <summary>
        /// 获取获取本地加密狗服务路径：仅在Windows平台可用，通过注册表方式查找当前计算机上已安装的XDreamer认证服务；
        /// </summary>
        /// <returns></returns>
        public static LocalDogServerPath GetLocalDogServerPath()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

            //当前工程对应的设置中获取
            var exePath = PlayerPrefs.GetString(ProductServer.AuthenticationServer.LastLocalDogServerPath, "");
            if (FileHelper.Exists(exePath))
            {
                return new LocalDogServerPath(exePath, 1);
            }

#if UNITY_EDITOR

            //在Unity编辑器内时，从软件官方的公司产品配置中获取
            var companyName = UnityEditor.PlayerSettings.companyName;
            var productName = UnityEditor.PlayerSettings.productName;
            try
            {
                UnityEditor.PlayerSettings.companyName = nameof(XCSJ);
                UnityEditor.PlayerSettings.productName = ProductServer.Name;

                exePath = PlayerPrefs.GetString(ProductServer.AuthenticationServer.LastLocalDogServerPath, "");
                if (FileHelper.Exists(exePath))
                {
                    return new LocalDogServerPath(exePath, 1);
                }
            }
            finally
            {
                UnityEditor.PlayerSettings.companyName = companyName;
                UnityEditor.PlayerSettings.productName = productName;
            }
#endif
            return new LocalDogServerPath("", 1);
#else
            return default;
#endif

        }

        /// <summary>
        /// 尝试转换为层级JSON数据：主要用于处理JSON类型为对象（即字典型）且其键字符串中有点分隔符时，依据点前后不同字符串转为对应的层级JSON数据对象；仅处理键字符串中带一个点的情况,其他情况时保留原信息；
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static JsonData TryConvertToHierarchyJsonData(this JsonData jsonData)
        {
            if (jsonData == null) return new JsonData();
            switch (jsonData.GetJsonType())
            {
                case JsonType.Object:
                    {
                        var result = new JsonData();
                        result.SetJsonType(JsonType.Object);
                        foreach (var kv in jsonData.objectValue)
                        {
                            var a = kv.Key.GetSplitArray(".", StringSplitOptions.RemoveEmptyEntries);
                            switch (a.Length)
                            {
                                case 2:
                                    {
                                        if (!result.objectValue.TryGetValue(a[0], out var value))
                                        {
                                            value = new JsonData();
                                            value.SetJsonType(JsonType.Object);
                                            result[a[0]] = value;
                                        }
                                        value[a[1]] = TryConvertToHierarchyJsonData(kv.Value);
                                        break;
                                    }
                                default:
                                    {
                                        result[kv.Key] = TryConvertToHierarchyJsonData(kv.Value);
                                        break;
                                    }
                            }
                        }
                        return result;
                    }
                case JsonType.Array:
                    {
                        var result = new JsonData();
                        result.SetJsonType(JsonType.Array);
                        foreach (var i in jsonData.arrayValue)
                        {
                            result.Add(TryConvertToHierarchyJsonData(i));
                        }
                        return result;
                    }
                default:
                    {
                        return jsonData.ToString();
                    }
            }
        }

        /// <summary>
        /// 生成泛型类型
        /// </summary>
        /// <param name="originFieldInfo"></param>
        /// <param name="originGenericType"></param>
        /// <param name="typeArguments"></param>
        /// <returns></returns>
        public static Type MakeGenericType(FieldInfo originFieldInfo, Type originGenericType, params Type[] typeArguments)
        {
            if (originGenericType == null) return default;
            if (funcInEditor != null && funcInEditor.TryMakeGenericType(out var makedType, originFieldInfo, originGenericType, typeArguments) && makedType != null)
            {
                return makedType;
            }
            return originGenericType.MakeGenericType(typeArguments);
        }

        /// <summary>
        /// 编辑器内函数
        /// </summary>
        public static IFuncInEditor funcInEditor { get; set; }
    }

    /// <summary>
    /// 编辑器内函数
    /// </summary>
    public interface IFuncInEditor
    {
        /// <summary>
        /// 尝试生成泛型类型
        /// </summary>
        /// <param name="makedType"></param>
        /// <param name="originFieldInfo"></param>
        /// <param name="originGenericType"></param>
        /// <param name="typeArguments"></param>
        /// <returns></returns>
        bool TryMakeGenericType(out Type makedType, FieldInfo originFieldInfo, Type originGenericType, params Type[] typeArguments);
    }
}

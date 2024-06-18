using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// 编辑器对象辅助类
    /// </summary>
    public class EditorObjectHelper
    {
        internal static XGUIStyle MiniPopup { get; } = new XGUIStyle(nameof(MiniPopup));

        #region 组件集弹出式菜单-游戏对象、状态、跳转、状态组

        private static void DrawComponentCollectionMenu(UnityEngine.Object obj, Action<UnityEngine.Object> onMenuItemClicked)
        {
            var selectText = ComponentCollectionCache.Get(obj);
            var values = ComponentCollectionCache.Get();
            
            EditorHelper.DrawMenu(selectText, values, newSelectText =>
            {
                onMenuItemClicked?.Invoke(ComponentCollectionCache.Get(newSelectText));
            });
        }

        /// <summary>
        /// 弹出组件集选择菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="obj"></param>
        /// <param name="onMenuItemClicked"></param>
        private static void ComponentCollectionMenu(Rect position, UnityEngine.Object obj, Action<UnityEngine.Object> onMenuItemClicked)
        {
            var text = obj ? obj.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), MiniPopup))
            {
                DrawComponentCollectionMenu(obj, onMenuItemClicked);
            }
        }

        /// <summary>
        /// 弹出组件集选择菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static UnityEngine.Object ComponentCollectionPopup(Rect position, UnityEngine.Object obj)
        {
            var selectText = ComponentCollectionCache.Get(obj);
            var values = ComponentCollectionCache.Get();
            var index = values.IndexOf(selectText);
            var newIndex = EditorGUI.Popup(position, index, values);
            if (newIndex != index)
            {
                return ComponentCollectionCache.Get(values[newIndex]);
            }
            return obj;
        }

        /// <summary>
        /// 弹出组件集选择菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        public static void ComponentCollectionPopup(Rect position, SerializedProperty property)
        {
            //property.objectReferenceValue = ComponentCollectionPopup(position, property.objectReferenceValue);
            var propertyTmp = property.Copy();
            ComponentCollectionMenu(position, propertyTmp.objectReferenceValue, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            });
        }

        private static void ComponentCollectionMenu(UnityEngine.Object obj, Action<UnityEngine.Object> onMenuItemClicked, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(obj ? obj.name : "", options))
            {
                DrawComponentCollectionMenu(obj, onMenuItemClicked);
            }
        }

        /// <summary>
        /// 弹出组件集选择菜单
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static UnityEngine.Object ComponentCollectionPopup(UnityEngine.Object obj, params GUILayoutOption[] options)
        {
            var selectText = ComponentCollectionCache.Get(obj);
            var newText = UICommonFun.Popup(selectText, ComponentCollectionCache.Get(), false, options);
            if (selectText != newText)
            {
                return ComponentCollectionCache.Get(newText);
            }
            return obj;
        }

        /// <summary>
        /// 弹出组件集选择菜单
        /// </summary>
        /// <param name="property"></param>
        /// <param name="options"></param>
        public static void ComponentCollectionPopup(SerializedProperty property, params GUILayoutOption[] options)
        {
            //property.objectReferenceValue = ComponentCollectionPopup(property.objectReferenceValue, options);
            var propertyTmp = property.Copy();
            ComponentCollectionMenu(propertyTmp.objectReferenceValue, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            }, options);
        }

        /// <summary>
        /// 组件集缓存类
        /// </summary>
        internal class ComponentCollectionCache
        {
            private static string[] namePaths = null;

            public static Dictionary<string, UnityEngine.Object> objects { get; private set; } = new Dictionary<string, UnityEngine.Object>();
            public static Dictionary<UnityEngine.Object, string> paths { get; private set; } = new Dictionary<UnityEngine.Object, string>();

            public static string[] Get()
            {
                if (namePaths == null)
                {
                    Init();
                }
                return namePaths;
            }

            public static string Get(UnityEngine.Object obj) => (obj && paths.TryGetValue(obj, out string path)) ? path : "";

            public static UnityEngine.Object Get(string path) => (!string.IsNullOrEmpty(path) && objects.TryGetValue(path, out UnityEngine.Object obj)) ? obj : null;

            public const string GameObjectName = "游戏对象";
            public const string StateName = "状态";
            public const string TransitionName = "跳转";
            public const string StateGroupName = "状态组";

            /// <summary>
            /// 初始化组件集缓存
            /// </summary>
            public static void Init()
            {
                var paths = new List<string>();

                //游戏对象
                {
                    var components = ComponentCache.Get(typeof(Transform), true)?.components ?? new Component[] { };
                    if (components != null)
                    {
                        foreach (var component in components)
                        {
                            if (component)
                            {
                                var gameObject = component.gameObject;
                                var path = GameObjectName + CommonFun.GameObjectToString(gameObject);
                                if (objects.ContainsKey(path))
                                {
                                    //Debug.LogErrorFormat("游戏对象同名冲突：" + path);
                                }
                                else
                                {
                                    paths.Add(path);
                                    objects.Add(path, gameObject);
                                    ComponentCollectionCache.paths.Add(gameObject, path);
                                }
                            }
                        }
                    }
                }

                //状态
                {
                    var states = SMSCache.GetStates(RootStateMachine.instance, true);
                    if (states != null)
                    {
                        foreach (var state in states)
                        {
                            if (state)
                            {
                                var path = StateName + SMSHelper.StateToString(state);
                                paths.Add(path);
                                objects.Add(path, state);
                                ComponentCollectionCache.paths.Add(state, path);
                            }
                        }
                    }
                }

                //跳转
                {
                    var transitions = SMSCache.GetTransitions(RootStateMachine.instance, true);
                    if (transitions != null)
                    {
                        foreach (var transition in transitions)
                        {
                            if (transition)
                            {
                                var path = TransitionName + SMSHelper.TransitionToString(transition);
                                paths.Add(path);
                                objects.Add(path, transition);
                                ComponentCollectionCache.paths.Add(transition, path);
                            }
                        }
                    }
                }

                //状态组
                {
                    var stateGroups = SMSCache.GetStateGroups(RootStateMachine.instance, true);
                    if (stateGroups != null)
                    {
                        foreach (var stateGroup in stateGroups)
                        {
                            if (stateGroup)
                            {
                                var path = StateGroupName + SMSHelper.StateGroupToSring(stateGroup);
                                paths.Add(path);
                                objects.Add(path, stateGroup);
                                ComponentCollectionCache.paths.Add(stateGroup, path);
                            }
                        }
                    }
                }

                namePaths = paths.ToArray();
            }

            /// <summary>
            /// 清空组件集缓存
            /// </summary>
            public static void Clear()
            {
                objects.Clear();
                paths.Clear();
                namePaths = null;
            }
        }

        #endregion

        #region 对象组件弹出式菜单-游戏对象组件、状态组件、跳转组件、状态组组件

        /// <summary>
        /// 弹出对象组件选择菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static UnityEngine.Object ObjectComponentPopup(Rect position, UnityEngine.Object obj)
        {
            if (TryGetObjectComponentPopupInfo(obj, out string selectText, out Dictionary<string, UnityEngine.Object> objects))
            {
                var values = objects.Keys.ToArray();
                var index = values.IndexOf(selectText);
                var newIndex = EditorGUI.Popup(position, index, values);
                if (newIndex != index && objects.TryGetValue(values[newIndex], out UnityEngine.Object newObject))
                {
                    return newObject;
                }
                return obj;
            }
            EditorGUI.Popup(position, -1, Empty<string>.Array);
            return obj;
        }

        /// <summary>
        /// 弹出对象组件选择菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        public static void ObjectComponentPopup(Rect position, SerializedProperty property)
        {
            property.objectReferenceValue = ObjectComponentPopup(position, property.objectReferenceValue);
        }

        /// <summary>
        /// 弹出对象组件选择菜单
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static UnityEngine.Object ObjectComponentPopup(UnityEngine.Object obj, params GUILayoutOption[] options)
        {
            if (TryGetObjectComponentPopupInfo(obj, out string selectText, out Dictionary<string, UnityEngine.Object> objects))
            {
                var newText = UICommonFun.Popup(selectText, objects.Keys.ToArray(), false, options);
                if (newText != selectText)
                {
                    return objects[newText];
                }
                return obj;
            }
            EditorGUILayout.Popup(-1, Empty<string>.Array, options);
            return obj;
        }

        /// <summary>
        /// 弹出对象组件选择菜单
        /// </summary>
        /// <param name="property"></param>
        /// <param name="options"></param>
        public static void ObjectComponentPopup(SerializedProperty property, params GUILayoutOption[] options)
        {
            property.objectReferenceValue = ObjectComponentPopup(property.objectReferenceValue, options);
        }

        /// <summary>
        /// 尝试获取对象组件弹出式菜单信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="selectText"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        private static bool TryGetObjectComponentPopupInfo(UnityEngine.Object obj, out string selectText, out Dictionary<string, UnityEngine.Object> objects)
        {
            if (!obj)
            {
                selectText = null;
                objects = null;
                return false;
            }

            selectText = "";
            var tmpObjects = new Dictionary<string, UnityEngine.Object>();

            var objCache = obj;
            if (obj is Component unityComponent)
            {
                obj = unityComponent.gameObject;
            }
            else if (obj is XCSJ.PluginCommonUtils.ComponentModel.Component modelComponent)
            {
                obj = modelComponent.componentCollection;
            }

            if (obj is GameObject gameObject)
            {
                var start = "游戏对象";
                tmpObjects.Add(start, gameObject);
                if (objCache == gameObject)
                {
                    selectText = start;
                }
                start = "组件/";

                var components = gameObject.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    var component = components[i];
                    if (component)
                    {
                        var key = start + (i + 1).ToString() + "." + component.GetType().Name;

                        tmpObjects.Add(key, component);

                        if (objCache == component)
                        {
                            selectText = key;
                        }
                    }
                }
            }
            else if (obj is XCSJ.PluginCommonUtils.ComponentModel.ComponentCollection componentCollection)
            {
                var start = componentCollection.name;//CommonFun.Name(componentCollection.GetType());//"组件集对象";
                tmpObjects.Add(start, componentCollection);
                if (objCache == componentCollection)
                {
                    selectText = start;
                }
                start = "组件/";

                var components = componentCollection.GetComponents<XCSJ.PluginCommonUtils.ComponentModel.Component>(true);
                foreach (var component in components)
                {
                    if (component)
                    {
                        var cType = component.GetType();
                        var key = start + component.name;

                        tmpObjects.Add(key, component);

                        if (objCache == component)
                        {
                            selectText = key;
                        }
                    }
                }
            }

            objects = tmpObjects;
            return true;
        }

        #endregion

        #region 游戏对象组件弹出式菜单

        private static void DrawGameObjectComponentMenu(Type componentType, Component component, ESearchFlags searchFlags, Action<Component> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            if (searchFlags.HasAnyFlag(ESearchFlags.OptimizeComponent))
            {
                var info = ComponentCache.Get(componentType, searchFlags, scenePath);
                if (info != null)
                {
                    var selectText = info.GetGameObjectNamePath(component ? component.gameObject : default);
                    var values = info.namePathOfGameObject;

                    EditorHelper.DrawMenu(selectText, values, newSelectText =>
                    {
                        var go = info.GetGameObject(newSelectText);
                        if (go)
                        {
                            onMenuItemClicked?.Invoke(go.GetComponent(componentType));
                        }
                    });
                }
            }
            else
            {
                var info = ComponentCache.Get(componentType, searchFlags, scenePath);
                if (info != null)
                {
                    var selectText = info.GetComponentNamePath(component);
                    var values = info.namePathOfComponent;

                    EditorHelper.DrawMenu(selectText, values, newSelectText =>
                    {
                        onMenuItemClicked?.Invoke(info.GetComponent(newSelectText));
                    });
                }
            }
        }

        private static void GameObjectComponentMenu(Rect position, Type componentType, Component component, ESearchFlags searchFlags, Action<Component> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = component ? component.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), MiniPopup))
            {
                DrawGameObjectComponentMenu(componentType, component, searchFlags, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 游戏对象组件弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="componentType"></param>
        /// <param name="searchFlags"></param>
        /// <param name="property"></param>
        public static void GameObjectComponentPopup(Rect position, Type componentType, ESearchFlags searchFlags, SerializedProperty property)
        {
            var propertyTmp = property.Copy();
            GameObjectComponentMenu(position, componentType, propertyTmp.objectReferenceValue as Component, searchFlags, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            }, propertyTmp.serializedObject.targetObject);
        }

        #endregion

        #region 游戏对象组件类型弹出式菜单

        private static string[] _componentTypes = null;

        /// <summary>
        /// 游戏对象组件类型字符串数组
        /// </summary>
        public static string[] componentTypes
        {
            get
            {
                if (_componentTypes == null)
                {
                    var types = TypeHelper.FindTypeInAppWithClass(typeof(Component), true, true, false).Cast(t => t.FullNameToHierarchyString()).ToList();
                    types.Sort();
                    _componentTypes = types.ToArray();
                }
                return _componentTypes;
            }
        }

        private static void DrawGameObjectComponentTypeMenu(string componentType, Action<string> onMenuItemClicked)
        {
            EditorHelper.DrawMenu(componentType, componentTypes, newSelectText =>
            {
                onMenuItemClicked?.Invoke(newSelectText);
            });
        }

        private static void GameObjectComponentTypeMenu(Rect position, string componentType, Action<string> onMenuItemClicked)
        {
            var text = componentType ?? "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), MiniPopup))
            {
                DrawGameObjectComponentTypeMenu(componentType, onMenuItemClicked);
            }
        }

        /// <summary>
        /// 游戏对象组件类型弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        public static void GameObjectComponentTypePopup(Rect position, SerializedProperty property)
        {
            var propertyTmp = property.Copy();
            GameObjectComponentTypeMenu(position, propertyTmp.stringValue, obj =>
            {
                propertyTmp.stringValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            });
        }

        #endregion

        #region 游戏对象弹出式菜单

        private static void DrawGameObjectMenu(Type componentType, GameObject gameObject, bool includeInactive, Action<GameObject> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var info = ComponentCache.Get(componentType, includeInactive, scenePath);
            if (info != null)
            {
                var selectText = info.GetGameObjectNamePath(gameObject);
                var values = info.namePathOfGameObject;

                EditorHelper.DrawMenu(selectText, values, newSelectText =>
                {
                    onMenuItemClicked?.Invoke(info.GetGameObject(newSelectText));
                });
            }
        }

        private static void GameObjectMenu(Rect position, Type componentType, GameObject gameObject, bool includeInactive, Action<GameObject> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = gameObject ? gameObject.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), MiniPopup))
            {
                DrawGameObjectMenu(componentType, gameObject, includeInactive, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 游戏对象弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="componentType"></param>
        /// <param name="includeInactive"></param>
        /// <param name="property"></param>
        public static void GameObjectPopup(Rect position, Type componentType, bool includeInactive, SerializedProperty property)
        {
            var propertyTmp = property.Copy();
            GameObjectMenu(position, componentType, propertyTmp.objectReferenceValue as GameObject, includeInactive, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            }, propertyTmp.serializedObject.targetObject);
        }

        #endregion
    }

    #region ZIP组手

    /// <summary>
    /// ZIP组手
    /// </summary>
    public static class ZipHelper
    {
        #region 压缩

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <param name="outputPathName">压缩后的输出路径文件名</param>
        /// <param name="directoryOrOnlyChildren">文件夹或仅子级：压缩整个文件夹或仅压缩文件夹内的所有子级（包括子级文件与文件夹）</param>
        /// <param name="password">压缩密码</param>
        /// <param name="onPreZip">压缩单个文件或文件夹前执行的回调</param>
        /// <param name="onPostZip">压缩单个文件或文件夹后执行的回调</param>
        /// <param name="onFinished">压缩执行完毕后的回调</param>
        /// <param name="level">级别：压缩质量和压缩速度的平衡点</param>
        /// <returns></returns>
        public static bool ZipFolder(string directoryPath, string outputPathName, bool directoryOrOnlyChildren, string password = null, Func<ZipEntry, bool> onPreZip = null, Action<ZipEntry> onPostZip = null, Action<bool> onFinished = null, int level = 6)
        {
            if (directoryOrOnlyChildren)
            {
                return Zip(directoryPath, outputPathName, password, onPreZip, onPostZip, onFinished, level);
            }
            if (Directory.Exists(directoryPath))
            {
                return Zip(Directory.GetDirectories(directoryPath).Concat(Directory.GetFiles(directoryPath)).ToArray(), outputPathName, password, onPreZip, onPostZip, onFinished, level);
            }
            return false;
        }

        /// <summary>
        /// 压缩:压缩文件和文件夹
        /// </summary>
        /// <param name="fileOrDirectory">文件或文件夹路径</param>
        /// <param name="outputPathName">压缩后的输出路径文件名</param>
        /// <param name="password">压缩密码</param>
        /// <param name="onPreZip">压缩单个文件或文件夹前执行的回调</param>
        /// <param name="onPostZip">压缩单个文件或文件夹后执行的回调</param>
        /// <param name="onFinished">压缩执行完毕后的回调</param>
        /// <param name="level">级别：压缩质量和压缩速度的平衡点</param>
        /// <returns></returns>
        public static bool Zip(string fileOrDirectory, string outputPathName, string password = null, Func<ZipEntry, bool> onPreZip = null, Action<ZipEntry> onPostZip = null, Action<bool> onFinished = null, int level = 6)
        {
            return Zip(new string[] { fileOrDirectory }, outputPathName, password, onPreZip, onPostZip, onFinished, level);
        }

        /// <summary>
        /// 压缩:压缩文件和文件夹
        /// </summary>
        /// <param name="fileOrDirectoryArray">文件或文件夹路径数组</param>
        /// <param name="outputPathName">压缩后的输出路径文件名</param>
        /// <param name="password">压缩密码</param>
        /// <param name="onPreZip">压缩单个文件或文件夹前执行的回调</param>
        /// <param name="onPostZip">压缩单个文件或文件夹后执行的回调</param>
        /// <param name="onFinished">压缩执行完毕后的回调</param>
        /// <param name="level">级别：压缩质量和压缩速度的平衡点</param>
        /// <returns></returns>
        public static bool Zip(string[] fileOrDirectoryArray, string outputPathName, string password = null, Func<ZipEntry, bool> onPreZip = null, Action<ZipEntry> onPostZip = null, Action<bool> onFinished = null, int level = 6)
        {
            if ((null == fileOrDirectoryArray) || string.IsNullOrEmpty(outputPathName))
            {
                onFinished?.Invoke(false);
                return false;
            }

            ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(outputPathName));
            zipOutputStream.SetLevel(level);    // 压缩质量和压缩速度的平衡点
            if (!string.IsNullOrEmpty(password))
                zipOutputStream.Password = password;

            for (int index = 0; index < fileOrDirectoryArray.Length; ++index)
            {
                bool result = false;
                string fileOrDirectory = fileOrDirectoryArray[index];
                if (Directory.Exists(fileOrDirectory))
                    result = ZipDirectory(fileOrDirectory, string.Empty, zipOutputStream, onPreZip, onPostZip, onFinished);
                else if (File.Exists(fileOrDirectory))
                    result = ZipFile(fileOrDirectory, string.Empty, zipOutputStream, onPreZip, onPostZip, onFinished);

                if (!result)
                {
                    onFinished?.Invoke(false);
                    return false;
                }
            }

            zipOutputStream.Finish();
            zipOutputStream.Close();

            onFinished?.Invoke(true);

            return true;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="filePathName">文件路径名</param>
        /// <param name="parentRelPath">要压缩的文件的父相对文件夹</param>
        /// <param name="zipOutputStream">压缩输出流</param>
        /// <param name="onPreZip">压缩单个文件或文件夹前执行的回调</param>
        /// <param name="onPostZip">压缩单个文件或文件夹后执行的回调</param>
        /// <param name="onFinished">压缩执行完毕后的回调</param>
        /// <returns></returns>
        private static bool ZipFile(string filePathName, string parentRelPath, ZipOutputStream zipOutputStream, Func<ZipEntry, bool> onPreZip = null, Action<ZipEntry> onPostZip = null, Action<bool> onFinished = null)
        {
            //Crc32 crc32 = new Crc32();
            ZipEntry entry = null;
            FileStream fileStream = null;
            try
            {
                string entryName = string.IsNullOrEmpty(parentRelPath) ? Path.GetFileName(filePathName) : (parentRelPath + '/' + Path.GetFileName(filePathName));
                entry = new ZipEntry(entryName);
                entry.DateTime = System.DateTime.Now;

                //过滤
                if ((null != onPreZip) && !onPreZip(entry)) return true;

                fileStream = File.OpenRead(filePathName);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                fileStream.Close();

                entry.Size = buffer.Length;

                //crc32.Reset();
                //crc32.Update(buffer);
                //entry.Crc = crc32.Value;

                zipOutputStream.PutNextEntry(entry);
                zipOutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (System.Exception ex)
            {
                ex.HandleException(nameof(ZipFile));
                return false;
            }
            finally
            {
                if (null != fileStream)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
            onPostZip?.Invoke(entry);

            return true;
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="directoryPath">要压缩的文件夹</param>
        /// <param name="parentRelPath">要压缩的文件夹的父相对文件夹</param>
        /// <param name="zipOutputStream">压缩输出流</param>
        /// <param name="onPreZip">压缩单个文件或文件夹前执行的回调</param>
        /// <param name="onPostZip">压缩单个文件或文件夹后执行的回调</param>
        /// <param name="onFinished">压缩执行完毕后的回调</param>
        /// <returns></returns>
        private static bool ZipDirectory(string directoryPath, string parentRelPath, ZipOutputStream zipOutputStream, Func<ZipEntry, bool> onPreZip = null, Action<ZipEntry> onPostZip = null, Action<bool> onFinished = null)
        {
            ZipEntry entry = null;
            try
            {
                string entryName = Path.Combine(parentRelPath, Path.GetFileName(directoryPath) + '/');
                entry = new ZipEntry(entryName);
                entry.DateTime = System.DateTime.Now;
                entry.Size = 0;

                // 过滤
                if ((null != onPreZip) && !onPreZip(entry)) return true;

                zipOutputStream.PutNextEntry(entry);
                zipOutputStream.Flush();

                string[] files = Directory.GetFiles(directoryPath);
                for (int index = 0; index < files.Length; ++index)
                {
                    // 排除Unity中可能的 .meta 文件
                    if (files[index].EndsWith(".meta") == true)
                    {
                        Debug.LogWarning(files[index] + " not to zip");
                        continue;
                    }

                    ZipFile(files[index], Path.Combine(parentRelPath, Path.GetFileName(directoryPath)), zipOutputStream, onPreZip, onPostZip, onFinished);
                }
            }
            catch (System.Exception ex)
            {
                ex.HandleException(nameof(ZipDirectory));
                return false;
            }

            string[] directories = Directory.GetDirectories(directoryPath);
            for (int index = 0; index < directories.Length; ++index)
            {
                if (!ZipDirectory(directories[index], Path.Combine(parentRelPath, Path.GetFileName(directoryPath)), zipOutputStream, onPreZip, onPostZip, onFinished))
                {
                    return false;
                }
            }
            onPostZip?.Invoke(entry);
            return true;
        }

        #endregion

        #region 解压缩

        /// <summary>
        /// 解压Zip包
        /// </summary>
        /// <param name="filePathName">Zip包的文件路径名</param>
        /// <param name="outputPath">解压输出路径</param>
        /// <param name="password">解压密码</param>
        /// <param name="onPreUnzip">解压单个文件或文件夹前执行的回调</param>
        /// <param name="onPostUnzip">解压单个文件或文件夹后执行的回调</param>
        /// <param name="onFinished">解压执行完毕后的回调</param>
        /// <returns></returns>
        public static bool UnzipFile(string filePathName, string outputPath, string password = null, Func<ZipEntry, bool> onPreUnzip = null, Action<ZipEntry> onPostUnzip = null, Action<bool> onFinished = null)
        {
            if (string.IsNullOrEmpty(filePathName) || string.IsNullOrEmpty(outputPath))
            {
                onFinished?.Invoke(false);
                return false;
            }

            try
            {
                return UnzipFile(File.OpenRead(filePathName), outputPath, password, onPreUnzip, onPostUnzip, onFinished);
            }
            catch (System.Exception ex)
            {
                ex.HandleException(nameof(UnzipFile));
                onFinished?.Invoke(false);
                return false;
            }
        }

        /// <summary>
        /// 解压Zip包
        /// </summary>
        /// <param name="fileBytes">Zip包字节数组</param>
        /// <param name="outputPath">解压输出路径</param>
        /// <param name="password">解压密码</param>
        /// <param name="onPreUnzip">解压单个文件或文件夹前执行的回调</param>
        /// <param name="onPostUnzip">解压单个文件或文件夹后执行的回调</param>
        /// <param name="onFinished">解压执行完毕后的回调</param>
        /// <returns></returns>
        public static bool UnzipFile(byte[] fileBytes, string outputPath, string password = null, Func<ZipEntry, bool> onPreUnzip = null, Action<ZipEntry> onPostUnzip = null, Action<bool> onFinished = null)
        {
            if ((null == fileBytes) || string.IsNullOrEmpty(outputPath))
            {
                onFinished?.Invoke(false);
                return false;
            }

            bool result = UnzipFile(new MemoryStream(fileBytes), outputPath, password, onPreUnzip, onPostUnzip, onFinished);
            if (!result)
            {
                onFinished?.Invoke(false);
            }

            return result;
        }

        /// <summary>
        /// 解压Zip包
        /// </summary>
        /// <param name="inputStream">Zip包输入流</param>
        /// <param name="outputPath">解压输出路径</param>
        /// <param name="password">解压密码</param>
        /// <param name="onPreUnzip">解压单个文件或文件夹前执行的回调</param>
        /// <param name="onPostUnzip">解压单个文件或文件夹后执行的回调</param>
        /// <param name="onFinished">解压执行完毕后的回调</param>
        /// <returns></returns>
        public static bool UnzipFile(Stream inputStream, string outputPath, string password = null, Func<ZipEntry, bool> onPreUnzip = null, Action<ZipEntry> onPostUnzip = null, Action<bool> onFinished = null)
        {
            if ((null == inputStream) || string.IsNullOrEmpty(outputPath))
            {
                onFinished?.Invoke(false);
                return false;
            }

            // 创建文件目录
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            // 解压Zip包
            ZipEntry entry = null;
            using (ZipInputStream zipInputStream = new ZipInputStream(inputStream))
            {
                if (!string.IsNullOrEmpty(password))
                    zipInputStream.Password = password;

                while (null != (entry = zipInputStream.GetNextEntry()))
                {
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    if ((null != onPreUnzip) && !onPreUnzip(entry))
                        continue;   // 过滤

                    string filePathName = Path.Combine(outputPath, entry.Name);

                    // 创建文件目录
                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(filePathName);
                        continue;
                    }

                    // 写入文件
                    try
                    {
                        using (FileStream fileStream = File.Create(filePathName))
                        {
                            byte[] bytes = new byte[1024];
                            while (true)
                            {
                                int count = zipInputStream.Read(bytes, 0, bytes.Length);
                                if (count > 0)
                                    fileStream.Write(bytes, 0, count);
                                else
                                {
                                    onPostUnzip?.Invoke(entry);
                                    break;
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        ex.HandleException(nameof(UnzipFile));
                        onFinished?.Invoke(false);
                        return false;
                    }
                }
            }

            onFinished?.Invoke(true);
            return true;
        }

        #endregion
    }

    #endregion
}

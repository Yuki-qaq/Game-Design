using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.NodeKit;

namespace XCSJ.EditorExtension.Base.NodeKit.Canvases
{
    /// <summary>
    /// 画布工具栏
    /// </summary>
    public class CanvasToolBar
    {
        /// <summary>
        /// 画布视图
        /// </summary>
        public ICanvasView canvasView { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="canvasView"></param>
        public CanvasToolBar(ICanvasView canvasView)
        {
            this.canvasView = canvasView ?? throw new ArgumentNullException(nameof(canvasView));
            Init();
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="canvasRect"></param>
        public void Draw(Rect canvasRect)
        {
            foreach (var kv in canvasToolBarButtonGroups)
            {
                kv.Value.Draw(canvasRect);
            }
        }

        private void Init()
        {
            canvasToolBarButtonGroups.Clear();

            var validates = new List<(MethodInfo, CanvasToolBarAttribute)>();
            foreach (var mi in canvasView.GetType().GetMethods())
            {
                if (mi.GetParameters().Length == 0 && AttributeCache<CanvasToolBarAttribute>.Get(mi) is CanvasToolBarAttribute canvasToolBarAttribute)
                {
                    if (canvasToolBarAttribute.validate)
                    {
                        if (mi.ReturnType == typeof(bool))
                        {
                            validates.Add((mi, canvasToolBarAttribute));
                        }
                    }
                    else
                    {
                        if (!canvasToolBarButtonGroups.TryGetValue(canvasToolBarAttribute.group, out var group))
                        {
                            canvasToolBarButtonGroups[canvasToolBarAttribute.group] = group = new CanvasToolBarButtonGroup(canvasView, canvasToolBarAttribute.group);
                        }
                        group.canvasToolBarButtons.Add(new CanvasToolBarButton(group, mi, canvasToolBarAttribute));
                    }
                }
            }

            canvasToolBarButtonGroups = canvasToolBarButtonGroups.Sort();
            int i = 0;
            foreach (var kv in canvasToolBarButtonGroups)
            {
                var group = kv.Value;
                group.sortIndex = i++;
                foreach (var b in group.canvasToolBarButtons)
                {
                    b.validateMethodInfo = validates.FirstOrDefault(vm => vm.Item2.name == b.canvasToolBarAttribute.name).Item1;
                }
                group.Sort();
            }
        }

        /// <summary>
        /// 画布工具栏按钮组字典
        /// </summary>
        public Dictionary<int, CanvasToolBarButtonGroup> canvasToolBarButtonGroups = new Dictionary<int, CanvasToolBarButtonGroup>();
    }

    /// <summary>
    /// 画布工具栏特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CanvasToolBarAttribute : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// 验证:是验证方法
        /// </summary>
        public bool validate { get; private set; } = false;

        /// <summary>
        /// 组：组索引
        /// </summary>
        public int group { get; set; } = 0;

        /// <summary>
        /// 索引：组内索引
        /// </summary>
        public int index { get; set; } = 0;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="validate"></param>
        public CanvasToolBarAttribute(string name, bool validate = false)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.validate = validate;
        }
    }

    /// <summary>
    /// 画布工具栏按钮组
    /// </summary>
    public class CanvasToolBarButtonGroup
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="canvasView"></param>
        /// <param name="group"></param>
        internal CanvasToolBarButtonGroup(ICanvasView canvasView, int group)
        {
            this.canvasView = canvasView;
            this.group = group;
        }

        /// <summary>
        /// 画布视图
        /// </summary>
        public ICanvasView canvasView { get; private set; }

        /// <summary>
        /// 组：组索引
        /// </summary>
        public int group { get; private set; }

        /// <summary>
        /// 排序索引
        /// </summary>
        public int sortIndex { get; internal set; }

        /// <summary>
        /// 画布工具栏按钮列表
        /// </summary>
        public List<CanvasToolBarButton> canvasToolBarButtons = new List<CanvasToolBarButton>();

        /// <summary>
        /// 排序
        /// </summary>
        public void Sort()
        {
            canvasToolBarButtons.Sort((x, y) => Comparer<int>.Default.Compare(x.index, y.index));
            for (int i = 0; i < canvasToolBarButtons.Count; i++)
            {
                canvasToolBarButtons[i].sortIndex = i;
            }
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="canvasRect"></param>
        public void Draw(Rect canvasRect)
        {
            foreach (var button in canvasToolBarButtons)
            {
                button.Draw(canvasRect);
            }
        }
    }

    /// <summary>
    /// 画布工具栏按钮
    /// </summary>
    public class CanvasToolBarButton
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="canvasToolBarButtonGroup"></param>
        /// <param name="executeMethodInfo"></param>
        /// <param name="canvasToolBarAttribute"></param>
        internal CanvasToolBarButton(CanvasToolBarButtonGroup canvasToolBarButtonGroup, MethodInfo executeMethodInfo, CanvasToolBarAttribute canvasToolBarAttribute)
        {
            this.canvasToolBarButtonGroup = canvasToolBarButtonGroup;
            this.executeMethodInfo = executeMethodInfo;
            this.canvasToolBarAttribute = canvasToolBarAttribute;
        }

        /// <summary>
        /// 画布工具栏按钮组
        /// </summary>
        public CanvasToolBarButtonGroup canvasToolBarButtonGroup { get; private set; }

        /// <summary>
        /// 画布视图
        /// </summary>
        public ICanvasView canvasView => canvasToolBarButtonGroup.canvasView;

        /// <summary>
        /// 画布工具栏特性:非验证方法上修饰的特性实例对象
        /// </summary>
        public CanvasToolBarAttribute canvasToolBarAttribute { get; private set; }

        /// <summary>
        /// 索引
        /// </summary>
        public int index => canvasToolBarAttribute.index;

        /// <summary>
        /// 排序索引
        /// </summary>
        public int sortIndex { get; internal set; }

        /// <summary>
        /// 执行方法信息
        /// </summary>
        public MethodInfo executeMethodInfo { get; private set; }

        /// <summary>
        /// 验证方法信息
        /// </summary>
        public MethodInfo validateMethodInfo { get; internal set; }

        /// <summary>
        /// 可执行
        /// </summary>
        /// <returns></returns>
        public bool CanExecute()
        {
            try
            {
                if (validateMethodInfo == null) return true;
                if (canvasView == null) return false;
                return (bool)validateMethodInfo.Invoke(canvasView, Empty<object>.Array);
            }
            catch (Exception ex)
            {
                ex.HandleException(nameof(CanExecute));
                return false;
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void Execute()
        {
            try
            {
                if (executeMethodInfo == null || canvasView == null) return;
                executeMethodInfo.Invoke(canvasView, Empty<object>.Array);
            }
            catch (Exception ex)
            {
                ex.HandleException(nameof(Execute));
            }
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="canvasRect"></param>
        public void Draw(Rect canvasRect)
        {
            Vector2 buttonSize = NodeKitHelperExtension.customStyle.canvasToolBarButtonSize;
            Vector2 space = NodeKitHelperExtension.customStyle.canvasToolBarButtonSpace;
            var rect = new Rect(canvasRect.xMax - (buttonSize.x + space.x) * (sortIndex + 1), canvasRect.y + space.y + canvasToolBarButtonGroup.sortIndex * (buttonSize.y + space.y), buttonSize.x, buttonSize.y);
            EditorGUI.BeginDisabledGroup(!CanExecute());
            if (GUI.Button(rect, CommonFun.NameTip(executeMethodInfo, ENameTip.EmptyTextWhenHasImage)))
            {
                Execute();
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}

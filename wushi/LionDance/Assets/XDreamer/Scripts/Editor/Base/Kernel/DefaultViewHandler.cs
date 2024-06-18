using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorCommonUtils.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit;
using XCSJ.EditorExtension.Base.NodeKit.Canvases;
using XCSJ.EditorExtension.Base.NodeKit.Nodes;
using XCSJ.EditorXGUI;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.Kernel
{
    /// <summary>
    /// 缺省视图处理器
    /// </summary>
    public class DefaultViewHandler : InstanceClass<DefaultViewHandler>, IViewHandler
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="componentTypes"></param>
        /// <returns></returns>
        public GameObject Create(string name, Transform parent = null, params Type[] componentTypes)
        {
            GameObject newGO = null;

            try
            {
                // 创建画布类型的游戏对象比较复杂,单独进行创建操作
                if (componentTypes.Contains(typeof(Canvas)))
                {
                    newGO = EditorXGUIHelper.FindOrCreateRootCanvas();
                }
                else if (parent && parent.GetComponent<Canvas>())
                {
                    newGO = UnityObjectHelper.CreateGameObject(name);
                    newGO.XAddComponent<RectTransform>();
                }
                else
                {
                    newGO = UnityObjectHelper.CreateGameObject(name);
                }

                if (newGO)
                {
                    foreach (var item in componentTypes)
                    {
                        newGO.XGetOrAddComponent(item);
                    }

                    // 设置父子节点
                    if (parent)
                    {
                        newGO.XSetParent(parent);
                        newGO.XModifyProperty(()=> { GameObjectUtility.EnsureUniqueNameForSibling(newGO); });

                        // 如果游戏对象是RectTransform 则设置为随父节点缩放
                        if (newGO.GetComponent<RectTransform>() is RectTransform rect)
                        {
                            rect.XStretchHV();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.Log(ex);
            }

            return newGO;
        }

        /// <summary>
        /// 创建缺省画布视图
        /// </summary>
        /// <param name="canvasModelType"></param>
        /// <returns></returns>
        public BaseCanvasView CreateDefaultCanvasView(Type canvasModelType) => new CanvasView();

        /// <summary>
        /// 创建缺省实体节点视图
        /// </summary>
        /// <param name="nodeModelType"></param>
        /// <param name="canvasModelType"></param>
        /// <param name="useForParent"></param>
        /// <returns></returns>
        public BaseEntityNodeView CreateDefaultEntityNodeView(Type nodeModelType, Type canvasModelType, bool useForParent) => useForParent ? new ParentNodeView() : new NodeView();

        /// <summary>
        /// 创建缺省虚体节点视图
        /// </summary>
        /// <param name="nodeModelType"></param>
        /// <param name="canvasModelType"></param>
        /// <param name="useForParent"></param>
        /// <returns></returns>
        public BaseVirtualNodeView CreateDefaultVirtualNodeView(Type nodeModelType, Type canvasModelType, bool useForParent) => new VirtualNodeView();
    }
}

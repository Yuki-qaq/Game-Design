using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 渲染顺序:渲染顺序组件是用于设置游戏对象的渲染顺序的执行体
    /// </summary>
    [ComponentMenu(SMSCategory.ComponentDirectory + Title, typeof(SMSManager))]
    [Name(Title, nameof(RenderQueue))]
    [Tip("渲染顺序组件是用于设置游戏对象的渲染顺序的执行体", "The render order component is the executor used to set the render order of game objects")]
    [XCSJ.Attributes.Icon(EIcon.Render)]
    [RequireComponent(typeof(GameObjectSet))]
    public class RenderQueue : StateComponent<RenderQueue>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渲染顺序";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSCategory.Component, typeof(SMSManager))]
        [StateComponentMenu(SMSCategory.ComponentDirectory + Title, typeof(SMSManager))]
        [Name(Title, nameof(RenderQueue))]
        [Tip("渲染顺序组件是用于设置游戏对象的渲染顺序的执行体", "The render order component is the executor used to set the render order of game objects")]
        [XCSJ.Attributes.Icon(EIcon.Render)]
        public static State CreateRenderQueue(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 游戏对象集
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        /// <summary>
        /// 渲染模式
        /// </summary>
        [Name("渲染模式")]
        public enum ERenderingMode
        {
            /// <summary>
            /// 无操作
            /// </summary>
            [Tip("无操作", "无操作")]
            None,

            /// <summary>
            /// 默认
            /// </summary>
            [Tip("默认")]
            Default,

            /// <summary>
            /// 不透明
            /// </summary>
            [Tip("不透明", "Opaque")]
            Opaque,

            /// <summary>
            /// 镂空
            /// </summary>
            [Tip("镂空", "Cutout")]
            Cutout,

            /// <summary>
            /// 隐现
            /// </summary>
            [Tip("隐现", "Fade")]
            Fade,

            /// <summary>
            /// 透明
            /// </summary>
            [Tip("透明", "Transparent")]
            Transparent,
        }

        /// <summary>
        /// 进入时渲染顺序
        /// </summary>
        [Name("进入时渲染顺序")]
        [EnumPopup]
        public ERenderingMode enterRenderingMode = ERenderingMode.None;

        /// <summary>
        /// 退出时渲染顺序
        /// </summary>
        [Name("退出时渲染顺序")]
        [EnumPopup]
        public ERenderingMode exitRenderingMode = ERenderingMode.None;

        /// <summary>
        /// 包含成员
        /// </summary>
        [Name("包含成员")]
        public bool includeChildren = true;

        private List<Recorder> recorders = new List<Recorder>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            List<GameObject> objects = gameObjectSet.objects;
            foreach (var go in objects)
            {
                Recorder recorder = new Recorder();
                recorder.gameObject = go;
                recorder.includeChildren = includeChildren;
                recorder.Record();
                recorders.Add(recorder);
            }
            return base.Init(data);
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            
            base.OnEntry(data);
            if(enterRenderingMode == ERenderingMode.None) return;
            if(enterRenderingMode == ERenderingMode.Default)
            {
                foreach (var recoder in recorders)
                {
                    recoder.ResetRecorder();
                }
            }
            else
            {
                foreach (var recoder in recorders)
                {
                    recoder.SetRenderingMode(enterRenderingMode);
                }
            }
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished()
        {
            return true;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            if (exitRenderingMode != ERenderingMode.None)
            {
                if(exitRenderingMode == ERenderingMode.Default && enterRenderingMode != ERenderingMode.Default)
                {
                    foreach (var recoder in recorders)
                    {
                        recoder.ResetRecorder();
                    }
                }
                else if(exitRenderingMode != ERenderingMode.Default)
                {
                    foreach (var recoder in recorders)
                    {
                        recoder.SetRenderingMode(exitRenderingMode);
                    }
                }
            }
            base.OnExit(data);
        }

        /// <summary>
        /// 设置材质渲染模式
        /// </summary>
        /// <param name="material"></param>
        /// <param name="renderingMode"></param>
        public static void SetMaterialRenderingMode(Material material, ERenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case ERenderingMode.Opaque:
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = -1;
                        break;
                    }
                case ERenderingMode.Cutout:
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.EnableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 2450;
                        break;
                    }
                case ERenderingMode.Fade:
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        break;
                    }
                case ERenderingMode.Transparent:
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 设置材质渲染模式
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="renderingMode"></param>
        public static void SetMaterialsRenderingMode(Material[] materials, ERenderingMode renderingMode)
        {
            foreach (Material mat in materials)
                SetMaterialRenderingMode(mat, renderingMode);
        }

        /// <summary>
        /// 记录器
        /// </summary>
        public class Recorder
        {
            /// <summary>
            /// 游戏对象
            /// </summary>
            public GameObject gameObject;

            /// <summary>
            /// 包含子级
            /// </summary>
            public bool includeChildren;

            /// <summary>
            /// 渲染队列
            /// </summary>
            public List<MaterialRenderQuene> renderQuenes = new List<MaterialRenderQuene>();

            /// <summary>
            /// 记录
            /// </summary>
            public void Record()
            {
                if (!gameObject) return;
                renderQuenes.Clear();
                if (includeChildren)
                {
                    Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer ren in renderers)
                    {
                        Record(ren);
                    }
                }
                else
                {
                    Record(gameObject.GetComponent<Renderer>());
                }
            }

            private void Record(Renderer renderer)
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (!materials[i].shader.name.Contains("Standard")) continue;
                    ERenderingMode renderingMode = ERenderingMode.Opaque;
                    if (materials[i].renderQueue == 2000)
                    {
                        renderingMode = ERenderingMode.Opaque;
                    }
                    else if (materials[i].renderQueue == 2450)
                    {
                        renderingMode = ERenderingMode.Cutout;
                    }
                    else if (materials[i].renderQueue == 3000 && materials[i].GetInt("_SrcBlend") == (int)UnityEngine.Rendering.BlendMode.SrcAlpha)
                    {
                        renderingMode = ERenderingMode.Fade;
                    }
                    else if (materials[i].renderQueue == 3000)
                    {
                        renderingMode = ERenderingMode.Transparent;
                    }
                    renderQuenes.Add(new MaterialRenderQuene(materials[i], renderingMode));
                }
            }

            /// <summary>
            /// 重置记录器
            /// </summary>
            public void ResetRecorder()
            {
                for(int i=0;i<renderQuenes.Count;i++)
                {
                    SetMaterialRenderingMode(renderQuenes[i].material, renderQuenes[i].renderingMode); 
                }
            }

            /// <summary>
            /// 设置渲染模式
            /// </summary>
            /// <param name="renderingMode"></param>
            public void SetRenderingMode(ERenderingMode renderingMode)
            {
                for (int i = 0; i < renderQuenes.Count; i++)
                {
                    SetMaterialRenderingMode(renderQuenes[i].material, renderingMode);
                }
            }
        }

        /// <summary>
        /// 材质渲染队列
        /// </summary>
        public class MaterialRenderQuene
        {
            /// <summary>
            /// 材质
            /// </summary>
            public Material material;

            /// <summary>
            /// 渲染模式
            /// </summary>
            [EnumPopup]
            public ERenderingMode renderingMode;

            /// <summary>
            /// 材质
            /// </summary>
            /// <param name="mat"></param>
            /// <param name="eRenderingMode"></param>
            public MaterialRenderQuene(Material mat, ERenderingMode eRenderingMode)
            {
                this.material = mat;
                this.renderingMode = eRenderingMode;
            }
        }

    }
}

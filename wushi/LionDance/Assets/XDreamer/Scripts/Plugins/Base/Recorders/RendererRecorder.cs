using System.Collections.Generic;
using UnityEngine;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 渲染器记录器
    /// </summary>
    public class RendererRecorder : Recorder<Renderer, RendererRecorder.Info>, IRecord<GameObject>, IRecord<IEnumerable<GameObject>>
    {
        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="obj"></param>
        public override void Record(Renderer obj)
        {
            if (!obj)
            {
                //Debug.Log("无效Renderer");
                return;
            }
            base.Record(obj);
        }

        /// <summary>
        /// 记录游戏对象上的渲染器，不包含子游戏对象
        /// </summary>
        /// <param name="gameObject"></param>
        public void Record(GameObject gameObject)
        {
            Record(gameObject, false);
        }

        /// <summary>
        /// 记录游戏对象上的渲染器
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="includeChildren"></param>
        public void Record(GameObject gameObject, bool includeChildren)
        {
            if (gameObject)
            {
                if (includeChildren)
                {
                    Record(gameObject.GetComponentsInChildren<Renderer>());
                }
                else
                {
                    Record(gameObject.GetComponent<Renderer>());
                }
            }
        }

        /// <summary>
        /// 记录游戏对象集合上的渲染器属性
        /// </summary>
        /// <param name="gameObjects"></param>
        public void Record(IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null) return;
            foreach (var go in gameObjects)
            {
                Record(go);
            }
        }

        /// <summary>
        /// 记录变换对象集合上的渲染器属性
        /// </summary>
        /// <param name="transforms"></param>
        public void Record(IEnumerable<Transform> transforms)
        {
            if (transforms == null) return;
            foreach (var t in transforms)
            {
                Record(t.gameObject);
            }
        }


        /// <summary>
        /// 信息
        /// </summary>
        public class Info : ISingleRecord<Renderer>
        {
            /// <summary>
            /// 渲染器
            /// </summary>
            [Json(false)]
            public Renderer renderer;

            /// <summary>
            /// 组件
            /// </summary>
            [Json(exportString = true)]
            public Component component { get => renderer; set => renderer = value as Renderer; }

            /// <summary>
            /// 启用
            /// </summary>
            public bool enabled;

            /// <summary>
            /// 渲染层遮罩
            /// </summary>
            public uint renderingLayerMask;

            /// <summary>
            /// 材质
            /// </summary>
            [Json(exportString = true)]
            public List<Material> materials = new List<Material>();

            /// <summary>
            /// 颜色
            /// </summary>
            [Json(exportString = true)]
            public List<Color> colors = new List<Color>();
            
            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="renderer"></param>
            public void Record(Renderer renderer)
            {
                this.renderer = renderer;
                if (renderer)
                {
                    this.enabled = renderer.enabled;
                    this.renderingLayerMask = renderer.renderingLayerMask;
                    foreach (var mat in renderer.materials)
                    {
                        materials.Add(mat);
                        colors.Add(mat.color);
                    }
                }
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                RecoverEnabled();
                RecoverRenderingLayerMask();
                RecoverMaterial();
                RecoverColor();
            }

            /// <summary>
            /// 恢复启用
            /// </summary>
            public void RecoverEnabled()
            {
                if (renderer)
                {
                    renderer.enabled = enabled;
                }
            }

            /// <summary>
            /// 恢复渲染层遮罩
            /// </summary>
            public void RecoverRenderingLayerMask()
            {
                if (renderer)
                {
                    renderer.renderingLayerMask = renderingLayerMask;
                }
            }

            /// <summary>
            /// 恢复材质
            /// </summary>
            public void RecoverMaterial()
            {
                if (renderer)
                {
                    renderer.materials = materials.ToArray();
                }
            }

            /// <summary>
            /// 恢复颜色
            /// </summary>
            public void RecoverColor()
            {
                if (renderer)
                {
                    for (int i = 0; i < renderer.materials.Length; ++i)
                    {
                        renderer.materials[i].color = colors[i];
                    }
                }
            }

            /// <summary>
            /// 设置百分比
            /// </summary>
            /// <param name="percent"></param>
            /// <param name="color"></param>
            public void SetPercent(Percent percent, Color color)
            {
                if (renderer)
                {
                    for (int i = 0; i < renderer.materials.Length; ++i)
                    {
                        renderer.materials[i].color = Color.Lerp(colors[i], color, (float)percent.percent01OfWorkCurve);
                    }
                }
            }

            /// <summary>
            /// 设置颜色
            /// </summary>
            /// <param name="color"></param>
            public void SetColor(Color color)
            {
                if (renderer)
                {
                    for (int i = 0; i < renderer.materials.Length; ++i)
                    {
                        renderer.materials[i].color = color;
                    }
                }
            }

            /// <summary>
            /// 设置百分比
            /// </summary>
            /// <param name="percent"></param>
            /// <param name="alpha"></param>
            public void SetPercent(Percent percent, float alpha)
            {
                if (renderer)
                {
                    for (int i = 0; i < renderer.materials.Length; ++i)
                    {
                        var color = colors[i];
                        var dstColor = new Color(color.r, color.g, color.b, alpha);
                        renderer.materials[i].color = Color.Lerp(color, dstColor, (float)percent.percent01OfWorkCurve);
                    }
                }
            }

            /// <summary>
            /// 设置透明度
            /// </summary>
            /// <param name="alpha"></param>
            public void SetAlpha(float alpha)
            {
                if (renderer)
                {
                    for (int i = 0; i < renderer.materials.Length; ++i)
                    {
                        var dstColor = colors[i];
                        dstColor.a = alpha;
                        renderer.materials[i].color = dstColor;
                    }
                }
            }

            /// <summary>
            /// 设置材质
            /// </summary>
            /// <param name="materials"></param>
            public void SetMaterial(params Material[] materials)
            {
                if (renderer)
                {
                    renderer.materials = materials;
                }
            }

            /// <summary>
            /// 填充材质尺寸
            /// </summary>
            /// <param name="materials"></param>
            public void FillMaterialSize(params Material[] materials)
            {
                if (!renderer) return;

                if (materials.Length == 0)
                {
                    renderer.materials = materials;
                }
                else
                {
                    if (renderer.materials.Length != materials.Length)
                    {
                        Material[] newMaterials = new Material[renderer.materials.Length];
                        int length = materials.Length;
                        for (int i = 0; i < renderer.materials.Length; ++i)
                        {
                            newMaterials[i] = (materials[i % length]);
                        }
                        materials = newMaterials;
                    }

                    renderer.materials = materials;
                }
            }

            /// <summary>
            /// 设置启用
            /// </summary>
            /// <param name="enabled"></param>
            public void SetEnabled(bool enabled)
            {
                if (renderer)
                {
                    renderer.enabled = enabled;
                }
            }

            /// <summary>
            /// 设置启用
            /// </summary>
            /// <param name="enabled"></param>
            public void SetEnabled(EBool enabled)
            {
                if (renderer)
                {
                    renderer.enabled = CommonFun.BoolChange(renderer.enabled, enabled);
                }
            }
        }
    }

    /// <summary>
    /// 渲染层记录器：仅记录渲染层信息，提高记录和还原效率
    /// </summary>
    public class RendererRenderingLayerMaskRecorder : Recorder<Renderer, RendererRenderingLayerMaskRecorder.Info>
    {
        /// <summary>
        /// 信息
        /// </summary>
        public class Info : ISingleRecord<Renderer>
        {
            /// <summary>
            /// 渲染器
            /// </summary>
            public Renderer renderer;

            /// <summary>
            /// 渲染层遮罩
            /// </summary>
            public uint renderingLayerMask;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="renderer"></param>
            public void Record(Renderer renderer)
            {
                this.renderer = renderer;
                if (this.renderer)
                {
                    renderingLayerMask = this.renderer.renderingLayerMask;
                }
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                if (renderer)
                {
                    renderer.renderingLayerMask = renderingLayerMask;
                }
            }
        }
    }
}

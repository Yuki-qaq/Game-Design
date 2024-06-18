using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Renderers
{
    /// <summary>
    /// GL纹理渲染器
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Render)]
    [Name("GL纹理渲染器")]
    [Tip("基于屏幕[0,1]坐标体系使用GL绘制GL.QUADS形式的纹理；", "Draw GL using GL based on the screen [0,1] coordinate system Texture in quads form;")]
    [Tool(ToolsCategory.Renderer, rootType = typeof(ToolsManager))]
    public class GLTextureRenderer : ToolRenderer
    {
        /// <summary>
        /// 材质
        /// </summary>
        [Name("材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material material;

        /// <summary>
        /// 通道
        /// </summary>
        [Name("通道")]
        [Range(0, 7)]
        public int pass = 0;

        /// <summary>
        /// 顶点0
        /// </summary>
        [Name("顶点0")]
        public Vector3 vertex0 = new Vector3(0.25f, 0.25f, 0);

        /// <summary>
        /// UV0
        /// </summary>
        [Name("UV0")]
        [Tip("顶点0对应的UV坐标", "UV coordinates corresponding to vertex 0")]
        public Vector3 uv0 = new Vector3(0, 0, 0);

        /// <summary>
        /// 颜色0
        /// </summary>
        [Name("颜色0")]
        [Tip("顶点0对应的颜色", "Color corresponding to vertex 0")]
        public Color color0 = new Color(1, 1, 1, 1);

        /// <summary>
        /// 顶点1
        /// </summary>
        [Name("顶点1")]
        public Vector3 vertex1 = new Vector3(0.25f, 0.75f, 0);

        /// <summary>
        /// UV1
        /// </summary>
        [Name("UV1")]
        [Tip("顶点1对应的UV坐标", "UV coordinates corresponding to vertex 1")]
        public Vector3 uv1 = new Vector3(0, 1, 0);

        /// <summary>
        /// 颜色1
        /// </summary>
        [Name("颜色1")]
        [Tip("顶点1对应的颜色", "Color corresponding to vertex 1")]
        public Color color1 = new Color(1, 1, 1, 1);

        /// <summary>
        /// 顶点2
        /// </summary>
        [Name("顶点2")]
        public Vector3 vertex2 = new Vector3(0.75f, 0.75f, 0);

        /// <summary>
        /// UV2
        /// </summary>
        [Name("UV2")]
        [Tip("顶点2对应的UV坐标", "UV coordinates corresponding to vertex 2")]
        public Vector3 uv2 = new Vector3(1, 1, 0);

        /// <summary>
        /// 颜色2
        /// </summary>
        [Name("颜色2")]
        [Tip("顶点2对应的颜色", "Color corresponding to vertex 2")]
        public Color color2 = new Color(1, 1, 1, 1);

        /// <summary>
        /// 顶点3
        /// </summary>
        [Name("顶点3")]
        public Vector3 vertex3 = new Vector3(0.75f, 0.25f, 0);

        /// <summary>
        /// UV3
        /// </summary>
        [Name("UV3")]
        [Tip("顶点3对应的UV坐标", "UV coordinates corresponding to vertex 3")]
        public Vector3 uv3 = new Vector3(1, 0, 0);

        /// <summary>
        /// 颜色3
        /// </summary>
        [Name("颜色3")]
        [Tip("顶点3对应的颜色")]
        public Color color3 = new Color(1, 1, 1, 1);

        /// <summary>
        /// 绘制对象
        /// </summary>
        private void OnRenderObject()
        {
            if (!material) return;

            GL.PushMatrix();
            {
                material.SetPass(pass);

                //正交模式绘制
                GL.LoadOrtho();

                //开始绘制
                GL.Begin(GL.QUADS);
                {
                    GL.TexCoord(uv0);
                    GL.Color(color0);
                    GL.Vertex(vertex0);

                    GL.TexCoord(uv1);
                    GL.Color(color1);
                    GL.Vertex(vertex1);

                    GL.TexCoord(uv2);
                    GL.Color(color2);
                    GL.Vertex(vertex2);

                    GL.TexCoord(uv3);
                    GL.Color(color3);
                    GL.Vertex(vertex3);
                }
                //结束绘制
                GL.End();
            }
            GL.PopMatrix();
        }
    }
}

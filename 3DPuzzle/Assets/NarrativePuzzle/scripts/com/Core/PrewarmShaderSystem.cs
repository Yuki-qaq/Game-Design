using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class PrewarmShaderSystem : MonoBehaviour
{
    public Shader[] shaders;
 
    void Awake()
    {
        //(Shader shader, ShaderWarmupSetup setup
        var setup = new ShaderWarmupSetup();
        var decl1 = new VertexAttributeDescriptor();
        var decl2 = new VertexAttributeDescriptor(VertexAttribute.Normal);
        var decl3 = new VertexAttributeDescriptor(VertexAttribute.Tangent);
        var decl4 = new VertexAttributeDescriptor(VertexAttribute.TexCoord0);

        setup.vdecl = new VertexAttributeDescriptor[] { decl1, decl2, decl3, decl4 };

        foreach (var s in shaders)
            ShaderWarmup.WarmupShader(s, setup);
    }
}

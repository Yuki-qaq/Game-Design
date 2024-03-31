
using UnityEngine;

public class ColorPropertySetter : MonoBehaviour
{
    //The color of the object
    public bool applyInInspector = false;
    public bool applyOnStart = true;

    [Space]
    public Renderer materialRenderer;
    public Color materialColor;
    public string colorProperty = "_Color";

    [Space]
    //The material property block we pass to the GPU
    private MaterialPropertyBlock propertyBlock;


    private void Reset()
    {
        propertyBlock ??= new MaterialPropertyBlock();
        materialRenderer= GetComponent<Renderer>();
        RemoveColor();
    }

    // OnValidate is called in the editor after the component is edited
    void OnValidate()
    {
        InspectorApply();
    }

    public void InspectorApply()
    {
        if (!applyInInspector)
            return;

        //create propertyblock only if none exists
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        //Get a renderer component either of the own gameobject or of a child
        materialRenderer = GetComponentInChildren<Renderer>();
        //set the color property
        propertyBlock.SetColor(colorProperty, materialColor);
        //apply propertyBlock to renderer
        materialRenderer.SetPropertyBlock(propertyBlock);
    }

    private void Awake()
    {
        propertyBlock ??= new MaterialPropertyBlock();

        materialRenderer = GetComponentInChildren<Renderer>();

        if (applyOnStart)
            ApplyColor();
        //materialRenderer.SetPropertyBlock(propertyBlock);
    }

    public void ApplyColor(Color color)
    {
        propertyBlock.SetColor(colorProperty, color);
        materialRenderer.SetPropertyBlock(propertyBlock);
    }

    public void ApplyColor() => ApplyColor(materialColor);

    public void ApplyRandomColor() => ApplyColor(Random.ColorHSV());

    public void RemoveColor()
    {
        propertyBlock.Clear();
        materialRenderer.SetPropertyBlock(propertyBlock);
    }
}
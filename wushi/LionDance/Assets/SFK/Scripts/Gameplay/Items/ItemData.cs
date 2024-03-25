using UnityEngine;

public class ItemData : MonoBehaviour
{
    public ItemDataScriptable dataObject;
    public ColorPropertySetter colorSetter;

    public int ID;

    private void Reset()
    {
        colorSetter = GetComponent<ColorPropertySetter>();
    }

    private void OnValidate()
    {
        if (dataObject)
        {
            ID = dataObject.ID;

            if (colorSetter)
            {
                colorSetter.materialColor = dataObject.color;
                colorSetter.InspectorApply();
            }
        }
    }

    private void Start()
    {
        if (dataObject != null)
        {
            ID = dataObject.ID;

            if (colorSetter)
                colorSetter.ApplyColor(dataObject.color);
        }

    }
}

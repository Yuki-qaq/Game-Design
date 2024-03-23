using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemDataScriptable : ScriptableObject
{
    public Color color;
    public int ID;

    private void Reset()
    {
        ID = GetInstanceID();
        color = Random.ColorHSV();
    }
}

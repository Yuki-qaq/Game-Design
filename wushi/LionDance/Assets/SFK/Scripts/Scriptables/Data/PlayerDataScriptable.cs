using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/Player Data", order = 1)]
public class PlayerDataScriptable : ScriptableObject
{
    public int ID;
    public string displayName = "Display Name";
    public IntVariableScriptable scoreVariable;

    public event Action RestartEvent;

    public void Restart()
    {
        RestartEvent?.Invoke();
    }

    private void Reset()
    {
        ID = GetInstanceID();
    }
}

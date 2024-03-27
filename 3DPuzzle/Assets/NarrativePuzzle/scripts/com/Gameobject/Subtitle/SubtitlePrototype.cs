using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ibis/SubtitlePrototype")]
public class SubtitlePrototype : ScriptableObject
{
    public string txt;
    public string soundId;
    public SubtitlePrototype next;
}
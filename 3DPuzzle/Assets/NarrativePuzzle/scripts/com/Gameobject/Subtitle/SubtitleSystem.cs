using System;
using UnityEngine;

public class SubtitleSystem : MonoBehaviour
{
    public static SubtitleSystem instance;

    [HideInInspector]
    public SubtitlePanelBehaviour panelBehaviour;
    public SubtitleConfig config;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    public void Show(SubtitlePrototype sp)
    {
        panelBehaviour.Show(sp);
    }
}
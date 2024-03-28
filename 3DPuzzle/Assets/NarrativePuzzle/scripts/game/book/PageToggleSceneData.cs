using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PageToggleSceneData
{
    public int page;
    public GameObject[] toShow;
    public GameObject[] toHide;
    public UnityEvent evt;

    public void On()
    {
        foreach (var th in toHide)
            th.SetActive(false);
        foreach (var ts in toShow)
            ts.SetActive(true);
        evt?.Invoke();
    }

    public void Off()
    {
        foreach (var th in toHide)
            th.SetActive(true);
        foreach (var ts in toShow)
            ts.SetActive(false);
    }
}
using DG.Tweening;
using UnityEngine;

public class BookSceneSwitcher : MonoBehaviour
{
    public CanvasGroup cg;
    public GameObject[] toShow;
    public GameObject[] toHide;
    public float duration;
    public void SwitchToHouseScene()
    {
        cg.DOFade(1, duration).OnComplete(
            () =>
            {
                foreach (var th in toHide)
                    th.SetActive(false);
                foreach (var ts in toShow)
                    ts.SetActive(true);

                cg.DOFade(0, duration);
            }
            );
    }

}
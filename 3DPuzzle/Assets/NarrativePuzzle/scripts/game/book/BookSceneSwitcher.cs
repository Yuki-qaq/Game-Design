using DG.Tweening;
using System;
using UnityEngine;

public class BookSceneSwitcher : MonoBehaviour
{
    public CanvasGroup cg;
    public GameObject[] toShow;
    public GameObject[] toHide;
    public float duration;

    public static BookSceneSwitcher notAGoodInstance;

    private void Awake()
    {
        notAGoodInstance = this;
    }

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

        SmallHouseBehaviour.instance.InitBooks();
    }

    public static void Switch(Action action)
    {
        notAGoodInstance.GeneralSwitch(action);
    }

    public void GeneralSwitch(Action action)
    {
        cg.DOFade(1, duration).OnComplete(
            () =>
            {
                action?.Invoke();
                cg.DOFade(0, duration);
            }
            );
    }
}
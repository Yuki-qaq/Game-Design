using DG.Tweening;
using System;
using UnityEngine;

public class BookSceneSwitcher : MonoBehaviour
{
    public CanvasGroup cg;
    public GameObject[] toShow;
    public GameObject[] toHide;
    public float duration;
    public GameObject fpc;
    public GameObject fpcAttachPlace;

    public static BookSceneSwitcher notAGoodInstance;

    private void Awake()
    {
        notAGoodInstance = this;
    }

    void CommonSwitchScene()
    {
        GeneralSwitch(() =>
        {
            foreach (var th in toHide)
                th.SetActive(false);
            foreach (var ts in toShow)
                ts.SetActive(true);

            fpc.transform.position = fpcAttachPlace.transform.position;
            fpc.transform.rotation = fpcAttachPlace.transform.rotation;
        });
    }
    public void SwitchToDarkRoom()
    {
        CommonSwitchScene();
        DarkRoomBehaviour.instance.InitPuzzle();
    }

    public void SwitchToHouseScene()
    {
        CommonSwitchScene();
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
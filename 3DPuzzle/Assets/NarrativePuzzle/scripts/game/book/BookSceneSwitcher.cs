using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Collections.Specialized.BitVector32;

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
        Debug.Log("SwitchToDarkRoom");
        //CommonSwitchScene();
        //DarkRoomBehaviour.instance.InitPuzzle();
        //TODO change Scene
        FadeToWhite(() => {  SceneManager.LoadScene(1);});
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

    public void FadeToWhite(Action action)
    {
        cg.DOFade(1, duration).OnComplete(
      () =>
      {
          action?.Invoke();
      });
    }

    public void FadeFromWhite(Action action)
    {
        cg.alpha = 1;
        cg.DOFade(0, duration).OnComplete(
     () =>
     {
         action?.Invoke();
     });
    }
}
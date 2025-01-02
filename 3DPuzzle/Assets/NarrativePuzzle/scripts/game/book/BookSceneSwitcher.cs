using DG.Tweening;
using System;
using System.Collections;
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
        FadeToWhite(() => { SceneManager.LoadScene(1); });
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

    public Transform[] GuideCameraBook1_1_trans;
    public Transform[] GuideCameraBook1_2_trans;
    public Transform[] GuideCameraBook2_1_trans;
    public GameObject bookControlPad;
    public Transform bookCam1;
    public Transform bookCam2;

    public void GuideCameraBook1_1()
    {
        StartCoroutine(GuideCameraAndDisableBookControlThenBack(GuideCameraBook1_1_trans, bookCam1));
    }

    public void GuideCameraBook1_2()
    {
        StartCoroutine(GuideCameraAndDisableBookControlThenBack(GuideCameraBook1_2_trans, bookCam1));
    }

    public void GuideCameraBook2_1()
    {
        StartCoroutine(GuideCameraAndDisableBookControlThenBack(GuideCameraBook2_1_trans, bookCam2));
    }


    IEnumerator GuideCameraAndDisableBookControlThenBack(Transform[] trans, Transform cameraTrans)
    {
        var pos0 = cameraTrans.position;
        var rot0 = cameraTrans.eulerAngles;
        bookControlPad.SetActive(false);
        yield return new WaitForSeconds(1);
        var duration = 1.5f;
        foreach (var t in trans)
        {
            yield return new WaitForSeconds(1);
            cameraTrans.DOMove(t.position, duration);
            cameraTrans.DORotate(t.eulerAngles, duration);
            yield return new WaitForSeconds(duration);
        }

        yield return new WaitForSeconds(0.8f);
        var d2 = duration + 2;
        cameraTrans.DOMove(pos0, d2).SetEase(Ease.OutCubic);
        cameraTrans.DORotate(rot0, d2).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(d2);
        yield return new WaitForSeconds(d2);
        bookControlPad.SetActive(true);
    }
}
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

    bool _GuideCameraBook1_1_done = false;
    bool _GuideCameraBook1_2_done = false;
    bool _GuideCameraBook2_1_done = false;

    public void GuideCameraBook1_1()
    {
        if (_GuideCameraBook1_1_done)
            return;

        _GuideCameraBook1_1_done = true;
        StartCoroutine(GuideCameraAndDisableBookControlThenBack(GuideCameraBook1_1_trans, bookCam1));
    }

    public void GuideCameraBook1_2()
    {
        if (_GuideCameraBook1_2_done)
            return;

        _GuideCameraBook1_2_done = true;
        StartCoroutine(GuideCameraAndDisableBookControlThenBack(GuideCameraBook1_2_trans, bookCam1));
    }

    public void GuideCameraBook2_1()
    {
        if (_GuideCameraBook2_1_done)
            return;

        _GuideCameraBook2_1_done = true;
        StartCoroutine(GuideCameraAndDisableBookControlThenBack(GuideCameraBook2_1_trans, bookCam2));
    }


    IEnumerator GuideCameraAndDisableBookControlThenBack(Transform[] trans, Transform cameraTrans)
    {
        Debug.Log("GuideCameraAndDisableBookControlThenBack");
        Debug.Log(trans.Length);
        Debug.Log(cameraTrans.gameObject);

        cameraTrans.DOKill();
        var pos0 = cameraTrans.position;
        var rot0 = cameraTrans.eulerAngles;
        bookControlPad.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        var duration = 1.6f;
        bool first = true;
        foreach (var t in trans)
        {
            yield return new WaitForSeconds(1);
            var d = duration;
            if (first)
            {
                d += 1;
                first = false;
            }
            cameraTrans.DOMove(t.position, d).SetEase(Ease.InOutCubic);
            cameraTrans.DORotate(t.eulerAngles, d).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(d);
        }

        yield return new WaitForSeconds(0.5f);
        var d2 = duration + 2;
        cameraTrans.DOKill();
        cameraTrans.DOMove(pos0, d2).SetEase(Ease.OutCubic);
        cameraTrans.DORotate(rot0, d2).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(d2);
        bookControlPad.SetActive(true);
    }
}
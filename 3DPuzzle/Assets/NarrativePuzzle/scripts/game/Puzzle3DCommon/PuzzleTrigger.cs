using com;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleTrigger : MonoBehaviour
{
    public Transform mainCamera;
    public Transform puzzleCamera;
    public GameObject hud_enterPuzzle;
    public GameObject triggerCol_enterPuzzle;
    public float durationTransitCamera;
    public FirstPersonController fpc;
    private Transform _mainCamera_defaultParent;

    public UnityEvent puzzleEndInstantEvt;
    public UnityEvent puzzleEndEvt;
    public UnityEvent puzzleStartInstantEvt;
    public UnityEvent puzzleStartEvt;

    private void Start()
    {
        hud_enterPuzzle.SetActive(false);
        _mainCamera_defaultParent = mainCamera.parent;
    }

    public void InitPuzzle()
    {
        fpc.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnterPuzzle()
    {
        puzzleStartInstantEvt?.Invoke();
        triggerCol_enterPuzzle.SetActive(false);
        hud_enterPuzzle.SetActive(false);
        //Debug.Log("EnterPuzzle ");
        Cursor.lockState = CursorLockMode.None;
        mainCamera.SetParent(null);
        //off player camera follow
        //off player control

        fpc.enabled = false;
        mainCamera.DOMove(puzzleCamera.position, durationTransitCamera).SetEase(Ease.InOutCubic).
            OnComplete(() => { puzzleStartEvt?.Invoke(); });
        mainCamera.DORotate(puzzleCamera.eulerAngles, durationTransitCamera).SetEase(Ease.InOutCubic);
    }

    public void OnPuzzleEnd()
    {
        //Debug.Log("OnPuzzleEnd ");
        StartCoroutine(OnPuzzleEnd_Coroutine());
    }

    IEnumerator OnPuzzleEnd_Coroutine()
    {
        puzzleEndInstantEvt?.Invoke();
        yield return new WaitForSeconds(1.0f);
        mainCamera.SetParent(_mainCamera_defaultParent);
        mainCamera.DOKill();
        mainCamera.DOMove(_mainCamera_defaultParent.position, 1).SetEase(Ease.InOutCubic);
        mainCamera.DORotate(_mainCamera_defaultParent.eulerAngles, 1).SetEase(Ease.InOutCubic);
        //mainCamera.localEulerAngles = Vector3.zero;
        //mainCamera.localPosition = Vector3.zero;
        yield return new WaitForSeconds(1.1f);
        fpc.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        puzzleEndEvt?.Invoke();
    }

    public void ToggleWatchPuzzle(bool on)
    {
        //Debug.Log("ToggleWatchPuzzle " + on);
        hud_enterPuzzle.SetActive(on);
    }
}
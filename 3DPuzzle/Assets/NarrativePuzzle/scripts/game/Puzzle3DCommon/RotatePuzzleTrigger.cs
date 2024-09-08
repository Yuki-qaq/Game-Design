using com;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RotatePuzzleTrigger : MonoBehaviour
{
    public Transform mainCamera;
    public Transform puzzleCamera;
    public GameObject hud_enterPuzzle;
    public GameObject triggerCol_enterPuzzle;
    public float durationTransitCamera;
    public FirstPersonController fpc;
    private Transform _mainCamera_defaultParent;
    public RotatePuzzleBehaviour rpb;
    public bool inPuzzle { get; private set; }


    private void Start()
    {
        hud_enterPuzzle.SetActive(false);
        _mainCamera_defaultParent = mainCamera.parent;
        inPuzzle = false;
    }

    public void InitPuzzle()
    {
        fpc.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnterPuzzle()
    {
        hud_enterPuzzle.SetActive(false);
        triggerCol_enterPuzzle.SetActive(false);
        Debug.Log("EnterPuzzle ");
        Cursor.lockState = CursorLockMode.None;
        mainCamera.SetParent(null);
        //off player camera follow
        //off player control
        fpc.enabled = false;
        rpb.enabled = true;

        mainCamera.DOMove(puzzleCamera.position, durationTransitCamera).SetEase(Ease.InOutCubic).OnComplete(
            () =>
            { inPuzzle = true; }
            );
        mainCamera.DORotate(puzzleCamera.eulerAngles, durationTransitCamera).SetEase(Ease.InOutCubic);
    }

    public void OnPuzzleEnd()
    {
        Debug.Log("OnPuzzleEnd ");
        inPuzzle = false;
        rpb.enabled = false;
        //show some chat
        //play sound
        StartCoroutine(OnPuzzleEnd_Coroutine());
    }

    IEnumerator OnPuzzleEnd_Coroutine()
    {
        yield return new WaitForSeconds(2.0f);
        mainCamera.SetParent(_mainCamera_defaultParent);
        mainCamera.localEulerAngles = Vector3.zero;
        mainCamera.localPosition = Vector3.zero;
        mainCamera.DOKill();
        fpc.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleWatchPuzzle(bool on)
    {
        Debug.Log("ToggleWatchPuzzle " + on);
        hud_enterPuzzle.SetActive(on);
    }
}
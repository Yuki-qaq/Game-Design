using DG.Tweening;
using UnityEngine;

public class SmallHouseBehaviour : MonoBehaviour
{
    public Transform mainCamera;
    public Transform puzzleCamera;
    public GameObject endBook;
    public GameObject hud_enterPuzzle;
    public GameObject hud_enterEndBook;
    public float durationTransitCamera;
    public FirstPersonController fpc;
    private Transform _mainCamera_defaultParent;

    private void Start()
    {
        hud_enterPuzzle.SetActive(false);
        hud_enterEndBook.SetActive(false);
        endBook.SetActive(false);
        _mainCamera_defaultParent = mainCamera.parent;
    }
    public void ToggleWatchBookShelf(bool on)
    {
        Debug.Log("ToggleWatchBookShelf " + on);
        hud_enterPuzzle.SetActive(on);
    }

    public void ToggleWatchEndBook(bool on)
    {
        Debug.Log("ToggleWatchEndBook " + on);
        hud_enterEndBook.SetActive(on);
    }

    public void EnterPuzzle()
    {
        Debug.Log("EnterPuzzle ");
        mainCamera.SetParent(null);
        //off player camera follow
        //off player control
        fpc.enabled = false;
        mainCamera.DOMove(puzzleCamera.position, durationTransitCamera).SetEase(Ease.InOutCubic);
        mainCamera.DORotate(puzzleCamera.eulerAngles, durationTransitCamera).SetEase(Ease.InOutCubic);
    }

    public void OnPuzzleEnd()
    {
        //on player camera follow
        //on player control
        mainCamera.SetParent(_mainCamera_defaultParent);
        mainCamera.localPosition = Vector3.zero;
        mainCamera.localEulerAngles = Vector3.zero;
        fpc.enabled = true;

        Debug.Log("OnPuzzleEnd ");
        endBook.SetActive(true);
        //show some chat
        //play sound
    }
}
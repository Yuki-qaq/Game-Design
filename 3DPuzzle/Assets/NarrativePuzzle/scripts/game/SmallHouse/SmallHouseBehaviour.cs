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

    private void Start()
    {
        hud_enterPuzzle.SetActive(false);
        hud_enterEndBook.SetActive(false);
        endBook.SetActive(false);
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
        //off player camera follow
        //off player control
        mainCamera.DOMove(puzzleCamera.position, durationTransitCamera).SetEase(Ease.InOutCubic);
        mainCamera.DORotate(puzzleCamera.eulerAngles, durationTransitCamera).SetEase(Ease.InOutCubic);
    }

    public void OnPuzzleEnd()
    {
        //on player camera follow
        //on player control
        Debug.Log("OnPuzzleEnd ");
        endBook.SetActive(true);
        //show some chat
        //play sound
    }
}
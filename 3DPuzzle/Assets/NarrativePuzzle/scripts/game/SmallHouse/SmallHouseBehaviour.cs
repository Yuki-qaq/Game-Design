using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SmallHouseBehaviour : MonoBehaviour
{
    public static SmallHouseBehaviour instance;

    public Transform mainCamera;
    public Transform puzzleCamera;
    public GameObject endBook;
    public GameObject hud_enterPuzzle;
    public GameObject hud_enterEndBook;
    public float durationTransitCamera;
    public FirstPersonController fpc;
    private Transform _mainCamera_defaultParent;
    public float bookShelfHeightDelta = 0.27f;

    public List<DraggableBook> books;
    private List<Vector3> _slots = new List<Vector3>();
    public float dragThresholdY;
    public float dragThresholdX;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        hud_enterPuzzle.SetActive(false);
        hud_enterEndBook.SetActive(false);
        endBook.SetActive(false);

        _mainCamera_defaultParent = mainCamera.parent;
    }

    public void InitBooks()
    {
        int i = 0;

        foreach (var book in books)
        {
            book.idealOrder = i;
            book.enabled = true;
            i++;
            _slots.Add(book.gameObject.transform.position - new Vector3(0, bookShelfHeightDelta, 0));
        }

        books.Sort(SortRandom);

        i = 0;
        foreach (var book in books)
        {
            book.gameObject.transform.position = _slots[i];
            i++;
        }
    }

    int SortRandom<T>(T a, T b)
    {
        return Random.value < 0.5 ? 1 : -1;
    }

    public void GetBookEndDragRes(DraggableBook book, out bool isFinal, out Vector3 endPos)
    {
        isFinal = false;
        endPos = book.startPos;
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
        Cursor.lockState = CursorLockMode.None;
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
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("OnPuzzleEnd ");
        endBook.SetActive(true);
        //show some chat
        //play sound
    }

    public void EnterBook()
    {
        Debug.Log("EnterBook ");
    }
}
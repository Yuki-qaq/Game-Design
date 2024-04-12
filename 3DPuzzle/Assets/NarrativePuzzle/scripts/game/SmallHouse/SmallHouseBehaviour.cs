using com;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSlot
{
    public Vector3 pos;
    public DraggableBook book;
}

public class SmallHouseBehaviour : MonoBehaviour
{
    public static SmallHouseBehaviour instance;

    public Transform mainCamera;
    public Transform puzzleCamera;
    public GameObject endBook;
    public GameObject hud_enterPuzzle;
    public GameObject triggerCol_enterPuzzle;
    public GameObject hud_enterEndBook;
    public float durationTransitCamera;
    public FirstPersonController fpc;
    private Transform _mainCamera_defaultParent;
    public float bookShelfHeightDelta = 0.27f;

    public List<DraggableBook> books;
    private List<BookSlot> _slots = new List<BookSlot>();
    public float dragDistThreshold = 0.3f;
    public List<DraggableBook> booksFinalAnim;

    public bool inPuzzle { get; private set; }

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
        inPuzzle = false;
    }

    public void InitBooks()
    {
        int i = 0;

        foreach (var book in books)
        {
            book.idealOrder = i;
            //book.enabled = true;
            i++;
            BookSlot bookSlot = new BookSlot();
            bookSlot.pos = book.gameObject.transform.position;
            _slots.Add(bookSlot);
        }

        foreach (var book in books)
        {
            BookSlot bookSlot = new BookSlot();
            bookSlot.pos = book.gameObject.transform.position - new Vector3(0, bookShelfHeightDelta, 0);
            _slots.Add(bookSlot);
        }

        books.Sort(SortRandom);

        i = 0;
        foreach (var book in books)
        {
            var slot = _slots[i];
            book.gameObject.transform.position = slot.pos;
            slot.book = book;

            book.Init();
            i++;
        }
    }

    int SortRandom<T>(T a, T b)
    {
        return (Random.value < 0.5) ? 1 : -1;
    }

    public void GetBookEndDragRes(DraggableBook book, out bool isFinal, out Vector3 endPos)
    {
        isFinal = false;
        BookSlot crtSlot = null;
        foreach (var ss in _slots)
        {
            if (ss.book == book)
            {
                crtSlot = ss;
                break;
            }
        }

        endPos = crtSlot.pos;

        float distMax = 100;
        BookSlot nearestBookSlot = null;
        foreach (var s in _slots)
        {
            var dist = Vector3.Distance(s.pos, book.transform.position);
            if (dist < distMax && dist < dragDistThreshold)
            {
                distMax = dist;
                nearestBookSlot = s;
            }
        }

        if (nearestBookSlot != null && nearestBookSlot.book == null)
        {
            //remove from old slot
            Debug.Log("remove from old slot");
            crtSlot.book = null;

            nearestBookSlot.book = book;
            Debug.Log("replace nearestBookSlot");
            endPos = nearestBookSlot.pos;
            isFinal = CheckFinalStrings(nearestBookSlot);
        }
    }

    bool CheckFinalStrings(BookSlot bs)
    {
        int correctNum = 0;
        for (int i = 10; i < 20; i++)
        {
            var slot = _slots[i];
            if (slot.book == null)
                return false;

            int idealNum = i - 10;
            //Debug.Log("i " + i + " ideaOrder " + slot.book.idealOrder);
            if (idealNum == slot.book.idealOrder)
            {
                correctNum++;
            }
            else
            {
                foreach (var ir in slot.book.idealOrderReplacement)
                {
                    if (idealNum == ir)
                    {
                        correctNum++;
                        break;
                    }
                }
            }

            Debug.Log("CheckFinalStrings correctNum " + correctNum);
        }

        return correctNum >= 10;
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
        triggerCol_enterPuzzle.SetActive(false);
        Debug.Log("EnterPuzzle ");
        Cursor.lockState = CursorLockMode.None;
        mainCamera.SetParent(null);
        //off player camera follow
        //off player control
        fpc.enabled = false;
        mainCamera.DOMove(puzzleCamera.position, durationTransitCamera).SetEase(Ease.InOutCubic).OnComplete(
            () =>
            { inPuzzle = true; }
            );
        mainCamera.DORotate(puzzleCamera.eulerAngles, durationTransitCamera).SetEase(Ease.InOutCubic);
    }

    public void OnPuzzleEnd()
    {
        //on player camera follow
        //on player control
        Debug.Log("OnPuzzleEnd ");
        endBook.SetActive(true);
        inPuzzle = false;
        //show some chat
        //play sound
        StartCoroutine(OnPuzzleEnd_Coroutine());
    }

    IEnumerator OnPuzzleEnd_Coroutine()
    {
        float extraTime = 1.1f;
        foreach (var b in booksFinalAnim)
        {
            b.enabled = true;
            b.transform.DOShakeRotation(0.5f + extraTime, 4, 10);
            yield return new WaitForSeconds(0.4f + extraTime);
            b.SetToFinalString();
            yield return new WaitForSeconds(0.1f);
            extraTime -= 0.4f;
            if (extraTime < 0)
                extraTime = 0;
            //SoundSystem.instance.Play("note");
        }

        yield return new WaitForSeconds(0.5f);
        mainCamera.SetParent(_mainCamera_defaultParent);
        mainCamera.localEulerAngles = Vector3.zero;
        mainCamera.localPosition = Vector3.zero;
        mainCamera.DOKill();
        fpc.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnterBook()
    {
        Debug.Log("EnterBook ");
    }
}
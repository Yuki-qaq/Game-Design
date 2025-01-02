using com;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DarkRoomBehaviour : MonoBehaviour
{
    public static DarkRoomBehaviour instance;

    public Transform mainCamera;
    public Transform puzzleCamera;
    public GameObject endBook;
    public GameObject hud_enterPuzzle;
    public GameObject triggerCol_enterPuzzle;
    public GameObject hud_enterEndBook;
    public float durationTransitCamera;
    public FirstPersonController fpc;
    private Transform _mainCamera_defaultParent;
    public RotatePuzzleBehaviour rpb;
    public bool inPuzzle { get; private set; }

    private void Awake()
    {
        instance = this;
        BookSceneSwitcher.notAGoodInstance.FadeFromWhite(null);
    }

    private void Start()
    {
        hud_enterPuzzle.SetActive(false);
        hud_enterEndBook.SetActive(false);
        endBook.SetActive(false);

        _mainCamera_defaultParent = mainCamera.parent;
        inPuzzle = false;


    }

    public void InitPuzzle()
    {
        fpc.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleWatchPuzzle(bool on)
    {
        Debug.Log("ToggleWatchPuzzle " + on);
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
        endBook.SetActive(true);
        inPuzzle = false;
        rpb.enabled = false;
        //show some chat
        //play sound
        StartCoroutine(OnPuzzleEnd_Coroutine());
    }

    IEnumerator OnPuzzleEnd_Coroutine()
    {
        crosshair.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        // mainCamera.SetParent(_mainCamera_defaultParent);
        // mainCamera.localEulerAngles = Vector3.zero;
        // mainCamera.localPosition = Vector3.zero;
        // mainCamera.DOKill();
        // fpc.enabled = true;
        // Cursor.lockState = CursorLockMode.Locked;

        PlayFinalCutScene();
    }

    public void EnterBook()
    {
        // inPuzzle = false;
        Debug.Log("EnterBook ");
        BookSceneSwitcher.Switch(() =>
        {
            fpc.enabled = false;
            BookController.instance.SwitchBook(BookController.instance.bookDatas[2]);
        });

        Cursor.lockState = CursorLockMode.None;
    }

    [SerializeField] Transform crosshair;
    [SerializeField] Transform[] heartTrans;
    [SerializeField] float[] heartTransDuration;

    [SerializeField] Transform[] camTrans;
    [SerializeField] float[] camTransDuration;

    [SerializeField] Transform fragmentedHeart;
    [SerializeField] Transform realHeart;
    [SerializeField] float realHeartScaleDuration = 1.5f;
    [SerializeField] float fragmentedHeartScaleDuration = 2f;
    [SerializeField] Transform shakeAltar;
    [SerializeField] float shakeStrength = 0.5f;
    [SerializeField] float commonDelay = 1.5f;
    public void PlayFinalCutScene()
    {
        StartCoroutine(HeartCo());
        StartCoroutine(CamCo());
    }

    IEnumerator HeartCo()
    {
        yield return new WaitForSeconds(commonDelay);
        shakeAltar.DOShakePosition(5.5f, shakeStrength, 50);

        yield return new WaitForSeconds(heartTransDuration[0]);
        fragmentedHeart.DOScale(0, fragmentedHeartScaleDuration).SetEase(Ease.InElastic);
        yield return new WaitForSeconds(fragmentedHeartScaleDuration - 0.4f);
        realHeart.position = heartTrans[0].position;
        realHeart.eulerAngles = heartTrans[0].eulerAngles;
        realHeart.localScale = Vector3.zero;
        realHeart.gameObject.SetActive(true);
        realHeart.DOScale(1, realHeartScaleDuration).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(realHeartScaleDuration + 0.5f);

        for (int i = 1; i < heartTrans.Length; i++)
        {
            var t = heartTrans[i];
            var d = heartTransDuration[i];
            realHeart.DOKill();
            realHeart.DOMove(t.position, d).SetEase(Ease.InOutQuad);
            realHeart.DORotate(t.eulerAngles, d).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(d);
        }

    }

    IEnumerator CamCo()
    {
        fpc.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        yield return new WaitForSeconds(commonDelay);
        mainCamera.SetParent(_mainCamera_defaultParent);
        mainCamera.position = camTrans[0].position;
        mainCamera.eulerAngles = camTrans[0].eulerAngles;
        yield return new WaitForSeconds(camTransDuration[0]);

        for (int i = 1; i < camTrans.Length; i++)
        {
            var t = camTrans[i];
            var d = camTransDuration[i];
            mainCamera.DOKill();
            mainCamera.DOMove(t.position, d).SetEase(Ease.InOutQuad);
            mainCamera.DORotate(t.eulerAngles, d).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(d);
        }
        yield return new WaitForSeconds(3f);
        finalCg.DOFade(1, 3);
    }

    [SerializeField] CanvasGroup finalCg;
}
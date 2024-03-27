using DG.Tweening;
using UnityEngine;

public class BookCamera : MonoBehaviour
{
    private Vector3 _startPos;
    private Quaternion _startRot;
    [SerializeField] private Transform _target;
    [SerializeField] private float duration;
    [SerializeField] private BookSceneSwitcher bookSceneSwitcher;

    private void Start()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
    }

    public void ResetToStart()
    {
        transform.DOKill();
        transform.position = _startPos;
        transform.rotation = _startRot;
    }

    public void FocusOnTargetPage()
    {
        transform.DOKill();
        transform.DOMove(_target.position, duration).SetEase(Ease.InOutCubic).SetDelay(0.7f);
        transform.DORotateQuaternion(_target.rotation, duration).SetEase(Ease.InOutCubic).SetDelay(1.5f).OnComplete(

            () => { bookSceneSwitcher.SwitchToHouseScene(); }
            );
    }
}
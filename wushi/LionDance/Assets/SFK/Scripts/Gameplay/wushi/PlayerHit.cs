using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public AnimationClipData clip;
    public PlayerActionPerformDependency dependency;

    private float _currentAnimEndTime;

    public GameObject hitSource;

    void Start()
    {
        _currentAnimEndTime = 0;
        hitSource.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
      //  if (Input.GetMouseButtonDown(2))
        //    TryHit();
        if(!isHiting)
            hitSource.SetActive(false);
    }

   public void TryHit()
    {
        if (!PlayerBehaviour.instance.AssertCondition(dependency))
            return;

        PlayerBehaviour.instance.animator.SetTrigger("hit");
        _currentAnimEndTime = clip.GetEndTime();
        hitSource.SetActive(true);
    }

    public bool isHiting
    {
        get
        {
            return Time.time < _currentAnimEndTime;
        }
    }
}

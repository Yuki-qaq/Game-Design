using UnityEngine;

[System.Serializable]
public struct AnimationClipData
{
    public AnimationClip clip;
    public float speedRatio;

    public float GetEndTime()
    {
        return Time.time + clip.length / speedRatio;
    }
}

[System.Serializable]
public struct PlayerActionPerformDependency
{
    [Header("check the conditions which is not allowed to perform this action")]
    public bool notWalking;
    public bool notHiting;
}
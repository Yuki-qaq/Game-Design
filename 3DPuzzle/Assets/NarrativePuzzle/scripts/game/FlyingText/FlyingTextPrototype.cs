using UnityEngine;

namespace Assets.Game.Scripts.com.FlyingText
{
    [CreateAssetMenu]
    public class FlyingTextPrototype : ScriptableObject
    {
        [Multiline]
        public string content;

        public Color color = Color.blue;

        [Header("1=normal,2=slow 2x")]
        public float timeRatio = 1;
        [Header("多少个字换行")]
        public int breakCharNum;

        public enum FromCorner
        {
            LT,
            LB,
            RT,
            RB,
        }
        [Header("来源的位置")]
        public FromCorner fromCorner;
        [Header("离开的位置")]
        [Tooltip("鼠标悬停的注释")]
        public FromCorner leaveCorner;
    }
}
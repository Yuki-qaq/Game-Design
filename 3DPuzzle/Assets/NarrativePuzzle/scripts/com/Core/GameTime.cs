using UnityEngine;

namespace com
{
    //用这个代替Unity自带的Time类，可以实现暂停部分时间
    //比如，游戏过程中让玩家选择一个强化，期间游戏中的子弹，敌人等等暂停
    //但是ui动画和特效等不暂停，update也会正常被调用
    //
    //用法
    //在脚本上面，加入一行 using com;
    //在各种逻辑中，用GameTime.time代替Time.time
    //在各种逻辑中，用GameTime.timeScale代替Time.timeScale
    //在各种逻辑中，用GameTime.deltaTime代替Time.deltaTime

    //暂停时，直接写GameTime.timeScale=0，
    //则用GameTime计算的时间逻辑全部暂停，
    //而Time计算的时间逻辑（包括unity自带的动画，粒子特效等）不会受影响
    public class GameTime : MonoBehaviour
    {
        private static float _timeScale = 1;
        private static float _timeValidated = 0;
        private static float _timeCached = 0;

        public static float timeScale
        {
            get
            {
                return _timeScale;
            }
            set
            {
                ValidateTime();
                if (value < 0)
                {
                    _timeScale = 0;
                }
                else
                {
                    _timeScale = value;
                }
            }
        }

        public static float time
        {
            get
            {
                ValidateTime();
                return _timeValidated;
            }
        }

        public static float deltaTime
        {
            get
            {
                return Time.deltaTime * _timeScale;
            }
        }

        private static void ValidateTime()
        {
            var delta = Time.time - _timeCached;
            _timeCached = Time.time;
            _timeValidated += delta * _timeScale;
        }
    }
}
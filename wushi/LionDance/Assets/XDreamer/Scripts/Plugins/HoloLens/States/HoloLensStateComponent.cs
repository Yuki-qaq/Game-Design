using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginHoloLens
{
    /// <summary>
    /// HoloLens检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Owner(typeof(HoloLensManager))]
    public abstract class HoloLensStateComponent<T> : StateComponent<T>
    where T : HoloLensStateComponent<T>
    {
        /// <summary>
        /// 输入监听器
        /// </summary>
        protected InputListener inputListener = null;

        /// <summary>
        /// 游戏对象
        /// </summary>
        protected virtual GameObject gameObj {get;} = null;

        /// <summary>
        /// 当进入之前
        /// </summary>
        /// <param name="data"></param>
        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);

            if (gameObj && !inputListener)
            {
                inputListener = CommonFun.GetOrAddComponent<InputListener>(gameObj);
            }
        }
    }
}

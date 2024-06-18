using System;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;

namespace XCSJ.Extension.Base.Helpers
{
    /// <summary>
    /// 对象辅助类
    /// </summary>
    public static class ObjectHelper
    {
        /// <summary>
        /// 尝试转为Unity对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unityObject"></param>
        /// <returns></returns>
        public static bool TryConvertToUnityObject(this string value, out UnityEngine.Object unityObject)
        {
            if(string.IsNullOrEmpty(value))
            {
                unityObject = default;
                return false;
            }

            //游戏对象
            unityObject = CommonFun.StringToGameObject(value);
            if (unityObject) return true;

            //组件
            unityObject = CommonFun.StringToGameObjectComponent(value);
            if (unityObject) return true;

            //状态
            unityObject = SMSHelper.StringToState(value);
            if (unityObject) return true;

            //状态组件
            unityObject = SMSHelper.StringToStateComponent(value);
            if (unityObject) return true;

            //跳转
            unityObject = SMSHelper.StringToTransition(value);
            if (unityObject) return true;

            //跳转组件
            unityObject = SMSHelper.StringToTransitionComponent(value);
            if (unityObject) return true;

            //状态组
            unityObject = SMSHelper.StringToStateGroup(value);
            if (unityObject) return true;

            //状态组组件
            unityObject = SMSHelper.StringToStateGroupComponent(value);
            if (unityObject) return true;

            //Unity资产对象
            unityObject = CommonFun.StringToUnityAssetObject(value);
            if (unityObject) return true;

            return false;
        }
    }
}

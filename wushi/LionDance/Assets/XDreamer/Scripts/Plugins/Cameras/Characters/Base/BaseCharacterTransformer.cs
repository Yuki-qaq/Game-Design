using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.PluginCameras.Characters.Base
{
    /// <summary>
    /// 基础角色变换器
    /// </summary>
    [Name("基础角色变换器")]
    public abstract class BaseCharacterTransformer : BaseCharacterCoreController
    {
        /// <summary>
        /// 计算期望的矢量速度
        /// </summary>
        /// <returns></returns>
        public abstract Vector3 CalcDesiredVelocity();

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        /// <param name="sender"></param>
        public abstract void Move(Vector3 value, int moveMode, BaseCharacterTransformInputController sender);

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        /// <param name="sender"></param>
        public abstract void Rotate(Vector3 value, int rotateMode, BaseCharacterTransformInputController sender);
    }

    /// <summary>
    /// 角色变换输入控制器
    /// </summary>
    public interface ICharacterTransformInputController
    {

    }

    /// <summary>
    /// 基础角色变换输入控制器
    /// </summary>
    public abstract class BaseCharacterTransformInputController: BaseCharacterToolController, ICharacterTransformInputController
    {

    }
}

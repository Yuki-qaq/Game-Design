using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;

namespace XCSJ.PluginCameras.Characters
{
    /// <summary>
    /// 自定义参数枚举;在Animator中已经自定义的参数名枚举
    /// </summary>
    [Name("自定义参数枚举")]
    public enum ECustomParameter
    {
        /// <summary>
        /// 前进
        /// </summary>
        [Name("前进")]
        [Tip("用于控制角色的移动；类型：Float", "Used to control the movement of characters; Type: float")]
        Forward,

        /// <summary>
        /// 转向
        /// </summary>
        [Name("转向")]
        [Tip("用于控制角色的旋转；类型：Float", "Used to control the rotation of characters; Type: float")]
        Turn,

        /// <summary>
        /// 下蹲
        /// </summary>
        [Name("下蹲")]
        [Tip("用于控制角色的下蹲；类型：Bool", "Used to control the character's squat; Type: bool")]
        Crouch,

        /// <summary>
        /// 地面上
        /// </summary>
        [Name("地面上")]
        [Tip("控制角色是否在地面；类型：Bool", "Control whether the character is on the ground; Type: bool")]
        OnGround,

        /// <summary>
        /// 跳跃
        /// </summary>
        [Name("跳跃")]
        [Tip("控制角色是否跳跃；类型：Float", "Control whether the character jumps; Type: float")]
        Jump,

        /// <summary>
        /// 跳跃腿
        /// </summary>
        [Name("跳跃腿")]
        [Tip("控制角色的腿；类型：Float", "Control the character's legs; Type: float")]
        JumpLeg
    }
}

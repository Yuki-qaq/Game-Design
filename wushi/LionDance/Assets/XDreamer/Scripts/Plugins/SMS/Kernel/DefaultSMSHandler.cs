using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.PluginSMS.Kernel
{
    /// <summary>
    /// 默认SMS处理器
    /// </summary>
    public class DefaultSMSHandler : InstanceClass<DefaultSMSHandler>, ISMSHandler
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            SMSHandler.handler = instance;
        }
    }
}

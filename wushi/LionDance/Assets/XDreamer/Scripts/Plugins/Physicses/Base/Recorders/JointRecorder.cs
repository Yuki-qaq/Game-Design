using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPhysicses.Base.Recorders
{
    /// <summary>
    /// 关节记录器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRecord"></typeparam>
    public abstract class JointRecorder<T, TRecord> : Recorder<T, TRecord> where T : Joint
        where TRecord : class, ISingleRecord<T>, new()
    {

    }
}

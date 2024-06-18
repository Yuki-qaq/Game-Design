using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Helper;

namespace XCSJ.PluginMMO.Base
{
    /// <summary>
    /// MMO事件类型
    /// </summary>
    [Name("MMO事件类型")]
    public enum EMMOEventType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 当MMO将要启动:当对象将在MMO网络环境中生效时回调
        /// </summary>
        [Name("当MMO将要启动")]
        OnMMOWillStart,

        /// <summary>
        /// 当MMO启动完成
        /// </summary>
        [Name("当MMO启动完成")]
        OnMMOStartCompleted,

        /// <summary>
        /// 当MMO已停止:当对象已在MMO网络环境中失效时回调
        /// </summary>
        [Name("当MMO已停止")]
        OnMMOStoped,

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        [Name("当MMO进入房间完成")]
        OnMMOEnterRoomCompleted,

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        [Name("当MMO退出房间完成")]
        OnMMOExitRoomCompleted,

        /// <summary>
        /// 当MMO房间增加用户
        /// </summary>
        [Name("当MMO房间增加用户")]
        OnMMORoomAddUser,

        /// <summary>
        /// 当MMO房间移除用户
        /// </summary>
        [Name("当MMO房间移除用户")]
        OnMMORoomRemoveUser,

        /// <summary>
        /// 当MMO房间增加玩家
        /// </summary>
        [Name("当MMO房间增加玩家")]
        OnMMORoomAddPlayer,

        /// <summary>
        /// 当MMO房间移除玩家
        /// </summary>
        [Name("当MMO房间移除玩家")]
        OnMMORoomRemovePlayer,

        /// <summary>
        /// 当MMO已克隆作为新对象：当对象在网络环境中MMO对象克隆时回调；如果对象调用本函数，说明当前对象是运行时被动态克隆的；
        /// </summary>
        [Name("当MMO已克隆作为新对象")]
        OnMMOClonedAsNew,

        /// <summary>
        /// 当MMO已克隆作为模版：当对象在网络环境中MMO对象被作为模版克隆时回调；如果对象调用本函数，说明当前对象是运行时被作为模版被动态克隆了新的对象；
        /// </summary>
        [Name("当MMO已克隆作为模版")]
        OnMMOClonedAsTemplate,

        /// <summary>
        /// 当MMO将要销毁：当对象在网络环境中MMO对象将销毁时回调；如果对象调用本函数，说明当前对象是运行时被动态克隆的；
        /// </summary>
        [Name("当MMO将要销毁")]
        OnMMOWillDestroy,

        /// <summary>
        /// 当MMO启动玩家关联：当前对象与网络环境中的玩家产生关联时回调
        /// </summary>
        [Name("当MMO启动玩家关联")]
        OnMMOStartPlayerLink,

        /// <summary>
        /// 当MMO停止玩家关联：当前对象与网络环境中的玩家解除关联时回调
        /// </summary>
        [Name("当MMO停止玩家关联")]
        OnMMOStopPlayerLink,

        /// <summary>
        /// 当MMO启动控制权限：本地用户对当前对象开始具有控制权限时回调
        /// </summary>
        [Name("当MMO启动控制权限")]
        OnMMOStartControlAccess,

        /// <summary>
        /// 当MMO停止控制权限：本地用户对当前对象停止具有控制权限时回调
        /// </summary>
        [Name("当MMO停止控制权限")]
        OnMMOStopControlAccess,
    }

    /// <summary>
    /// 确保同步缓存：确保MMO对象的待发送数据成功进行MMO网络同步的缓存类；
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class EnsureSyncBuffer<TData> where TData : class
    {
        /// <summary>
        /// 待发送缓存
        /// </summary>
        private List<TData> waitSend = new List<TData>();

        /// <summary>
        /// 待接收缓存
        /// </summary>
        private Dictionary<int, List<TData>> waitReceive = new Dictionary<int, List<TData>>();

        /// <summary>
        /// 设置待接收
        /// </summary>
        /// <param name="datas"></param>
        public void SetWaitReceive(List<TData> datas)
        {
            var version = mmoObject.version;
            if (!waitReceive.TryGetValue(version, out List<TData> list))
            {
                waitReceive[version] = list = new List<TData>();
            }
            //Log.DebugFormat("等待{0}数据", version);
            list.AddRange(datas);
        }

        /// <summary>
        /// MMO对象
        /// </summary>
        public IMMOObject mmoObject { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="mmoObject"></param>
        public EnsureSyncBuffer(IMMOObject mmoObject)
        {
            this.mmoObject = mmoObject ?? throw new ArgumentNullException(nameof(mmoObject));
        }

        /// <summary>
        /// 有待发送的：判断是否有待发送的数据
        /// </summary>
        /// <returns></returns>
        public bool HasWaitSend()
        {
            foreach (var kv in waitReceive)
            {
                if (mmoObject.version > kv.Key)
                {
                    //Log.DebugFormat("重发{0}数据{1}", kv.Key, waitReceive.Count);
                    waitSend.AddRange(kv.Value);
                    waitReceive.Remove(kv.Key);
                    break;
                }
            }
            return waitSend.Count > 0;
        }

        /// <summary>
        /// 发送：将数据加入待发送缓存
        /// </summary>
        /// <param name="data"></param>
        public void Send(TData data) => waitSend.Add(data);

        /// <summary>
        /// 获取发送：将待发送缓存中所有数据转为JSON字符串、并加入待接收缓存；之后将待发送缓存清空；
        /// </summary>
        /// <returns></returns>
        public string GetSend()
        {
            try
            {
                return JsonHelper.ToJson(waitSend);
            }
            finally
            {
                SetWaitReceive(waitSend);
                waitSend.Clear();
            }
        }

        /// <summary>
        /// 当接收：如果是当前（本地）用户发送的数据，那么会对待接收缓存做处理，并回调已成功同步的数据；
        /// </summary>
        /// <param name="dataObj">数据对象</param>
        /// <param name="onSyncSuccessed">当同步成功</param>
        /// <returns>如果数据是当前（本地）用户并且是指定MMO对象刚刚发送并再次接收的数据，则返回True；否则返回False;</returns>
        public bool OnReceive(Data dataObj, Action<List<TData>> onSyncSuccessed)
        {
            if (mmoObject.IsLocalUserSended(dataObj))
            {
                var version = mmoObject.version;
                //Log.DebugFormat("收到{0}数据", version);
                if (waitReceive.TryGetValue(version, out var list))
                {
                    waitReceive.Remove(version);
                    onSyncSuccessed?.Invoke(list);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            waitSend.Clear();
            waitReceive.Clear();
        }
    }
}

using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.Net;
using XCSJ.Net.Tcp;
using XCSJ.Net.Tcp.Threading;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Net
{
    /// <summary>
    /// 跨平台TCP客户端接口
    /// </summary>
    public interface ICrossPlatformTcpClient : IBaseClient, IAsyncClient { }

    /// <summary>
    /// 跨平台TCP客户端
    /// </summary>
    public class CrossPlatformTcpClient
#if UNITY_WEBGL
        : WebSocketClient, ICrossPlatformTcpClient
#else
        : TcpClientEntityThread<CrossPlatformTcpClient, QuestionNetPackage, AnswerNetPackage>
        , ICrossPlatformTcpClient
#endif
    {
        
        /// <summary>
        /// 构造
        /// </summary>
        public CrossPlatformTcpClient() { }

#if UNITY_WEBGL

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="path"></param>
        /// <param name="onConnect"></param>
        /// <returns></returns>
        public bool ConnectAsync(IAddress serverAddress, string path, Action<IAsyncClient> onConnect)
        {
            if (serverAddress == null) return false;
            this.path = path;
            serverAddress = new Address(serverAddress.address, NetHelper.ToWSPort(serverAddress.port));
            var result = base.Connect(serverAddress);
            StartCheckConnection(()=> onConnect?.Invoke(this));
            return true;
        }

        /// <summary>
        /// 无延时
        /// </summary>
        public bool noDelay { get; set; } = false;

        /// <summary>
        /// 写入超时时间，单位毫秒
        /// </summary>
        public int writeTimeout { get; set; } = 0;

        /// <summary>
        /// 读取超时时间，单位毫秒
        /// </summary>
        public int readTimeout { get; set; } = 0;

        /// <summary>
        /// 启动检查连接性
        /// </summary>
        /// <param name="onConnected"></param>
        private void StartCheckConnection(Action onConnected)
        {
            //异步检查连接性
            GlobalMB.instance.StartCoroutine(CheckConnection(onConnected));
        }

        private IEnumerator CheckConnection(Action onConnected)
        {
            while (IsConnected())
            {
                yield return null;
                if (sock == null)//可能外部调用了关闭功能
                {
                    break;
                }
                if (sock.Connected)
                {
                    break;
                }
                if (clientState != EClientState.Connecting)
                {
                    break;
                }
            }
            onConnected?.Invoke();
        }        
        
        private void OnReceived(AnswerNetPackage answerNetPackage)
        {
            //Debug.Log("CrossPlatformTcpClient.WebGL OnReceived:" + answerNetPackage.ToJson());
            onAsyncReceived?.Invoke(answerNetPackage);
        }

        /// <summary>
        /// 有效异步模式
        /// </summary>
        public bool validAsyncMode => true;

        /// <summary>
        /// 当接收到数据时回调
        /// </summary>

        public event Action<NetPackage> onAsyncReceived;

        private object sendLocker = new object();

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public bool Send(NetPackage package)
        {
            lock (sendLocker)
            {
                return base.Write(package.ToJson()) > 0;
            }
        }

#else

        private Action onExit;

        /// <summary>
        /// 当关闭
        /// </summary>
        protected override void OnClose()
        {
            base.OnClose();
            onExit?.Invoke();
        }

        Action<AnswerNetPackage> onRead;

        /// <summary>
        /// 当接收答案
        /// </summary>
        /// <param name="answer"></param>
        protected override void OnReceivedAnswer(AnswerNetPackage answer)
        {
            base.OnReceivedAnswer(answer);
            onRead?.Invoke(answer);
        }

        Action<object> onHasError;

        /// <summary>
        /// 当错误
        /// </summary>
        /// <param name="o"></param>
        protected override void OnError(object o)
        {
            base.OnError(o);
            onHasError?.Invoke(o);
        }

#endif

        /// <summary>
        /// 开始读取答案
        /// </summary>
        /// <param name="onRead"></param>
        /// <param name="onExit"></param>
        /// <param name="onError"></param>
        public void StartReadAnswer(Action<AnswerNetPackage> onRead, Action onExit, Action<object> onError)
        {
            StopReadAnswer(onRead, onExit, onError);

#if UNITY_WEBGL

            CommonFun.DelayCall(() =>
            {
                CommonFun.StartReadAsync<AnswerNetPackage>(this, (c, a) =>
                {
                    onRead?.Invoke(a);
                    OnReceived(a);
                }, (c) =>
                {
                    onExit?.Invoke();
                }, (c, e) =>
                {
                    onError?.Invoke(e);
                });
            });            

#else
            this.onRead +=  onRead;
            this.onExit += onExit;
            this.onHasError += onError;
#endif
        }

        /// <summary>
        /// 停止读取答案
        /// </summary>
        /// <param name="onRead"></param>
        /// <param name="onExit"></param>
        /// <param name="onError"></param>
        public void StopReadAnswer(Action<AnswerNetPackage> onRead, Action onExit, Action<object> onError)
        {
#if UNITY_WEBGL
#else
            this.onRead -= onRead;
            this.onExit -= onExit;
            this.onHasError -= onError;
#endif
        }
    }


    /// <summary>
    /// 连接模式
    /// </summary>
    [Name("连接模式")]
    public enum EConnectMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 同步
        /// </summary>
        [Name("同步")]
        Sync,

        /// <summary>
        /// 异步
        /// </summary>
        [Name("异步")]
        Async,
    }

    /// <summary>
    /// 连接服务器配置
    /// </summary>
    [Serializable]
    [Name("连接服务器配置")]
    public class ConnectServerConfig : IAddress, IToFriendlyString
    {
        /// <summary>
        /// 地址
        /// </summary>
        [Name("地址")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _address = Product.WebSite;

        /// <summary>
        /// 地址
        /// </summary>
        public string address { get => _address; set => _address = value; }

        /// <summary>
        /// 端口
        /// </summary>
        [Name("端口")]
        [ValidityCheck(EValidityCheckType.NotZero)]
        public int _port = 0;


        /// <summary>
        /// 端口
        /// </summary>
        public int port { get => _port; set => _port = value; }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString() => Address.ToString(address, port);
    }

    /// <summary>
    /// 带模式的连接服务器配置
    /// </summary>
    [Serializable]
    [Name("带模式的连接服务器配置")]
    public class ConnectServerConfigWithMode : ConnectServerConfig
    {
        /// <summary>
        /// 连接模式
        /// </summary>
        [Name("连接模式")]
        [EnumPopup]
        [Readonly]
        public EConnectMode _connectMode = EConnectMode.Async;

        /// <summary>
        /// 网络问答模式
        /// </summary>
        [Name("网络问答模式")]
        [EnumPopup]
        [Readonly]
        public ENetQAMode _netQAMode = ENetQAMode.T1QT2A;
    }

    /// <summary>
    /// 处理客户端配置
    /// </summary>
    [Serializable]
    [Name("处理客户端配置")]
    public class HandleClientConfig
    {
        /// <summary>
        /// 无延时
        /// </summary>
        [Name("无延时")]
        public bool _noDealy = true;

        /// <summary>
        /// 写入超时时间:写入超过本时间网络将断开；单位：毫秒；
        /// </summary>
        [Name("写入超时时间")]
        [Tip("写入超过本时间网络将断开；单位：毫秒；", "The network will be disconnected when the write exceeds this time; Unit: milliseconds;")]
        [ValidityCheck(EValidityCheckType.GreaterEqual, 3000)]
        public int _writeTimeout = 300 * 1000;

        /// <summary>
        /// 写入超时时间:写入超过本时间网络将断开；单位：毫秒；本值不会低于<see cref="NetHelper.DefaultTimeoutOfClient"/>值；
        /// </summary>
        public int writeTimeout => Mathf.Max(_writeTimeout, NetHelper.DefaultTimeoutOfClient);

        /// <summary>
        /// 读取超时时间:读取超过本时间网络将断开；单位：毫秒；
        /// </summary>
        [Name("读取超时时间")]
        [Tip("读取超过本时间网络将断开；单位：毫秒；", "After reading this time, the network will be disconnected; Unit: milliseconds;")]
        [ValidityCheck(EValidityCheckType.GreaterEqual, 3000)]
        public int _readTimeout = 300 * 1000;

        /// <summary>
        /// 读取超时时间:读取超过本时间网络将断开；单位：毫秒；本值不会低于<see cref="NetHelper.DefaultTimeoutOfClient"/>值；
        /// </summary>
        public int readTimeout => Mathf.Max(_readTimeout, NetHelper.DefaultTimeoutOfClient);
    }
}

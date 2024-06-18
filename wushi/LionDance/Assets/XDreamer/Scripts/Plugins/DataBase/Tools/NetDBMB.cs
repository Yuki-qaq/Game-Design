using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Net;
using XCSJ.Helper;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginDataBase.Tools
{
    /// <summary>
    /// 网络数据库:可跨平台使用，需要与[XDreamer数据库服务]应用程序配合;
    /// </summary>
    [Name("网络数据库")]
    [Tip("可跨平台使用，需要与[XDreamer数据库服务]应用程序配合;", "It can be used across platforms and needs to cooperate with [xdreamer database service] application;")]
    public sealed class NetDBMB : BaseNetDBMB
    {
        /// <summary>
        /// 心跳间隔时间
        /// </summary>
        [Name("心跳间隔时间")]
        [Range(1, 60)]
        public float _heartbeatIntervalTime = 10;

        private float heartbeatCounter =0;

        private DBClient _client = new DBClient();

        /// <summary>
        /// 客户端网络包
        /// </summary>
        public override IClientNetPackage client => _client;

        /// <summary>
        /// 在连接中
        /// </summary>
        public override bool inConnecting => _client.clientState == EClientState.Connecting;

        private NetDB _netDB = null;

        /// <summary>
        /// 网络数据库
        /// </summary>
        public override NetDB netDB
        {
            get
            {
                if (_netDB == null)
                {
                    _client.noDelay = _handleClientConfig._noDealy;
                    _client.readTimeout = _handleClientConfig.readTimeout;
                    _client.writeTimeout = _handleClientConfig.writeTimeout;
                    _netDB = new NetDB(_client, _userInfo);
                }
                return _netDB;
            }
            protected set => _netDB = value;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            serverPort= DBHelper.DefaultPort;
            connectMode = EConnectMode.Async;
        }

        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            if (netDB.IsConnected() && (heartbeatCounter += Time.deltaTime) >= _heartbeatIntervalTime)
            {
                heartbeatCounter = 0;
                _client.Send(_client.heartHeadQ);
            }
        }

        bool inConnected = false;

        private void OnConnect()
        {
            //Debug.Log("OnConnect:" + netDB.IsConnected());
            //Debug.Log("OnConnect 1:" + netDB.validAsyncMode);
            if (!netDB.IsConnected()) return;

            //连接成功开始读取答案
            _client.StartReadAnswer(OnReadAnswer, OnExit, OnError);
            inConnected = true;
        }

        private void OnError(object e)
        {
            if(_outputErrorInfo) Log.Error(e);
        }

        private void OnExit()
        {
            if (!inConnected) return;
            inConnected = false;

            //Log.Debug("OnExit 0");
            //退出时停止读取答案
            _client.StopReadAnswer(OnReadAnswer, OnExit, OnError);

            //禁用组件
            CommonFun.DelayCall(() =>
            {
                if (!inConnecting && !netDB.IsConnected())
                {
                    //Log.Debug("OnExit 1:" + enabled);
                    //Log.Debug("OnExit 2:" + inConnecting);
                    enabled = false;
                }
            });
        }

        private void OnReadAnswer(AnswerNetPackage answerNetPackage)
        {
            if (answerNetPackage is HeartHeatA heartHeatA)
            {
                //Debug.Log("HeartHeatA");
                _client.HandleHeartHeatA(heartHeatA);
            }
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns></returns>
        protected override bool ConnectDB()
        {
            if (IsConnected()) return true;
            if (inConnecting) return true;

            try
            {
                return netDB.Connect(serverAddress, serverPort, DBHelperExtension.WSPath, OnConnect);
            }
            catch (Exception ex)
            {
                ex.HandleException(nameof(ConnectDB));
            }
            return false;
        }
    }

    /// <summary>
    /// DB客户端
    /// </summary>
    public class DBClient : CrossPlatformTcpClient { }

    /// <summary>
    /// 网络客户端类型
    /// </summary>
    [Name("网络客户端类型")]
    public enum ENetClientType
    {
        /// <summary>
        /// XTcp:使用XDreamer Tcp客户端方式请求网络数据；需要启动XDreamer网络数据服务；仅支持部分平台；
        /// </summary>
        [Name("XTcp")]
        [Tip("使用XDreamer Tcp客户端方式请求网络数据；需要启动XDreamer网络数据服务；仅支持部分平台；", "Use XDreamer TCP client mode to request network data; XDreamer network data service needs to be started; Only some platforms are supported;")]
        XTcp = 0,

        /// <summary>
        /// XTcpS:使用XDreamer Tcp安全客户端方式请求网络数据；需要启动XDreamer网络数据安全服务；仅支持部分平台；
        /// </summary>
        [Name("XTcpS")]
        [Tip("使用XDreamer Tcp安全客户端方式请求网络数据；需要启动XDreamer网络数据安全服务；仅支持部分平台；", "Use XDreamer TCP security client mode to request network data; XDreamer network data security service needs to be started; Only some platforms are supported;")]
        XTcpS,
    }
}

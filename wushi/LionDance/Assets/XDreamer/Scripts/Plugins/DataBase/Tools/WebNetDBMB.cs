using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Extension.Base.Net;
using XCSJ.Helper;
using XCSJ.Net;
using XCSJ.Net.Http;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginDataBase.Tools
{
    /// <summary>
    /// Web网络数据库:Web网络数据库通过Http协议提交网络数据;可跨平台使用,同时需要WebServer支持对应的处理,并遵循特殊的格式;
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [Name("Web网络数据库")]
    [Tip("Web网络数据库通过Http协议提交网络数据;可跨平台使用,同时需要WebServer支持对应的处理,并遵循特殊的格式;", "Web network database submits network data through HTTP protocol; It can be used across platforms. At the same time, webserver needs to support corresponding processing and follow special formats;")]
    public sealed class WebNetDBMB : BaseNetDBMB
    {
        /// <summary>
        /// 客户端
        /// </summary>
        private UnityWebRequestClient _client = new UnityWebRequestClient();

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
            serverPort = HttpHelper.DefaultPort;
            connectMode = EConnectMode.Async;
        }

        bool inConnected = false;

        private void OnConnect()
        {
            //Debug.Log("Connected");
            //Debug.Log("Connected 1:"+ netDB.validAsyncMode);
            if (!netDB.IsConnected()) return;

            //连接成功开始读取答案
            _client.StartReadAnswer(OnReadAnswer, OnExit, OnError);
            inConnected = true;
        }

        private void OnError(object e)
        {
            if (_outputErrorInfo) Log.Error(e);
        }

        private void OnExit()
        {
            if (!inConnected) return;
            inConnected = false;

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

        private void OnReadAnswer(AnswerNetPackage answerNetPackage) { }

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
                return netDB.Connect(serverAddress, serverPort, "", OnConnect);
            }
            catch (Exception ex)
            {
                ex.HandleException(nameof(ConnectDB));
            }
            return false;
        }
    }
}

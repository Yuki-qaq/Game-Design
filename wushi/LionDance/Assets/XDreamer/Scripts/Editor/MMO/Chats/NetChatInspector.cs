using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;
using XCSJ.PluginMMO.Chats;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.EditorMMO.Chats
{
    /// <summary>
    /// 网络聊天检查器
    /// </summary>
    [Name("网络聊天检查器")]
    [CustomEditor(typeof(NetChat), true)]
    public class NetChatInspector : NetMBInspector<NetChat>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, xcl => EditorToolsHelper.GetWithPurposes(nameof(NetChat)));

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// 消息文本
        /// </summary>
        [Name("消息文本")]
        public string messageText = "";

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [Languages.LanguageTuple("Send", "发送")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(TrLabel(nameof(messageText)));
            messageText = EditorGUILayout.TextField(messageText);
            if (GUILayout.Button(Tr("Send"), UICommonOption.Width80) && MMOHelper.isEnteredRoom)
            {
                mb.Send(messageText);
            }
            EditorGUILayout.EndHorizontal();

            categoryList.obj.DrawVertical();
        }
    }
}

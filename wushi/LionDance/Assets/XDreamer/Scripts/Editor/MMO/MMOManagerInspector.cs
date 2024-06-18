using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;
using XCSJ.PluginMMO.Tools;

namespace XCSJ.EditorMMO
{
    /// <summary>
    /// 多人在线MMO检查器
    /// </summary>
    [Name("多人在线MMO检查器")]
    [CustomEditor(typeof(MMOManager))]
    public class MMOManagerInspector : BaseManagerInspector<MMOManager>
    {
        /// <summary>
        /// 聊天
        /// </summary>
        static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, xcl => EditorToolsHelper.GetWithPurposes(MMOHelper.ToolPurpose));

        #region Unity 消息

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            manager.XGetOrAddComponent<MMOProvider>();
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawDetailInfos();

            categoryList.obj.DrawVertical();
        }

        #endregion

        #region 网络标识列表

        /// <summary>
        /// 网络标识列表
        /// </summary>
        [Name("网络标识列表")]
        [Tip("当前场景中所有的网络标识对象", "All network identification objects in the current scene")]
        public bool netIdentities = true;

        /// <summary>
        /// 网络标识
        /// </summary>
        [Name("网络标识")]
        [Tip("网络标识所在的游戏对象；本项只读；", "The game object where the network logo is located; This item is read-only;")]
        public bool netIdentity;

        /// <summary>
        /// 激活且可用
        /// </summary>
        [Name("激活且可用")]
        [Tip("网络标识所在的游戏对象是激活可用的；本项只读；", "The game object where the network logo is located; This item is read-only;")]
        public bool isActiveAndEnabled;

        /// <summary>
        /// 本地权限
        /// </summary>
        [Name("本地权限")]
        [Tip("详细解释请查看网络标识对象上对应属性的具体解释；本项只读；", "For detailed explanation, please check the specific explanation of the corresponding attribute on the network identification object; This item is read-only;")]
        public bool localAccess;

        /// <summary>
        /// 权限
        /// </summary>
        [Name("权限")]
        [Tip("详细解释请查看网络标识对象上对应属性的具体解释；本项只读；", "For detailed explanation, please check the specific explanation of the corresponding attribute on the network identification object; This item is read-only;")]
        public bool access;

        /// <summary>
        /// MMO对象唯一编号
        /// </summary>
        [Name("MMO对象唯一编号")]
        public static string mmoObjectGuid = "";

        [LanguageTuple("MMO object corresponding to unique number [{0}] not found!", "未找到唯一编号[{0}]对应的MMO对象！")]
        private void DrawMMOObjectFinder()
        {
            EditorGUILayout.BeginHorizontal();
            mmoObjectGuid = EditorGUILayout.TextField(TrLabel(nameof(mmoObjectGuid)), mmoObjectGuid);
            var ping = GUILayout.Button("Ping", GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            if (!ping || string.IsNullOrEmpty(mmoObjectGuid)) return;

            //1、找网络标识
            var netIdentity = MMOHelper.GetNetIdentity(mmoObjectGuid);
            if (netIdentity)
            {
                UICommonFun.PingObject(netIdentity);
                Debug.Log(netIdentity.ObjectToString());
                return;
            }

            //2、找网络标识中已缓存的MMO对象
            foreach (var kv in MMOHelper.allNetIdentities)
            {
                var mmoObject = kv.Value.GetMMOObject(mmoObjectGuid);
                if (mmoObject != null)
                {
                    if (mmoObject is UnityEngine.Object unityObject)
                    {
                        UICommonFun.PingObject(unityObject);
                    }
                    Debug.Log(mmoObject.ObjectToString());
                    return;
                }
            }

            //3、找网络标识中未缓存的MMO对象
            foreach (var kv in MMOHelper.allNetIdentities)
            {
                foreach (var mmoObject in kv.Value.GetMMOObjectsOfComponentNoCache())
                {
                    if (mmoObject != null)
                    {
                        if (mmoObject is UnityEngine.Object unityObject)
                        {
                            UICommonFun.PingObject(unityObject);
                        }
                        Debug.LogError(mmoObject.ObjectToString());
                        return;
                    }
                }
            }

            Debug.LogWarningFormat(Tr("MMO object corresponding to unique number [{0}] not found!"), mmoObjectGuid);
        }

        private void DrawDetailInfos()
        {
            netIdentities = UICommonFun.Foldout(netIdentities, TrLabel(nameof(netIdentities)));
            if (!netIdentities) return;

            CommonFun.BeginLayout();

            DrawMMOObjectFinder();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(netIdentity)));
            GUILayout.Label(TrLabel(nameof(isActiveAndEnabled)), UICommonOption.Width60);
            GUILayout.Label(TrLabel(nameof(localAccess)), UICommonOption.Width60);
            GUILayout.Label(TrLabel(nameof(access)), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(NetIdentity), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as NetIdentity;
                if (!component) continue;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //网络标识
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //激活且可用
                EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                //本地权限
                EditorGUILayout.Toggle(component.localAccess, UICommonOption.Width60);

                //权限
                EditorGUILayout.Toggle(component.access, UICommonOption.Width60);

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        #endregion
    }
}

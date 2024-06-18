using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;

namespace XCSJ.EditorMMO
{
    /// <summary>
    /// 网络标识检查器
    /// </summary>
    [Name("网络标识检查器")]
    [CustomEditor(typeof(NetIdentity))]
    public class NetIdentityInspector : MMOMBInspector<NetIdentity>
    {
        static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, xcl => EditorToolsHelper.GetWithPurposes(nameof(NetIdentity)));

        /// <summary>
        /// 显示MMO对象
        /// </summary>
        [Name("显示MMO对象")]
        public bool displayMMOObjects = true;

        [LanguageTuple("MMO Object", "MMO对象")]
        [LanguageTuple("GUID", "唯一编号")]
        [LanguageTuple("Operation", "操作")]
        [LanguageTuple("Sync", "同步")]
        private void DrawMMOObjects()
        {
            displayMMOObjects = UICommonFun.Foldout(displayMMOObjects, TrLabel(nameof(displayMMOObjects)), () =>
            {
                if(GUILayout.Button(UICommonOption.Update, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                {
                    mb.ClearMMOObjectsCache();
                }
            });
            if (!displayMMOObjects) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(Tr("MMO Object"));
            GUILayout.Label(Tr("GUID"), GUILayout.Width(300));
            GUILayout.Label(Tr("Operation"), UICommonOption.Width64);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var mmoObjects = mb.mmoObjects;
            var count = mmoObjects.Length;
            for (int i = 0; i < count; i++)
            {
                var obj = mmoObjects[i];
                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //MMO对象
                var unityObject = obj as UnityEngine.Object;
                if (unityObject)
                {
                    EditorGUILayout.ObjectField(unityObject, obj.GetType(), true);
                }
                else
                {
                    EditorGUILayout.TextField(obj.ObjectToString());
                }

                //唯一编号
                EditorGUILayout.TextField(obj.guid, GUILayout.Width(300));

                //操作
                if (GUILayout.Button(Tr("Sync"), UICommonOption.Width32))
                {
                    obj.MarkDirty();
                }
                if (GUILayout.Button("Ping", UICommonOption.Width32))
                {
                    UICommonFun.PingObject(unityObject);
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawMMOObjects();

            categoryList.obj.DrawVertical();
        }

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            XDreamerEvents.onSceneAnyAssetsChanged += OnSceneAnyAssetsChanged;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            XDreamerEvents.onSceneAnyAssetsChanged -= OnSceneAnyAssetsChanged;
        }

        private void OnSceneAnyAssetsChanged()
        {
            if (mb) mb.ClearMMOObjectsCache();
            Repaint();
        }
    }
}

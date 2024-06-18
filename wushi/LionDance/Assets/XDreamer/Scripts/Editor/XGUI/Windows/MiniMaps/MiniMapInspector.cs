using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Windows.MiniMaps;
using static XCSJ.PluginXGUI.Windows.MiniMaps.MiniMapCameraController;

namespace XCSJ.EditorXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 导航图检查器
    /// </summary>
    [Name("导航图检查器")]
    [CustomEditor(typeof(MiniMap))]
    public class MiniMapInspector : InteractorInspector<MiniMap>
    {
        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [LanguageTuple("MiniMap Init Size", "导航图初始大小")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(MiniMap._miniMapCameraCotroller):
                    {
                        base.OnDrawMember(serializedProperty, propertyData);

                        var camController = targetObject._miniMapCameraCotroller;
                        if (camController && !Application.isPlaying)
                        {
                            // 导航图跟随相机或玩家旋转设置
                            EditorGUI.BeginChangeCheck();
                            var follow = UICommonFun.EnumPopup(CommonFun.NameTip(typeof(MiniMapCameraController), nameof(MiniMapCameraController._followPlayerMode)), camController._followPlayerMode);
                            if (EditorGUI.EndChangeCheck())
                            {
                                var followMode = (EFollowPlayerMode)follow;
                                camController.XModifyProperty(() => camController._followPlayerMode = followMode);
                                targetObject.XModifyProperty(() =>
                                {
                                    switch (followMode)
                                    {
                                        case EFollowPlayerMode.None:
                                        case EFollowPlayerMode.Move:
                                            {
                                                targetObject._playerItemData.followRotation = true;
                                                break;
                                            }
                                        case EFollowPlayerMode.Rotate:
                                        case EFollowPlayerMode.MoveAndRotate:
                                            {
                                                targetObject._playerItemData.followRotation = false;
                                                break;
                                            }
                                    }
                                });
                            }


                            // 导航图初始大小设置
                            var cam = camController.linkCamera;
                            if (cam)
                            {
                                EditorGUI.BeginChangeCheck();
                                var size = EditorGUILayout.FloatField(Tr("MiniMap Init Size"), cam.orthographicSize);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    cam.XModifyProperty(() => cam.orthographicSize = size);
                                }
                            }
                        }
                        return;
                    }
                case nameof(MiniMap._minimapType):
                    {
                        EditorGUI.BeginChangeCheck();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (EditorGUI.EndChangeCheck())
                        {
                            UICommonFun.DelayCall(SetMiniMapType);
                        }
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private void SetMiniMapType()
        {
            switch (targetObject._minimapType)
            {
                case MiniMap.EMiniMapType.Circle:
                    {
                        SetMiniMapSprite(targetObject._circleBorder, targetObject._circleMask);
                        break;
                    }
                case MiniMap.EMiniMapType.Rect:
                    {
                        SetMiniMapSprite(targetObject._rectBorder, targetObject._rectMask);
                        break;
                    }
            }
        }

        private void SetMiniMapSprite(Sprite border, Sprite mask)
        {
            targetObject._border.XModifyProperty(() => targetObject._border.sprite = border);
            targetObject._mask.XModifyProperty(() => targetObject._mask.sprite = mask);
        }

        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(MiniMap)));

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CategoryListExtension.DrawVertical(categoryList);
        }
    }
}
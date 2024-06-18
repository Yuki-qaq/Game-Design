using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base.EditorWindows;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// EditorGUILayout控件查看器
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Material)]
    [XDreamerEditorWindow(nameof(TrHelper.Other), categoryIndex = int.MaxValue)]
    public class EditorGUILayoutViewerWindow : XEditorWindowWithScrollView<EditorGUILayoutViewerWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "EditorGUILayout控件查看器";

        /// <summary>
        /// 静态主入口函数
        /// </summary>
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        /// <summary>
        /// 测试类型
        /// </summary>
        [Flags]
        [Name("测试类型")]
        public enum ETestType
        {
           /// <summary>
           /// E1
           /// </summary>
            E1 = 0,

            /// <summary>
            /// E2
            /// </summary>
            E2,

            /// <summary>
            /// E3
            /// </summary>
            E3,
        }

        /// <summary>
        /// 开始切换组
        /// </summary>
        public bool BeginToggleGroup = false;

        /// <summary>
        /// 包围盒字段
        /// </summary>
        public Bounds BoundsField = new Bounds();

        /// <summary>
        /// 颜色字段
        /// </summary>
        public Color ColorField = new Color();

        /// <summary>
        /// 曲线字段
        /// </summary>
        public AnimationCurve CurveField = new AnimationCurve();

        /// <summary>
        /// 延时浮点数字段
        /// </summary>
        public float DelayedFloatField = 0;

        /// <summary>
        /// 延时整型字段
        /// </summary>
        public int DelayedIntField = 0;

        /// <summary>
        /// 延时文本字段
        /// </summary>
        public string DelayedTextField = "";

        /// <summary>
        /// 双精度字段
        /// </summary>
        public double DoubleField = 0;

        /// <summary>
        /// 枚举遮罩字段
        /// </summary>
        public ETestType EnumMaskField = ETestType.E2;

        /// <summary>
        /// 枚举遮罩弹出
        /// </summary>
        public ETestType EnumMaskPopup = ETestType.E2;

        /// <summary>
        /// 枚举弹出
        /// </summary>
        public ETestType EnumPopup = ETestType.E2;

        /// <summary>
        /// 浮点字段
        /// </summary>
        public float FloatField = 0;

        /// <summary>
        /// 折叠
        /// </summary>
        public bool Foldout = true;

        /// <summary>
        /// 检查器标题条
        /// </summary>
        public bool InspectorTitlebar = true;

        /// <summary>
        /// 整型字段
        /// </summary>
        public int IntField = 0;

        /// <summary>
        /// 整型弹出
        /// </summary>
        public int IntPopup = 0;

        /// <summary>
        /// 整型滑动条
        /// </summary>
        public int IntSlider = 0;

        /// <summary>
        /// 旋钮
        /// </summary>
        public float Knob = 0;

        /// <summary>
        /// 层字段
        /// </summary>
        public int LayerField = 0;

        /// <summary>
        /// 长整型字段
        /// </summary>
        public long LongField = 0;

        /// <summary>
        /// 遮罩字段
        /// </summary>
        public int MaskField = 0;

        /// <summary>
        /// 最小最大滑动条最小值
        /// </summary>
        public float MinMaxSlider_minValue = -1;

        /// <summary>
        /// 最小最大滑动条最大值
        /// </summary>
        public float MinMaxSlider_maxValue = 1;

        /// <summary>
        /// 对象字段
        /// </summary>
        public UnityEngine.Object ObjectField = null;

        /// <summary>
        /// 密码字段
        /// </summary>
        public string PasswordField = "";

        /// <summary>
        /// 矩形字段
        /// </summary>
        public Rect RectField = new Rect();

        /// <summary>
        /// 滑动条
        /// </summary>
        public float Slider = 0;

        /// <summary>
        /// 标签字段
        /// </summary>
        public string TagField = "";

        /// <summary>
        /// 文本区域
        /// </summary>
        public string TextArea = "TextArea\nTextArea";

        /// <summary>
        /// 文本字段
        /// </summary>
        public string TextField = "TextField\nTextField";

        /// <summary>
        /// 切换
        /// </summary>
        public bool Toggle = true;

        /// <summary>
        /// 切换做
        /// </summary>
        public bool ToggleLeft = true;

        /// <summary>
        /// 二维向量字段
        /// </summary>
        public Vector2 Vector2Field = new Vector2();

        /// <summary>
        /// 三维向量字段
        /// </summary>
        public Vector3 Vector3Field = new Vector3();

        /// <summary>
        /// 四维向量字段
        /// </summary>
        public Vector4 Vector4Field = new Vector4();

        /// <summary>
        /// 当绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            try
            {
                EditorGUILayout.BeginVertical();
                {
                    BeginToggleGroup = EditorGUILayout.BeginToggleGroup("BeginToggleGroup", BeginToggleGroup);

                    BoundsField = EditorGUILayout.BoundsField("BoundsField", BoundsField);
                    ColorField = EditorGUILayout.ColorField("ColorField", ColorField);
                    CurveField = EditorGUILayout.CurveField("CurveField", CurveField);
                    DelayedFloatField = EditorGUILayout.DelayedFloatField("DelayedFloatField", DelayedFloatField);
                    DelayedIntField = EditorGUILayout.DelayedIntField("DelayedIntField", DelayedIntField);
                    DelayedTextField = EditorGUILayout.DelayedTextField("DelayedTextField", DelayedTextField);
                    DoubleField = EditorGUILayout.DoubleField("DoubleField", DoubleField);
                    EnumMaskField = (ETestType)EditorGUILayout.EnumFlagsField("EnumMaskField", EnumMaskField);
                    EnumMaskPopup = (ETestType)EditorGUILayout.EnumFlagsField(new GUIContent("EnumMaskPopup"), EnumMaskPopup);
                    EnumPopup = (ETestType)EditorGUILayout.EnumPopup("EnumPopup", EnumPopup);
                    FloatField = EditorGUILayout.FloatField("FloatField", FloatField);
                    Foldout = EditorGUILayout.Foldout(Foldout, "Foldout");
                    EditorGUILayout.GetControlRect(true, 20f);
                    EditorGUILayout.HelpBox("HelpBox", MessageType.Info);
                    //InspectorTitlebar = EditorGUILayout.InspectorTitlebar(InspectorTitlebar, (UnityEngine.Object)null, true);
                    //InspectorTitlebar = EditorGUILayout.InspectorTitlebar(InspectorTitlebar, (UnityEngine.Object)null, false);
                    IntField = EditorGUILayout.IntField("IntField", IntField);
                    IntPopup = EditorGUILayout.IntPopup("IntPopup", IntPopup, new string[] { "数字0", "数字1", "数字2" }, new int[] { 0, 1, 2 });
                    IntSlider = EditorGUILayout.IntSlider("IntSlider", IntSlider, -10, 10);
                    Knob = EditorGUILayout.Knob(new Vector2(40, 40), Knob, -10, 20, "Knob unit", Color.black, Color.red, true);
                    EditorGUILayout.LabelField("LabelField label", "LabelField label2");
                    LayerField = EditorGUILayout.LayerField("LayerField", LayerField);
                    LongField = EditorGUILayout.LongField("LongField", LongField);
                    MaskField = EditorGUILayout.MaskField("MaskField", MaskField, new string[] { "0", "1", "2" });
                    EditorGUILayout.MinMaxSlider(new GUIContent("MinMaxSlider"), ref MinMaxSlider_minValue, ref MinMaxSlider_maxValue, -10f, 10f);
                    ObjectField = EditorGUILayout.ObjectField(new GUIContent("ObjectField"), ObjectField, typeof(GameObject), true);
                    PasswordField = EditorGUILayout.PasswordField(new GUIContent("PasswordField"), PasswordField);
                    EditorGUILayout.PrefixLabel("PrefixLabel");
                    RectField = EditorGUILayout.RectField("RectField", RectField);
                    EditorGUILayout.SelectableLabel("SelectableLabel");
                    Slider = EditorGUILayout.Slider("Slider", Slider, -10, 10);
                    TagField = EditorGUILayout.TagField("TagField", TagField);
                    TextArea = EditorGUILayout.TextArea(TextArea);
                    TextField = EditorGUILayout.TextField("TextField", TextField);
                    Toggle = EditorGUILayout.Toggle("Toggle", Toggle);
                    ToggleLeft = EditorGUILayout.ToggleLeft("ToggleLeft", ToggleLeft);
                    Vector2Field = EditorGUILayout.Vector2Field("Vector2Field", Vector2Field);
                    Vector3Field = EditorGUILayout.Vector3Field("Vector3Field", Vector3Field);
                    Vector4Field = EditorGUILayout.Vector4Field("Vector4Field", Vector4Field);

                    EditorGUILayout.EndToggleGroup();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(this.titleContent + " Exception: " + ex.ToString());
            }
            finally
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.Separator();
            }
        }
    }
}

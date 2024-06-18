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
    /// 序列帧保存工具
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Image)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class FramesSavingTool : XEditorWindowWithScrollView<FramesSavingTool>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "序列帧保存工具";

        /// <summary>
        /// 初始化
        /// </summary>
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();
        
        private string recordButton = "开始记录";

        private float lastFrameTime = 0.0f;

        /// <summary>
        /// 图片文件输出目录
        /// </summary>
        [Name("图片文件输出目录")]
        public string filePath = "";

        /// <summary>
        /// 序列帧文件名
        /// </summary>
        [Name("序列帧文件名")]
        public string fileName = "frame";

        /// <summary>
        /// 图片扩展名
        /// </summary>
        [Name("图片扩展名")]
        [Tip("扩展名即文件后缀格式", "The extension is the file suffix format")]
        public string picExtName = "jpg";

        /// <summary>
        /// 图片尺寸倍数
        /// </summary>
        [Name("图片尺寸倍数")]
        public int superSize = 1;

        /// <summary>
        /// 最高FPS
        /// </summary>
        [Name("最高FPS")]
        [Tip("勾选后，以最高100FPS输出每帧图片；如不勾选，则使用期望FPS的进行输出；", "After each frame is checked, the picture is output with the highest FPS; If unchecked, use the desired FPS for output;")]
        public bool highestFPS = false;

        /// <summary>
        /// 期望FPS
        /// </summary>
        [Name("期望FPS")]
        public float expectFPS = 24;

        private float realExpectFPS => highestFPS ? 100 : expectFPS;

        /// <summary>
        /// 期望帧数目
        /// </summary>
        [Name("期望帧数目")]
        [Tip("0 则无限输出，直到运行停止或停止记录；", "0, the output is unlimited until the operation stops or the recording stops;")]
        public int expectFrameCount = 0;

        /// <summary>
        /// 捕获帧计数
        /// </summary>
        [Name("捕获帧计数")]
        public int capturedFrameCount = 0;

        /// <summary>
        /// 帧时差
        /// </summary>
        [Name("帧时差")]
        public float deltaTime = 0;

        /// <summary>
        /// 实时帧时差
        /// </summary>
        [Name("实时帧时差")]
        public float deltaTimeRealtime = 0;

        /// <summary>
        /// 状态
        /// </summary>
        [Name("状态")]
        public enum EState
        {
            /// <summary>
            /// 闲置
            /// </summary>
            [Name("闲置")]
            Free = 0,

            /// <summary>
            /// 准备记录
            /// </summary>
            [Name("准备记录")]
            ReadyToRecord,

            /// <summary>
            /// 记录中
            /// </summary>
            [Name("记录中")]
            Recording,
        }

        /// <summary>
        /// 状态
        /// </summary>
        [Name("状态")]
        [EnumPopup]
        public EState state = EState.Free;

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="record"></param>
        public void Record(bool record)
        {
            if (record)
            {
                capturedFrameCount = 0;
                recordButton = "停止记录";
                state = EState.ReadyToRecord;
            }
            else
            {
                recordButton = "开始记录";
                state = EState.Free;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            switch (state)
            {
                case EState.Free:
                    {
                        break;
                    }
                case EState.ReadyToRecord:
                    {
                        if (Application.isPlaying && !EditorApplication.isPaused)
                        {
                            deltaTime = 1f / realExpectFPS;
                            lastFrameTime = Time.time;
                            //Debug.Log(deltaTime);
                            state = EState.Recording;
                        }
                        break;
                    }
                case EState.Recording:
                    {
                        RecordImages();
                        Repaint();
                        break;
                    }
            }
        }

        /// <summary>
        /// 记录图像
        /// </summary>
        public void RecordImages()
        {
            if (highestFPS)
            {
                ScreenCapture.CaptureScreenshot(string.Format("{0}/{1}{2}.{3}", filePath, fileName, capturedFrameCount, picExtName), superSize);
                capturedFrameCount++;
            }
            else
            {
                deltaTimeRealtime = Time.time - lastFrameTime;
                if (deltaTimeRealtime >= deltaTime)
                {
                    ScreenCapture.CaptureScreenshot(string.Format("{0}/{1}{2}.{3}", filePath, fileName, capturedFrameCount, picExtName), superSize);

                    lastFrameTime = Time.time;
                    capturedFrameCount++;
                }
            }

            if (capturedFrameCount >= expectFrameCount && expectFrameCount > 0)
            {
                Record(false);
            }
        }

        /// <summary>
        /// 绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            EditorGUI.BeginDisabledGroup(state != EState.Free);
            {
                EditorGUILayout.BeginHorizontal();
                if (string.IsNullOrEmpty(filePath)) filePath = Application.streamingAssetsPath;
                filePath = EditorGUILayout.TextField(Get(nameof(filePath)), filePath);
                if (GUILayout.Button("...", GUILayout.Width(50)))
                {
                    filePath = EditorUtility.OpenFolderPanel("选择文件夹", filePath, "");
                }
                EditorGUILayout.EndHorizontal();

                fileName = EditorGUILayout.TextField(Get(nameof(fileName)), fileName);
                picExtName = EditorGUILayout.TextField(Get(nameof(picExtName)), picExtName).TrimStart('.');
                superSize = EditorGUILayout.IntSlider(Get(nameof(superSize)), superSize, 1, 16);

                expectFrameCount = EditorGUILayout.IntSlider(Get(nameof(expectFrameCount)), expectFrameCount, 0, short.MaxValue);

                highestFPS = EditorGUILayout.Toggle(Get(nameof(highestFPS)), highestFPS);
                EditorGUI.BeginDisabledGroup(highestFPS);
                expectFPS = EditorGUILayout.Slider(Get(nameof(expectFPS)), expectFPS, 0.01f, 100f);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.LabelField(Get(nameof(deltaTime)), new GUIContent(deltaTime.ToString()));
                EditorGUILayout.LabelField(Get(nameof(deltaTimeRealtime)), new GUIContent(deltaTimeRealtime.ToString()));
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.LabelField(Get(nameof(state)), CommonFun.NameTooltip(state));
            EditorGUILayout.LabelField(Get(nameof(capturedFrameCount)), new GUIContent(capturedFrameCount.ToString()));

            EditorGUILayout.Separator();

            if (GUILayout.Button(recordButton))
            {
                Record(state == EState.Free);
            }
        }

        private GUIContent Get(string memberName) => CommonFun.NameTip(GetType(), memberName);
    }
}
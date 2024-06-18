using System;
using System.Runtime.InteropServices;

namespace XCSJ.PluginART.Base
{
    /// <summary>
    /// ART本地
    /// </summary>
    public static class ARTNative
    {
        /// <summary>
        /// DLL路径
        /// </summary>
        public const string DllPath = "ART.Native";

        /// <summary>
        /// DTrack SDK Flystick 最大按钮数
        /// </summary>
        public const int DTRACKSDK_FLYSTICK_MAX_BUTTON = 16;

        /// <summary>
        /// DTrack SDK Flystick 最大摇杆数
        /// </summary>
        /// 
        public const int DTRACKSDK_FLYSTICK_MAX_JOYSTICK = 8;

        /// <summary>
        /// DTrack SDK Meatool 最大按钮数
        /// </summary>
        public const int DTRACKSDK_MEATOOL_MAX_BUTTON = 16;

        /// <summary>
        /// DTrack SDK 手最大手指数
        /// </summary>
        public const int DTRACKSDK_HAND_MAX_FINGER = 5;

        /// <summary>
        ///  DTrack SDK 人最大关节数
        /// </summary>
        public const int DTRACKSDK_HUMAN_MAX_JOINTS = 200;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="server_host"></param>
        /// <param name="server_port"></param>
        /// <param name="data_port"></param>
        /// <param name="remote_type"></param>
        /// <param name="data_bufsize"></param>
        /// <param name="data_timeout_us"></param>
        /// <param name="srv_timeout_us"></param>
        /// <returns></returns>
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr create(ref string server_host, ushort server_port, ushort data_port, int remote_type = 0, int data_bufsize = 32768, int data_timeout_us = 1000000, int srv_timeout_us = 10000000);

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void destory(IntPtr instance);

        /// <summary>
        /// 接收
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool receive(IntPtr instance);

        /// <summary>
        /// 启动测量
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern bool startMeasurement(IntPtr instance);

        /// <summary>
        /// 停止测量
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern bool stopMeasurement(IntPtr instance);

        /// <summary>
        /// UDP是否有效
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern bool isUDPValid(IntPtr instance);

        /// <summary>
        /// TCP是否有效
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern bool isTCPValid(IntPtr instance);

        /// <summary>
        /// 获取数目
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern int getNumBody(IntPtr instance);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport(DllPath)]
        public static extern bool getBody(IntPtr instance, int id, ref DTrack_Body_Type_d data);

        /// <summary>
        /// 获取数目
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern int getNumFlyStick(IntPtr instance);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport(DllPath)]
        public static extern bool getFlyStick(IntPtr instance, int id, ref DTrack_FlyStick_Type_d data);

        /// <summary>
        /// 获取数目
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern int getNumMeaTool(IntPtr instance);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport(DllPath)]
        public static extern bool getMeaTool(IntPtr instance, int id, ref DTrack_MeaTool_Type_d data);

        /// <summary>
        /// 获取数目
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern int getNumMeaRef(IntPtr instance);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport(DllPath)]
        public static extern bool getMeaRef(IntPtr instance, int id, ref DTrack_MeaRef_Type_d data);

        /// <summary>
        /// 获取数目
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern int getNumHand(IntPtr instance);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport(DllPath)]
        public static extern bool getHand(IntPtr instance, int id, ref DTrack_Hand_Type_d data);

        /// <summary>
        /// 获取数目
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern int getNumHuman(IntPtr instance);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport(DllPath)]
        public static extern bool getHuman(IntPtr instance, int id, ref DTrack_Human_Type_d data);

        /// <summary>
        /// 获取数目
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern int getNumInertial(IntPtr instance);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport(DllPath)]
        public static extern bool getInertial(IntPtr instance, int id, ref DTrack_Inertial_Type_d data);

        /// <summary>
        /// 获取数目
        /// </summary>
        /// <param name="instance"></param>
        [DllImport(DllPath)]
        public static extern int getNumMarker(IntPtr instance);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [DllImport(DllPath)]
        public static extern bool getMarker(IntPtr instance, int id, ref DTrack_Marker_Type_d data);
    }

    /// <summary>
    /// DTrack身体类型
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DTrack_Body_Type_d
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id;          //!< id number (starting with 0)

        /// <summary>
        /// 质量
        /// </summary>
        public double quality;  //!< quality (0 <= qu <= 1, no tracking if -1)

        /// <summary>
        /// 位置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] loc;   //!< location (in mm)

        /// <summary>
        /// 旋转矩阵
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] rot;   //!< rotation matrix (column-wise)

        /// <summary>
        /// 协方差参考点
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] covref;   //!< reference point of covariance

        /// <summary>
        /// 6d姿态的6x6维协方差矩阵（3d位置以[mm]为单位，3d欧拉角以[rad]为单位）。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public double[] cov;   //!< 6x6-dimensional covariance matrix for the 6d pose (with 3d location in [mm], 3d euler angles in [rad]).
    };

    /// <summary>
    /// DTrack FlyStick类型
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DTrack_FlyStick_Type_d
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id;         //!< id number (starting with 0)

        /// <summary>
        /// 质量
        /// </summary>
        public double quality; //!< quality (0 <= qu <= 1, no tracking if -1)

        /// <summary>
        /// 按钮数量
        /// </summary>
        public int num_button; //!< number of buttons

        /// <summary>
        /// 按钮
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ARTNative.DTRACKSDK_FLYSTICK_MAX_BUTTON)]
        public int[] button;  //!< button state (1 pressed, 0 not pressed): 0 front, 1..n-1 right to left

        /// <summary>
        /// 摇杆数量
        /// </summary>
        public int num_joystick;  //!< number of joystick values

        /// <summary>
        /// 摇杆
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ARTNative.DTRACKSDK_FLYSTICK_MAX_JOYSTICK)]
        public double[] joystick;  //!< joystick value (-1 <= joystick <= 1); 0 horizontal, 1 vertical

        /// <summary>
        /// 位置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] loc;  //!< location (in mm)

        /// <summary>
        /// 旋转矩阵
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] rot;  //!< rotation matrix (column-wise)
    };

    /// <summary>
    /// DTrack测量工具类型
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DTrack_MeaTool_Type_d
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id;         //!< id number (starting with 0)

        /// <summary>
        /// 质量
        /// </summary>
        public double quality; //!< quality (0 <= qu <= 1, no tracking if -1)

        /// <summary>
        /// 按钮数量
        /// </summary>
        public int num_button; //!< number of buttons

        /// <summary>
        /// 按钮
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ARTNative.DTRACKSDK_MEATOOL_MAX_BUTTON)]
        public int[] button;  //!< button state (1 pressed, 0 not pressed): 0 front, 1..n-1 right to left

        /// <summary>
        /// 位置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] loc;  //!< location (in mm)

        /// <summary>
        /// 旋转矩阵
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] rot;  //!< rotation matrix (column-wise)

        /// <summary>
        /// 尖端半径（如适用）
        /// </summary>
        public double tipradius;   //!< radius of tip if applicable

        /// <summary>
        /// 位置协方差（单位：mm^2）
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] cov;  //!< covariance of location (in mm^2)
    };

    /// <summary>
    /// DTrack测量引用类型
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DTrack_MeaRef_Type_d
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id;         //!< id number (starting with 0)

        /// <summary>
        /// 质量
        /// </summary>
        public double quality; //!< quality (0 <= qu <= 1, no tracking if -1)

        /// <summary>
        /// 位置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] loc;  //!< location (in mm)

        /// <summary>
        /// 旋转矩阵
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] rot;  //!< rotation matrix (column-wise)
    };

    /// <summary>
    /// DTrack手类型
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DTrack_Hand_Type_d
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id;         //!< id number (starting with 0)

        /// <summary>
        /// 质量
        /// </summary>
        public double quality; //!< quality (0 <= qu <= 1, no tracking if -1)

        /// <summary>
        /// 左右
        /// </summary>
        public int lr;         //!< left (0) or right (1) hand

        /// <summary>
        /// 手指数量
        /// </summary>
        public int nfinger;    //!< number of fingers (maximum 5)

        /// <summary>
        /// 位置：手背：位置（单位：mm）
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] loc;  //!< back of the hand: location (in mm)

        /// <summary>
        /// 旋转：手背：旋转矩阵（按列）
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] rot;  //!< back of the hand: rotation matrix (column-wise)

        /// <summary>
        /// 手指：顺序：拇指，食指，中指。。。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ARTNative.DTRACKSDK_HAND_MAX_FINGER)]
        public DTrack_Hand_Type_d__Finger[] finger; //!< order: thumb, index finger, middle finger, ...
    };

    /// <summary>
    /// DTrack手类型手指
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DTrack_Hand_Type_d__Finger
    {
        /// <summary>
        /// 位置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] loc;           //!< location (in mm)

        /// <summary>
        /// 旋转
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] rot;           //!< rotation matrix (column-wise)

        /// <summary>
        /// 半径尖端
        /// </summary>
        public double radiustip;        //!< radius of tip

        /// <summary>
        /// 指骨长度:指骨长度；顺序：最外面、中间、最里面
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] lengthphalanx; //!< length of phalanxes; order: outermost, middle, innermost

        /// <summary>
        /// 指骨间夹角
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] anglephalanx;  //!< angle between phalanxes
    };

    /// <summary>
    /// DTrack人类型
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DTrack_Human_Type_d
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id;         //!< id of the human model (starting with 0)

        /// <summary>
        /// 关节数量
        /// </summary>
        public int num_joints; //!< number of joints of the human model

        /// <summary>
        /// 关节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ARTNative.DTRACKSDK_HUMAN_MAX_JOINTS)]
        public DTrack_Human_Type_d__Joint[] joint; //!< location and orientation of the joint
    };

    /// <summary>
    /// DTrack人类型关节
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DTrack_Human_Type_d__Joint
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id;           //!< id of the joint (starting with 0)

        /// <summary>
        /// 质量
        /// </summary>
        public double quality;   //!< quality of the joint (0 <= qu <= 1, no tracking if -1)

        /// <summary>
        /// 位置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] loc;    //!< location of the joint (in mm)

        /// <summary>
        /// 角度
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] ang;    //!< angles in relation to the joint coordinate system

        /// <summary>
        /// 旋转
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] rot;    //!< rotation matrix of the joint (column-wise) in relation to room coordinaten system
    };

    /// <summary>
    /// DTrack惯性类型
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DTrack_Inertial_Type_d
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id;          //!< id number (starting with 0)

        /// <summary>
        /// 状态：传感器状态（无跟踪0或1或2）
        /// </summary>
        public int st;          //!< state of sensor (no tracking 0 or 1 or 2)

        /// <summary>
        /// 错误：错误（0处于状态0和2，增加状态1小于等于360）
        /// </summary>
        public double error;    //!< error (0 in state 0 and 2, increase in state 1 <=360)

        /// <summary>
        /// 位置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] loc;   //!< location (in mm)

        /// <summary>
        /// 旋转
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] rot;   //!< rotation matrix (column-wise)
    };

    /// <summary>
    /// DTrack标记类型
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DTrack_Marker_Type_d
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id;          //!< id number (starting with 1)

        /// <summary>
        /// 质量
        /// </summary>
        public double quality;  //!< quality (0.0 <= qu <= 1.0; -1 not tracked)

        /// <summary>
        /// 位置
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] loc;   //!< location (in mm)
    };
}

using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorTools;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMMO;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginMMO.Tools;

namespace XCSJ.EditorMMO.Tools
{
    /// <summary>
    /// MMO提供者检查器
    /// </summary>
    [CustomEditor(typeof(MMOProvider), true)]
    public class MMOProviderInspector : InteractorInspector<MMOProvider>
    {
        #region 网络信息

        /// <summary>
        /// 显示网络信息
        /// </summary>
        [Name("显示网络信息")]
        public bool displayNetInfo = true;

        /// <summary>
        /// 会话编号
        /// </summary>
        [Readonly]
        [Name("会话编号")]
        public string sessionGuid = "";

        /// <summary>
        /// 用户编号
        /// </summary>
        [Readonly]
        [Name("用户编号")]
        public string userGuid = "";

        /// <summary>
        /// 房间编号
        /// </summary>
        [Readonly]
        [Name("房间编号")]
        public string roomGuid = "";

        /// <summary>
        /// 网络状态
        /// </summary>
        [Readonly]
        [Name("网络状态")]
        [EnumPopup]
        public ENetState netState = ENetState.Unknow;

        /// <summary>
        /// Ping值:单位：毫秒ms
        /// </summary>
        [Readonly]
        [Name("Ping")]
        [Tip("单位：毫秒", "Unit: ms")]
        public double ping = 0;

        /// <summary>
        /// 返回码
        /// </summary>
        [Readonly]
        [Name("返回码")]
        [EnumPopup]
        public EACode aCode = EACode.Unknow;

        /// <summary>
        /// 同步房间的数据已处理数量
        /// </summary>
        [Readonly]
        [Name("同步房间的数据已处理数量")]
        public double asyncRoomDataHandledCount = 0;

        /// <summary>
        /// 同步房间的数据总数量
        /// </summary>
        [Readonly]
        [Name("同步房间的数据总数量")]
        public double asyncRoomDataTotalCount = 0;

        /// <summary>
        /// 同步房间进度
        /// </summary>
        [Readonly]
        [Name("同步房间进度")]
        public double asyncRoomProgress = 0;

        private void OnEnterRoomCompleted(EACode code)
        {
            if (MMOHelper.isEnteredRoom) base.StartTimedRepaint();
        }

        private void OnNetStateChanged(ENetState oldNetState, ENetState newNetState) => Repaint();

        /// <summary>
        /// 定时绘制
        /// </summary>
        public override bool timedRepaint => base.timedRepaint || MMOHelper.isEnteredRoom;

        private void DrawNetInfo()
        {
            displayNetInfo = UICommonFun.Foldout(displayNetInfo, TrLabel(nameof(displayNetInfo)));
            if (!displayNetInfo) return;

            CommonFun.BeginLayout();
            EditorGUILayout.TextField(TrLabel(nameof(sessionGuid)), MMOHelper.sessionGuid);
            EditorGUILayout.TextField(TrLabel(nameof(userGuid)), MMOHelper.userGuid);
            EditorGUILayout.TextField(TrLabel(nameof(roomGuid)), MMOHelper.roomGuid);
            UICommonFun.EnumPopup(TrLabel(nameof(netState)), MMOHelper.netState);
            UICommonFun.EnumPopup(TrLabel(nameof(aCode)), MMOHelper.aCode);
            EditorGUILayout.DoubleField(TrLabel(nameof(ping)), MMOHelper.ping);
            EditorGUILayout.DoubleField(TrLabel(nameof(asyncRoomDataHandledCount)), MMOHelper.asyncRoomDataHandledCount);
            EditorGUILayout.DoubleField(TrLabel(nameof(asyncRoomDataTotalCount)), MMOHelper.asyncRoomDataTotalCount);
            EditorGUILayout.DoubleField(TrLabel(nameof(asyncRoomProgress)), MMOHelper.asyncRoomProgress);
            CommonFun.EndLayout();
        }

        #endregion

        #region 运行时控制器

        /// <summary>
        /// 显示运行时控制器
        /// </summary>
        [Name("显示运行时控制器")]
        public bool displayRuntimeController = true;

        [LanguageTuple("Start MMO", "启动MMO")]
        [LanguageTuple("Stop MMO", "停止MMO")]
        [LanguageTuple("Enter Room", "进入房间")]
        [LanguageTuple("Exit Room", "退出房间")]
        private void DrawRuntime()
        {
            displayRuntimeController = UICommonFun.Foldout(displayRuntimeController, TrLabel(nameof(displayRuntimeController)));
            if (!displayRuntimeController) return;

            CommonFun.BeginLayout();
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(MMOHelper.started);
                if (GUILayout.Button(Tr("Start MMO")))
                {
                    mb.StartMMO();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!MMOHelper.started);
                if (GUILayout.Button(Tr("Stop MMO")))
                {
                    mb.StopMMO();
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(!MMOHelper.canEnterRoom);
                if (GUILayout.Button(Tr("Enter Room")))
                {
                    mb.EnterRoom();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!MMOHelper.isEnteredRoom);
                if (GUILayout.Button(Tr("Exit Room")))
                {
                    mb.ExitRoom();
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();
            CommonFun.EndLayout();
        }

        #endregion

        #region 房间列表

        /// <summary>
        /// 显示房间列表
        /// </summary>
        [Name("显示房间列表")]
        public bool displayRooms = true;

        /// <summary>
        /// 搜索房间文本
        /// </summary>
        [Name("搜索房间文本")]
        public string searchRoomText = "";

        [LanguageTuple("Refresh Room List", "刷新房间列表")]
        [LanguageTuple("Default Room", "默认房间")]
        [LanguageTuple("New Room", "新建房间")]
        [LanguageTuple("Room Name", "房间名")]
        [LanguageTuple("Password", "密码")]
        [LanguageTuple("Count", "数量")]
        [LanguageTuple("Operation", "操作")]
        [LanguageTuple("Y", "有")]
        [LanguageTuple("N", "无")]
        private void DrawRooms()
        {
            displayRooms = UICommonFun.Foldout(displayRooms, TrLabel(nameof(displayRooms)));
            if (!displayRooms) return;

            CommonFun.BeginLayout();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            {
                if (GUILayout.Button(Tr("Refresh Room List")))
                {
                    MMOHelper.RoomList();
                }
                if (GUILayout.Button(Tr("Default Room")) && MMOHelper.canEnterRoom)
                {
                    mb.EnterDefaultRoom();
                }
                if (GUILayout.Button(Tr("New Room")) && MMOHelper.canEnterRoom)
                {
                    mb.EnterNewRoom();
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            searchRoomText = EditorGUILayout.TextField(TrLabel(nameof(searchRoomText)), searchRoomText);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("NO.", GUILayout.Width(32));
            EditorGUILayout.LabelField(Tr("Operation"), GUILayout.Width(60));
            EditorGUILayout.LabelField(Tr("Room Name"));
            EditorGUILayout.LabelField(Tr("Password"), GUILayout.Width(60));
            EditorGUILayout.LabelField(Tr("Count"), GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            int i = 0;
            foreach (var room in MMOHelper.rooms)
            {
                if (!StringHelper.SearchMatch(room.name, searchRoomText)) continue;

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField((++i).ToString(), GUILayout.Width(32));

                EditorGUI.BeginDisabledGroup(!MMOHelper.canEnterRoom || room.userCount >= room.limitCount);
                if (GUILayout.Button(Tr("Enter Room"), GUILayout.Width(60)))
                {
                    mb.EnterDesignatedRoom(room.roomGuid);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.TextField(room.name);
                EditorGUILayout.LabelField(room.pwd ? Tr("Y") : Tr("N"), GUILayout.Width(60));
                EditorGUILayout.LabelField(string.Format("{0}/{1}", room.userCount, room.limitCount), GUILayout.Width(60));

                EditorGUILayout.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        #endregion

        #region 用户列表

        /// <summary>
        /// 显示用户列表
        /// </summary>
        [Name("显示用户列表")]
        public bool displayUsers = true;

        [LanguageTuple("Nickname", "昵称")]
        [LanguageTuple("User Name", "用户名")]
        [LanguageTuple("User Guid", "用户编号")]
        private void DrawUsers()
        {
            displayUsers = UICommonFun.Foldout(displayUsers, TrLabel(nameof(displayUsers)));
            if (!displayUsers) return;

            CommonFun.BeginLayout();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("NO.", GUILayout.Width(32));
            EditorGUILayout.LabelField(Tr("Operation"), GUILayout.Width(60));
            EditorGUILayout.LabelField(Tr("User Name"), GUILayout.Width(120));
            EditorGUILayout.LabelField(Tr("User Guid"), GUILayout.Width(300));
            EditorGUILayout.LabelField(Tr("Nickname"), GUILayout.MinWidth(100));
            EditorGUILayout.EndHorizontal();

            int i = 0;
            foreach (var kv in MMOHelper.players)
            {
                var player = kv.Value;

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField((++i).ToString(), GUILayout.Width(32));

                if (GUILayout.Button("Ping", GUILayout.Width(60)))
                {
                    UICommonFun.PingObject(player.netIdentity);
                }

                EditorGUILayout.TextField(player.name, GUILayout.Width(120));
                EditorGUILayout.TextField(player.guid, GUILayout.Width(300));
                EditorGUILayout.TextField(player.nickname, GUILayout.MinWidth(100));

                EditorGUILayout.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        #endregion

        #region 玩家生成器

        /// <summary>
        /// 显示玩家生成器
        /// </summary>
        [Name("显示玩家生成器")]
        public bool displayPlayerCreater = true;

        [LanguageTuple("Character Prototype", "角色原型")]
        [LanguageTuple("Start Position", "起始位置")]
        [LanguageTuple("Nickname", "昵称")]
        [LanguageTuple("Delete Player", "删除玩家")]
        [LanguageTuple("Create Player", "创建玩家")]
        [LanguageTuple("Recreate Player", "重新创建玩家")]
        [LanguageTuple("Random Recreate Player", "随机重新创建玩家")]
        private void DrawPlayerCreater()
        {
            displayPlayerCreater = UICommonFun.Foldout(displayPlayerCreater, TrLabel(nameof(displayPlayerCreater)));
            if (!displayPlayerCreater) return;

            CommonFun.BeginLayout();

            bool hasPlayer = mb.player;

            //角色原型
            {
                GUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel(Tr("Character Prototype"));

                var list = mb.playerCharacterPrototypes;
                var array = list.ToList(p => p ? p.displayName : "<Null>").ToArray();

                var oldIndex = list.IndexOf(mb.selectedPlayerCharacterPrototype);
                var index = GUILayout.SelectionGrid(oldIndex, array, 2);
                if (oldIndex != index && !hasPlayer)
                {
                    mb.selectedPlayerCharacterPrototype = list[index];
                }
                GUILayout.EndHorizontal();
            }

            //起始位置
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(Tr("Start Position"));
                var list = mb.playerStartPositions;
                var array = list.ToList(p => p? p.startPositionName:"<Null>").ToArray();

                var oldIndex = list.IndexOf(mb.selectedPlayerStartPosition);
                var index = GUILayout.SelectionGrid(oldIndex, array, 2);
                if (oldIndex != index && !hasPlayer)
                {
                    mb.selectedPlayerStartPosition = list[index];
                }
                GUILayout.EndHorizontal();
            }

            //昵称
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(Tr("Nickname"));

                var list = mb.playerNicknames;
                var array = list.ToArray();
                var oldIndex = list.IndexOf(mb.selectedPlayerNickname);
                var index = GUILayout.SelectionGrid(oldIndex, array, 2);
                if (oldIndex != index && !hasPlayer)
                {
                    mb.selectedPlayerNickname = list[index];
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(Tr("Nickname"));
                mb.selectedPlayerNickname = EditorGUILayout.DelayedTextField(mb.selectedPlayerNickname);
                GUILayout.EndHorizontal();

                if (GUILayout.Button("强制更新昵称"))
                {
                    mb.selectedPlayerNickname = (int.TryParse(mb.selectedPlayerNickname, out var i) ? (i + 1) : 0).ToString();
                }
            }

            if (hasPlayer)
            {
                if (GUILayout.Button(Tr("Delete Player")))
                {
                    mb.DeletePlayer();
                }
            }
            else
            {
                if (GUILayout.Button(Tr("Create Player")))
                {
                    mb.CreatePlayer();
                }
            }

            if (GUILayout.Button(Tr("Recreate Player")))
            {
                mb.RecreatePlayer();
            }

            if (GUILayout.Button(Tr("Random Recreate Player")))
            {
                mb.RerandomRecreatePlayer();
            }

            CommonFun.EndLayout();
        }

        #endregion

        #region Unity 消息

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            MMOHelper.onNetStateChanged += OnNetStateChanged;
            MMOHelper.onEnterRoomCompleted += OnEnterRoomCompleted;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            MMOHelper.onNetStateChanged -= OnNetStateChanged;
            MMOHelper.onEnterRoomCompleted -= OnEnterRoomCompleted;
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawNetInfo();
            DrawRuntime();
            DrawRooms();
            DrawUsers();
            DrawPlayerCreater();
        }

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [LanguageTuple("Collect All NetPlayer Components", "收集所有网络玩家组件")]
        [LanguageTuple("Collect All NetPlayerStartPosition Components", "收集所有玩家起始位置组件")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(MMOProvider._playerCharacterPrototypes):
                    {
                        if (GUILayout.Button(Tr("Collect All NetPlayer Components")))
                        {
                            mb.XModifyProperty(() =>
                            {
                                mb._playerCharacterPrototypes.AddRangeWithDistinct(CommonFun.GetComponentsInChildren<NetPlayer>(true));
                            });
                        }
                        break;
                    }
                case nameof(MMOProvider._playerStartPositions):
                    {
                        if (GUILayout.Button(Tr("Collect All NetPlayerStartPosition Components")))
                        {
                            mb.XModifyProperty(() =>
                            {
                                mb._playerStartPositions.AddRangeWithDistinct(CommonFun.GetComponentsInChildren<NetPlayerStartPosition>(true));
                            });
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        #endregion
    }
}

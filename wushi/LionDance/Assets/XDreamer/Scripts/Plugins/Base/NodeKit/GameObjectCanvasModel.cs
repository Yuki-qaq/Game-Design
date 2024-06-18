using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.NodeKit;

namespace XCSJ.Extension.Base.NodeKit
{
    /// <summary>
    /// 游戏对象画布模型
    /// </summary>
    [Name("游戏对象画布模型")]
    [XCSJ.Attributes.Icon(EIcon.GameObject)]
    public class GameObjectCanvasModel : ICanvasModel
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string displayName => gameObject ? gameObject.name : "";

        /// <summary>
        /// 节点矩形
        /// </summary>
        public Rect nodeRect { get; set; } = new Rect(0, 0, 0, 0);

        /// <summary>
        /// 画布矩形
        /// </summary>
        public Rect canvasRect { get; set; } = new Rect(0, 0, 0, 0);

        /// <summary>
        /// 节点模型集合
        /// </summary>
        public IEnumerable<INodeModel> nodeModels => canvasModels;

        /// <summary>
        /// 是有效内容
        /// </summary>
        public bool isValidContent => gameObject;

        private ICanvasModel[] canvasModels;

        object INodeModel.entityModel => gameObject;

        /// <summary>
        /// 游戏对象
        /// </summary>
        public GameObject gameObject { get; private set; }

        /// <summary>
        /// 父级画布模型
        /// </summary>
        public ICanvasModel parentCanvasModel => default;

        /// <summary>
        /// 父级矩形
        /// </summary>
        public Rect parentRect { get; set; } = new Rect();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameObject"></param>
        public GameObjectCanvasModel(GameObject gameObject)
        {
            this.gameObject = gameObject;

            canvasModels = gameObject.GetComponents<ICanvasModel>();
        }
    }

}

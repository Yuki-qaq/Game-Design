using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 项
    /// </summary>
    public class Item : InteractableVirtual
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Readonly]
        public string _id;

        /// <summary>
        /// 项名称
        /// </summary>
        public string _itemName;

        /// <summary>
        /// 显示描述
        /// </summary>
        public string _showDescription;

        /// <summary>
        /// 图标
        /// </summary>
        public Sprite _icon;

        /// <summary>
        /// 编号
        /// </summary>
        public virtual string id { get => _id; set => _id = value; }

        /// <summary>
        /// 项名称
        /// </summary>
        public virtual string itemName { get => _itemName; set => _itemName = value; }

        /// <summary>
        /// 容器
        /// </summary>
        public ItemCollection _container;

        /// <summary>
        /// 容器
        /// </summary>
        public virtual ItemCollection container { get => _container; set => _container = value; }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (string.IsNullOrEmpty(_id))
            {
                _id = System.Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        public void Remove()
        {
            if (container)
            {
                container.Remove(this);
            }
        }
    }

    /// <summary>
    /// 物品信息
    /// </summary>
    [Serializable]
    public class ItemInfo
    {
        /// <summary>
        /// 物品名称
        /// </summary>
        [Name("物品名称")]
        public string _name;

        /// <summary>
        /// 数量
        /// </summary>
        [Name("数量")]
        [Min(0)]
        public int _amount;

        /// <summary>
        /// 物品坐标参考
        /// </summary>
        [Name("物品坐标参考")]
        public Transform _pose;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="amount"></param>
        /// <param name="pose"></param>
        public ItemInfo(string name, int amount, Transform pose)
        {
            this._name = name;
            this._amount = amount;
            this._pose = pose;
        }

        /// <summary>
        /// 设置姿态
        /// </summary>
        /// <param name="transform"></param>
        public void SetPose(Transform transform)
        {
            if (_pose)
            {
                transform.SetPositionAndRotation(_pose.position, _pose.rotation);
            }
        }
    }

}

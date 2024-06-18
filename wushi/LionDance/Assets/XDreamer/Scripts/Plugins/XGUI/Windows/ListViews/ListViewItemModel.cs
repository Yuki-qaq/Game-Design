using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.Windows.ListViews
{
    #region 列表视图项模型

    /// <summary>
    /// 列表视图项模型
    /// </summary>
    public class ListViewItemModel : IModelKeyValue
    {
        /// <summary>
        /// 列表视图项:模型所属的列表视图项
        /// </summary>
        public virtual ListViewItem listViewItem { get; internal set; }

        /// <summary>
        /// 列表视图
        /// </summary>
        public ListView listView => listViewItem ? listViewItem.listView : default;

        /// <summary>
        /// 模型实体
        /// </summary>
        public virtual object modelEntity { get; private set; }

        /// <summary>
        /// 索引:程序索引
        /// </summary>
        public int index => listViewItem ? listViewItem.index : -1;

        /// <summary>
        /// 层级索引
        /// </summary>
        public int hierarchyIndex => index + 1;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ListViewItemModel() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="modelEntity"></param>
        public ListViewItemModel(object modelEntity) 
        { 
            this.modelEntity = modelEntity;
        }

        /// <summary>
        /// 颜色
        /// </summary>
        public Color color
        {
            get
            {
                if (listView)
                {
                    return selected? listView._selectedColor : listView._unselectedColor;
                }
                return Color.black;
            }
        }

        /// <summary>
        /// 选择
        /// </summary>
        public virtual bool selected 
        { 
            get
            {
                var list = listView;
                if (list && list.listHost != null)
                {
                    return list.listHost.IsSelected(listView.refreshList, modelEntity);
                }
                return _selected;
            }
            set
            {
                _selected = value;
            }
        } 
        private bool _selected = false;

        /// <summary>
        /// 可交互性
        /// </summary>
        public virtual bool interactable { get; set; } = true;

        /// <summary>
        /// 有效性
        /// </summary>
        public virtual bool valid { get; protected set; } = true;

        #region IModelKeyValue

        /// <summary>
        /// 键列表
        /// </summary>
        public virtual IEnumerable<string> keys
        {
            get
            {
                if (_keys == null)
                {
                    _keys = new string[] { nameof(selected) };
                }
                return _keys;
            }
        }
        private string[] _keys = null;

        /// <summary>
        /// 尝试获取模型键值类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool TryGetModelKeyValueType(string key, out Type type)
        {
            switch (key)
            {
                case nameof(selected):
                    {
                        type = typeof(bool);
                        return true;
                    }
                default:
                    {
                        type = default;
                        return false;
                    }
            }
        }

        /// <summary>
        /// 尝试获取模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool TryGetModelKeyValue(string key, out object value)
        {
            switch (key)
            {
                case nameof(selected):
                    {
                        value = selected;
                        return true;
                    }
                default:
                    {
                        value = default;
                        return false;
                    }
            }
        }

        /// <summary>
        /// 尝试设置模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool TrySetModelKeyValue(string key, object value)
        {
            switch (key)
            {
                case nameof(selected):
                    {

                        if (Converter.instance.TryConvertTo<bool>(value, out var result))
                        {
                            selected = result;
                            return true;
                        }
                        break;
                    }
            }
            return false;
        }

        #endregion
    }

    #endregion

    #region 可拖拽模型

    /// <summary>
    /// 可拖拽模型
    /// </summary>
    public abstract class DraggableModel : ListViewItemModel
    {
        /// <summary>
        /// 拖拽开始
        /// </summary>
        public virtual void OnDragStart(ListViewInteractData viewItemData) { }

        /// <summary>
        /// 拖拽中
        /// </summary>
        public virtual void OnDrag(ListViewInteractData viewItemData) { }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        public virtual void OnDragEnd(ListViewInteractData viewItemData) { }
    }

    #endregion

    #region Unity对象模型

    /// <summary>
    /// Unity对象模型
    /// </summary>
    public class UnityObjectModel<T> : DraggableModel where T : UnityEngine.Object
    {
        /// <summary>
        /// Unity对象
        /// </summary>
        [Name("Unity对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [DynamicLabel]
        public T _unityObject;

        /// <summary>
        /// Unity对象
        /// </summary>
        public virtual T unityObject { get => _unityObject; protected set => _unityObject = value; }

        /// <summary>
        /// 模型实体
        /// </summary>
        public override object modelEntity => unityObject;

        /// <summary>
        /// 有效性
        /// </summary>
        public override bool valid { get => unityObject; protected set { } }

        /// <summary>
        /// 标题
        /// </summary>
        [Name("标题")]
        [DynamicLabel]
        public string _title = "";

        /// <summary>
        /// 标题
        /// </summary>
        public virtual string title { get => _title; set => _title = value; }

        /// <summary>
        /// 图片
        /// </summary>
        [Name("图片")]
        [DynamicLabel]
        public Texture2D _texture2D;

        /// <summary>
        /// 图片
        /// </summary>
        public Texture2D texture2D
        {
            get => _texture2D;
            set
            {
                _texture2D = value;
                if (_texture2D)
                {
                    _image = _texture2D.ToSprite();
                }
            }
        }

        /// <summary>
        /// 图片
        /// </summary>
        public Sprite image
        {
            get
            {
                if (!_image)
                {
                    _image = _texture2D.ToSprite();
                }
                return _image;
            }
        }

        [NonSerialized]
        private Sprite _image;

        /// <summary>
        /// 可交互性
        /// </summary>
        public override bool interactable { get => unityObject; set { } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public UnityObjectModel() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unityObject"></param>
        public UnityObjectModel(T unityObject)
        {
            this.unityObject = unityObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="texture2D"></param>
        public UnityObjectModel(T component, string title, Texture2D texture2D) : this(component)
        {
            this.title = title;
            this.texture2D = texture2D;
        }

        /// <summary>
        /// 获取哈希Code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => unityObject ? unityObject.GetHashCode() : base.GetHashCode();

        /// <summary>
        /// 相等比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => (unityObject && obj is UnityObjectModel<T> model) ? unityObject == model.unityObject : base.Equals(obj); 

        #region IModelKeyValue

        /// <summary>
        /// 关键字列表
        /// </summary>
        public override IEnumerable<string> keys
        {
            get
            {
                if (_keyArray == null)
                {
                    var list = new List<string>();
                    list.AddRange(base.keys);
                    list.Add(nameof(title));
                    list.Add(nameof(texture2D));
                    _keyArray = list.ToArray();
                }
                return _keyArray;
            }
        }

        /// <summary>
        /// 关键字列表
        /// </summary>
        protected string[] _keyArray;

        /// <summary>
        /// 尝试获取模型键值类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool TryGetModelKeyValueType(string key, out Type type)
        {
            if (base.TryGetModelKeyValueType(key, out type)) return true;

            switch (key)
            {
                case nameof(title):
                    {
                        type = typeof(string);
                        return true;
                    }
                case nameof(texture2D):
                    {
                        type = typeof(Texture2D);
                        return true;
                    }
                default:
                    {
                        type = default;
                        return false;
                    }
            }
        }

        /// <summary>
        /// 尝试获取模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetModelKeyValue(string key, out object value)
        {
            if (base.TryGetModelKeyValue(key, out value)) return true;

            switch (key)
            {
                case nameof(title):
                    {
                        value = title;
                        return true;
                    }
                case nameof(texture2D):
                    {
                        value = texture2D;
                        return true;
                    }
                default:
                    {
                        value = default;
                        return false;
                    }
            }
        }

        /// <summary>
        /// 尝试设置模型键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetModelKeyValue(string key, object value)
        {
            if (base.TrySetModelKeyValue(key, value)) return true;

            switch (key)
            {
                case nameof(title):
                    {
                        if (Converter.instance.TryConvertTo<string>(value, out var result))
                        {
                            title = result;
                            return true;
                        }
                        break;
                    }
                case nameof(texture2D):
                    {
                        if (Converter.instance.TryConvertTo<Texture2D>(value, out var result))
                        {
                            texture2D = result;
                            return true;
                        }
                        return true;
                    }
            }
            return false;
        }

        #endregion
    }

    #endregion

    #region 组件模型

    /// <summary>
    ///  组件模型模版
    /// </summary>
    public class ComponentModel<T> : UnityObjectModel<T> where T : Component
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ComponentModel() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        public ComponentModel(T component) : base(component) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="texture2D"></param>
        public ComponentModel(T component, string title, Texture2D texture2D = null) : base(component, title, texture2D) { }

        /// <summary>
        /// 游戏对象
        /// </summary>
        public virtual GameObject gameObject => unityObject ? unityObject.gameObject : null;

        /// <summary>
        /// 组件
        /// </summary>
        public Component component => unityObject;
    }

    #endregion

    /// <summary>
    /// 列表宿主：用于注入到【列表视图】中作为主对象
    /// </summary>
    public interface IListHost : IModelHost 
    {
        /// <summary>
        /// 是否选择
        /// </summary>
        /// <param name="list"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        bool IsSelected(IList list, object model);
    }
}

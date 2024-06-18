using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using UnityEngine.Serialization;
using XCSJ.Extension.Base;
using XCSJ.Extension.Base.Helpers;
using XCSJ.Extension.Base.Attributes;

namespace XCSJ.PluginSMS.States.Base
{
    /// <summary>
    /// 对象集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TObject"></typeparam>
    public abstract class ObjectSet<T, TObject> : StateComponent<T>, IObjectSet<TObject>
        where T : ObjectSet<T, TObject>
        where TObject : UnityEngine.Object
    {
        /// <summary>
        /// 包含自身
        /// </summary>
        [Readonly(EEditorMode.Runtime)]
        [Name("包含自身")]
        [Tip("运行初始化时，会从当前状态所在游戏对象上查找符合要求的对象；", "When running initialization, it will find qualified objects from the game object in the current state;")]
        public bool includeSelf = false;

        /// <summary>
        /// 对象集
        /// </summary>
        [Name("对象集")]
        [SerializeField]
        //[Readonly(EEditorMode.Runtime)]
        [FormerlySerializedAs(nameof(IObjectSet.objects))]
        [ObjectPopup]
        [Array]
        public List<TObject> _objects = new List<TObject>();

        /// <summary>
        /// 对象列表
        /// </summary>
        public virtual List<TObject> objects => _objects;

        List<Object> IObjectSet.objects => _objects.ToList(o => (Object)o);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            //移除无效项
            RemoveInvalidObject();
            return base.Init(data);
        }

        /// <summary>
        /// 去重方式添加对象；支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        /// <param name="obj"></param>
        public void Add(TObject obj)
        {
            if (CommonFun.ObjectIsNull(obj)) return;
            this.XModifyProperty(() =>
            {
                _objects.AddWithDistinct(obj);
            });
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Remove(TObject obj) => _objects.Remove(obj);

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Contains(TObject obj) => _objects.Contains(obj);

        /// <summary>
        /// 去重对象
        /// </summary>
        public void DistinctObjects()
        {
            _objects = objects.Distinct().ToList();
        }

        /// <summary>
        /// 移除无效对象
        /// </summary>
        public void RemoveInvalidObject()
        {
            _objects.RemoveAll(CommonFun.ObjectIsNull);
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return includeSelf || (objects.Count > 0 && objects.All(obj => !CommonFun.ObjectIsNull(obj)));
        }


        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return (objects.Count == 1 && objects[0]) ? objects[0].name : objects.Count + "个";
        }
    }

    /// <summary>
    /// 对象集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectSet<T> : IObjectSet
    {
        /// <summary>
        /// 对象集
        /// </summary>
        new List<T> objects { get; }
    }

    /// <summary>
    /// 对象集
    /// </summary>
    public interface IObjectSet
    {
        /// <summary>
        /// 对象集
        /// </summary>
        List<UnityEngine.Object> objects { get; }
    }
}

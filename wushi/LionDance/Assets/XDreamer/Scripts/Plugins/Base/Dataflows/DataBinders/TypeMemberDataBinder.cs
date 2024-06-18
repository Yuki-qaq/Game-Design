using System;
using System.Reflection;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;

namespace XCSJ.Extension.Base.Dataflows.DataBinders
{
    /// <summary>
    /// 基础类型成员数据绑定器
    /// </summary>
    public abstract class BaseTypeMemberDataBinder : DataBinder, ITypeMemberDataBinder
    {
        /// <summary>
        /// 主类型
        /// </summary>
        public override Type mainType => typeMemberBinder?.mainType;

        /// <summary>
        /// 成员名
        /// </summary>
        public override string memberName
        {
            get => typeMemberBinder?.memberName;
            set
            {
                if (typeMemberBinder != null)
                {
                    typeMemberBinder.memberName = value;
                }
            }
        }

        /// <summary>
        /// 反射标记量:用于获取成员时使用
        /// </summary>
        public override BindingFlags bindingFlags => typeMemberBinder != null ? typeMemberBinder.bindingFlags : base.bindingFlags;

        /// <summary>
        /// 包含基础类型
        /// </summary>
        public override bool includeBaseType => typeMemberBinder != null ? typeMemberBinder.includeBaseType : base.includeBaseType;

        /// <summary>
        /// 类型成员绑定器对象
        /// </summary>
        public ITypeMemberBinder typeMemberBinder { get; private set; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="typeMemberBinder"></param>
        public virtual void Init(ITypeMemberBinder typeMemberBinder)
        {
            this.typeMemberBinder = typeMemberBinder;
        } 

        /// <summary>
        /// 设置数据绑定器的主体对象 ：主要用于动态绑定真实数据项
        /// </summary>
        /// <param name="mainObject">主体对象</param>
        public bool SetMainObject(object mainObject)
        {
            // 类型不匹配
            if (mainObject != null && !typeMemberBinder.mainType.IsAssignableFrom(mainObject.GetType()))
            {
                return false;
            }

            typeMemberBinder.mainObject = mainObject;
            return true;
        }
    }

    /// <summary>
    /// 类型成员数据绑定器 : 使用反射方式获取和设置值
    /// 识别类型成员主体对象和成员对象是否具有触发事件接口，如果有，则绑定
    /// </summary>
    public class TypeMemberDataBinder : BaseTypeMemberDataBinder
    {
        /// <summary>
        /// 属性或字段
        /// </summary>
        protected bool IsFieldOrProperty => typeMemberBinder.memberInfo is FieldInfo || typeMemberBinder.memberInfo is PropertyInfo;
    }

    /// <summary>
    /// 类型成员数据绑定器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TypeMemberDataBinder<T> : TypeMemberDataBinder where T : class
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        public T target => typeMemberBinder.mainObject as T;
    }

    /// <summary>
    /// 缺省类型成员数据绑定器
    /// </summary>
    [Serializable]
    [DataBinder(typeof(object))]
    [Name("缺省类型成员数据绑定器")]
    public class DefaultTypeMemberDataBinder : TypeMemberDataBinder<object> { }
}

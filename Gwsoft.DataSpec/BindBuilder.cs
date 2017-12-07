using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace Gwsoft.DataSpec
{
    /// <summary>
    /// 特定类型的数据绑定表达式缓存
    /// </summary>
    public class BindBuilder
    {
        private BindBuilder()
        {

        }

        /// <summary>
        /// 自定义的绑定词典(在数据绑定时调用)
        /// </summary>
        internal static readonly ThreadSafeDictionary<string, Func<Stream, long, object, PropertyBindState>> CustomBindDict = new ThreadSafeDictionary<string, Func<Stream, long, object, PropertyBindState>>();

        /// <summary>
        /// 类型自定义绑定属性个数
        /// </summary>
        internal static readonly ThreadSafeDictionary<Type, bool> TypeBindDict = new ThreadSafeDictionary<Type, bool>();

        private static BindBuilder _instance = null;

        /// <summary>
        /// 获取系统内置绑定构建实例
        /// </summary>
        /// <returns></returns>
        public static BindBuilder Instance()
        {
            if (_instance == null)
            {
                //_instance = new BindBuilder();
                System.Threading.Interlocked.CompareExchange<BindBuilder>(ref _instance, new BindBuilder(), null);
            }
            return _instance;
        }

        /// <summary>
        /// 全局属性绑定词典
        /// </summary>
        public static ThreadSafeDictionary<string, Func<Stream, long, object, PropertyBindState>> GlobalBindDictinary
        {
            get { return CustomBindDict; }
        }

        /// <summary>
        /// 获取实例属性的绑定键值
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        /// <param name="pInfo">实例相关属性</param>
        /// <returns></returns>
        public static string GetPropertyBindKey(Type instanceType, PropertyInfo pInfo)
        {
            return string.Format("{0}:{1}", instanceType.FullName, pInfo.Name);
        }

        /// <summary>
        /// 添加属性绑定获取表达式
        /// </summary>
        /// <typeparam name="TEntity">实例类型</typeparam>
        /// <typeparam name="TProp">实例类型的的属性方法</typeparam>
        /// <param name="expression">获取绑定属性的Lambda表达式</param>
        /// <param name="bindAct">获取绑定属性值的委托</param>
        /// <returns></returns>
        public BindBuilder Add<TEntity, TProp>(Expression<Func<TEntity, TProp>> expression, Func<Stream, long, object, PropertyBindState> bindAct)
        {
            Type EntityType = typeof(TEntity);
            if (!TypeBindDict.ContainsKey(EntityType))
            {
                PropertyInfo prop = (PropertyInfo)((MemberExpression)(expression.Body)).Member;
                string propKey = GetPropertyBindKey(EntityType, prop);
                if (!CustomBindDict.ContainsKey(propKey))
                {
                    CustomBindDict.Add(propKey, bindAct);
                }
            }
            return this;
        }

        /// <summary>
        /// 完成特定类型的属性绑定加载
        /// </summary>
        /// <typeparam name="TEntity">特定类型</typeparam>
        public void End<TEntity>()
        {
            Type EntityType = this.GetType();
            if (!TypeBindDict.ContainsKey(EntityType))
            {
                TypeBindDict.Add(EntityType, true);
            }
        }
    }

    /// <summary>
    /// 属性绑定状态
    /// </summary>
    [Serializable]
    public struct PropertyBindState
    {
        /// <summary>
        /// 当前绑定属性的值
        /// </summary>
        public object PropertyValue { get; set; }

        /// <summary>
        /// 是否通过Stream映射绑定
        /// </summary>
        public bool StreamBind { get; set; }
    }

}

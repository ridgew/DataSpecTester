using System;
using System.Collections.Generic;
using System.Text;

namespace Gwsoft.DataSpec
{
    /// <summary>
    /// 绑定上下文
    /// </summary>
    public class ESPContext
    {
        private ThreadSafeDictionary<string, object> _items = new ThreadSafeDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 在绑定上下文中存储键值
        /// </summary>
        /// <param name="key">键(不区分大小写)</param>
        /// <param name="val">值</param>
        public void SetItem(string key, object val)
        {
            if (_items.ContainsKey(key))
            {
                _items[key] = val;
            }
            else
            {
                _items.Add(key, val);
            }
        }

        /// <summary>
        /// 获取绑定上下文中存储键的值
        /// </summary>
        /// <param name="key">键(不区分大小写)</param>
        /// <returns></returns>
        public object GetItem(string key)
        {
            if (_items.ContainsKey(key))
            {
                return _items[key];
            }
            else
            {
                return null;
            }
        }
    }
}

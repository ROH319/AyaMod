using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyaMod.Common
{
    /// <summary>
    /// 支持一对多/多对一的双向字典
    /// 特点：一个Key可关联多个Value，一个Value也可关联多个Key
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class MultiBidirectionalDictionary<TKey, TValue>
    {
        // 正向映射：Key → 多个Value（用HashSet保证Value不重复）
        private readonly Dictionary<TKey, HashSet<TValue>> _forwardMap;
        // 反向映射：Value → 多个Key（用HashSet保证Key不重复）
        private readonly Dictionary<TValue, HashSet<TKey>> _reverseMap;

        /// <summary>
        /// 初始化双向字典
        /// </summary>
        public MultiBidirectionalDictionary()
        {
            _forwardMap = new Dictionary<TKey, HashSet<TValue>>();
            _reverseMap = new Dictionary<TValue, HashSet<TKey>>();
        }

        #region 核心操作：添加关联
        /// <summary>
        /// 添加Key和Value的关联关系（自动去重）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Add(TKey key, TValue value)
        {
            // 校验参数非空（值类型无需校验，引用类型做空校验）
            if (key == null)
                throw new ArgumentNullException(nameof(key), "Key不能为null");
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Value不能为null");

            // 正向映射：若Key不存在则新建HashSet，否则添加Value（自动去重）
            if (!_forwardMap.ContainsKey(key))
            {
                _forwardMap[key] = new HashSet<TValue>();
            }
            _forwardMap[key].Add(value);

            // 反向映射：若Value不存在则新建HashSet，否则添加Key（自动去重）
            if (!_reverseMap.ContainsKey(value))
            {
                _reverseMap[value] = new HashSet<TKey>();
            }
            _reverseMap[value].Add(key);
        }

        /// <summary>
        /// 批量添加一个Key对应的多个Value
        /// </summary>
        public void AddRange(TKey key, IEnumerable<TValue> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (var value in values)
            {
                Add(key, value);
            }
        }
        #endregion

        #region 核心操作：查询关联
        /// <summary>
        /// 通过Key获取所有关联的Value（返回只读集合，避免外部修改内部数据）
        /// </summary>
        /// <returns>无关联则返回空集合</returns>
        public IReadOnlyCollection<TValue> GetValuesByKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _forwardMap.TryGetValue(key, out var values)
                ? new List<TValue>(values).AsReadOnly() // 返回副本的只读包装
                : Array.Empty<TValue>();
        }

        /// <summary>
        /// 通过Value获取所有关联的Key（返回只读集合）
        /// </summary>
        /// <returns>无关联则返回空集合</returns>
        public IReadOnlyCollection<TKey> GetKeysByValue(TValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return _reverseMap.TryGetValue(value, out var keys)
                ? new List<TKey>(keys).AsReadOnly()
                : Array.Empty<TKey>();
        }

        /// <summary>
        /// 检查Key和Value是否存在关联
        /// </summary>
        public bool Contains(TKey key, TValue value)
        {
            if (key == null || value == null)
                return false;

            return _forwardMap.TryGetValue(key, out var values) && values.Contains(value);
        }
        #endregion

        #region 核心操作：移除关联
        /// <summary>
        /// 移除指定Key和Value的关联关系
        /// </summary>
        /// <returns>是否成功移除</returns>
        public bool Remove(TKey key, TValue value)
        {
            if (key == null || value == null)
                return false;

            // 先移除正向映射中的关联
            bool forwardRemoved = false;
            if (_forwardMap.TryGetValue(key, out var values))
            {
                forwardRemoved = values.Remove(value);
                // 如果该Key已无关联Value，直接删除Key
                if (values.Count == 0)
                {
                    _forwardMap.Remove(key);
                }
            }

            // 再移除反向映射中的关联
            bool reverseRemoved = false;
            if (_reverseMap.TryGetValue(value, out var keys))
            {
                reverseRemoved = keys.Remove(key);
                // 如果该Value已无关联Key，直接删除Value
                if (keys.Count == 0)
                {
                    _reverseMap.Remove(value);
                }
            }

            return forwardRemoved && reverseRemoved;
        }

        /// <summary>
        /// 移除指定Key的所有关联关系
        /// </summary>
        public void RemoveKey(TKey key)
        {
            if (key == null)
                return;

            // 先拿到该Key关联的所有Value，再逐个移除反向映射
            if (_forwardMap.TryGetValue(key, out var values))
            {
                foreach (var value in values.ToList()) // ToList避免遍历中修改集合
                {
                    Remove(key, value);
                }
            }
        }

        /// <summary>
        /// 移除指定Value的所有关联关系
        /// </summary>
        public void RemoveValue(TValue value)
        {
            if (value == null)
                return;

            if (_reverseMap.TryGetValue(value, out var keys))
            {
                foreach (var key in keys.ToList())
                {
                    Remove(key, value);
                }
            }
        }
        #endregion

        #region 辅助操作
        /// <summary>
        /// 清空所有关联关系
        /// </summary>
        public void Clear()
        {
            _forwardMap.Clear();
            _reverseMap.Clear();
        }

        /// <summary>
        /// 获取所有Key的数量
        /// </summary>
        public int KeyCount => _forwardMap.Count;

        /// <summary>
        /// 获取所有Value的数量
        /// </summary>
        public int ValueCount => _reverseMap.Count;

        public bool KeyContains(TKey key)
        {
            if (key == null)
                return false;
            return _forwardMap.ContainsKey(key);
        }

        public bool ValueContains(TValue value)
        {
            if (value == null)
                return false;
            return _reverseMap.ContainsKey(value);
        }
        #endregion
    }
}

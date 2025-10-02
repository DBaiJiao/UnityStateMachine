using System;
using UnityEngine;

namespace UnityUtilities.GenericRepository
{
    /// <summary>
    /// 仓库项目基础抽象类
    /// </summary>
    [Serializable]
    public abstract class BaseRepositoryItem : IRepositoryItem
    {
        [SerializeField] protected string id;
        [SerializeField] protected string displayName;
        [SerializeField] protected string createdAtString;
        [SerializeField] protected string lastModifiedString;
        
        public string Id => id;
        public string DisplayName => displayName;
        
        public DateTime CreatedAt 
        { 
            get 
            { 
                if (DateTime.TryParse(createdAtString, out DateTime result))
                    return result;
                return DateTime.MinValue;
            } 
        }
        
        public DateTime LastModified 
        { 
            get 
            { 
                if (DateTime.TryParse(lastModifiedString, out DateTime result))
                    return result;
                return DateTime.MinValue;
            }
            set 
            { 
                lastModifiedString = value.ToString(); 
            } 
        }
        
        protected BaseRepositoryItem()
        {
            id = Guid.NewGuid().ToString();
            displayName = "New Item";
            var now = DateTime.Now;
            createdAtString = now.ToString();
            lastModifiedString = now.ToString();
        }
        
        protected BaseRepositoryItem(string displayName) : this()
        {
            this.displayName = displayName;
        }
        
        protected BaseRepositoryItem(string id, string displayName)
        {
            this.id = id;
            this.displayName = displayName;
            var now = DateTime.Now;
            createdAtString = now.ToString();
            lastModifiedString = now.ToString();
        }
        
        /// <summary>
        /// 设置显示名称
        /// </summary>
        /// <param name="name">新的显示名称</param>
        public virtual void SetDisplayName(string name)
        {
            displayName = name;
            LastModified = DateTime.Now;
        }
        
        /// <summary>
        /// 抽象克隆方法，子类必须实现
        /// </summary>
        /// <returns>克隆的对象</returns>
        public abstract IRepositoryItem Clone();
        
        /// <summary>
        /// 验证数据是否有效，子类可以重写
        /// </summary>
        /// <returns>是否有效</returns>
        public virtual bool IsValid()
        {
            return !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(displayName);
        }
        
        public override string ToString()
        {
            return $"{GetType().Name}: {displayName} (ID: {id})";
        }
        
        public override bool Equals(object obj)
        {
            if (obj is BaseRepositoryItem other)
            {
                return id == other.id;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return id?.GetHashCode() ?? 0;
        }
    }
}

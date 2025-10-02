using System;
using UnityEngine;

namespace UnityUtilities.GenericRepository
{
    /// <summary>
    /// 仓库项目基础接口
    /// </summary>
    public interface IRepositoryItem
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// 显示名称
        /// </summary>
        string DisplayName { get; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreatedAt { get; }
        
        /// <summary>
        /// 最后修改时间
        /// </summary>
        DateTime LastModified { get; set; }
        
        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <returns>克隆的对象</returns>
        IRepositoryItem Clone();
        
        /// <summary>
        /// 验证数据是否有效
        /// </summary>
        /// <returns>是否有效</returns>
        bool IsValid();
    }
}

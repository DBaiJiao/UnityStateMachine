using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities.GenericRepository
{
    /// <summary>
    /// 泛型仓库接口
    /// </summary>
    /// <typeparam name="T">必须实现IRepositoryItem接口的类型</typeparam>
    public interface IRepository<T> where T : class, IRepositoryItem
    {
        /// <summary>
        /// 仓库中的项目数量
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// 获取所有项目
        /// </summary>
        /// <returns>所有项目的集合</returns>
        IEnumerable<T> GetAll();
        
        /// <summary>
        /// 根据ID获取项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns>找到的项目，未找到返回null</returns>
        T GetById(string id);
        
        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="item">要添加的项目</param>
        /// <returns>是否添加成功</returns>
        bool Add(T item);
        
        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="item">要更新的项目</param>
        /// <returns>是否更新成功</returns>
        bool Update(T item);
        
        /// <summary>
        /// 根据ID删除项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns>是否删除成功</returns>
        bool Remove(string id);
        
        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="item">要删除的项目</param>
        /// <returns>是否删除成功</returns>
        bool Remove(T item);
        
        /// <summary>
        /// 检查是否存在指定ID的项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns>是否存在</returns>
        bool Exists(string id);
        
        /// <summary>
        /// 清空仓库
        /// </summary>
        void Clear();
        
        /// <summary>
        /// 根据条件查找项目
        /// </summary>
        /// <param name="predicate">查找条件</param>
        /// <returns>匹配的项目集合</returns>
        IEnumerable<T> FindWhere(Func<T, bool> predicate);
        
        /// <summary>
        /// 保存仓库数据
        /// </summary>
        void Save();
        
        /// <summary>
        /// 加载仓库数据
        /// </summary>
        void Load();
        
        /// <summary>
        /// 项目添加事件
        /// </summary>
        event Action<T> OnItemAdded;
        
        /// <summary>
        /// 项目更新事件
        /// </summary>
        event Action<T> OnItemUpdated;
        
        /// <summary>
        /// 项目删除事件
        /// </summary>
        event Action<T> OnItemRemoved;
        
        /// <summary>
        /// 仓库清空事件
        /// </summary>
        event Action OnRepositoryCleared;
    }
}

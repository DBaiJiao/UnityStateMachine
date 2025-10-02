using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

namespace UnityUtilities.GenericRepository
{
    /// <summary>
    /// 泛型仓库实现类
    /// </summary>
    /// <typeparam name="T">必须实现IRepositoryItem接口的类型</typeparam>
    [Serializable]
    public class GenericRepository<T> : IRepository<T> where T : class, IRepositoryItem
    {
        [SerializeField] private List<T> items = new List<T>();
        [SerializeField] private string repositoryName;
        [SerializeField] private bool autoSave = true;
        
        private string saveFilePath;
        
        public int Count => items.Count;
        
        // 事件
        public event Action<T> OnItemAdded;
        public event Action<T> OnItemUpdated;
        public event Action<T> OnItemRemoved;
        public event Action OnRepositoryCleared;
        
        public GenericRepository(string repositoryName = null, bool autoSave = true)
        {
            this.repositoryName = repositoryName ?? typeof(T).Name + "Repository";
            this.autoSave = autoSave;
            SetupSaveFilePath();
        }
        
        private void SetupSaveFilePath()
        {
            string saveDirectory = Path.Combine(Application.persistentDataPath, "Repositories");
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            saveFilePath = Path.Combine(saveDirectory, $"{repositoryName}.json");
        }
        
        public IEnumerable<T> GetAll()
        {
            return items.ToList(); // 返回副本以防止外部修改
        }
        
        public T GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
                
            return items.FirstOrDefault(item => item.Id == id);
        }
        
        public bool Add(T item)
        {
            if (item == null || !item.IsValid())
            {
                Debug.LogWarning($"[{repositoryName}] 尝试添加无效项目");
                return false;
            }
            
            if (Exists(item.Id))
            {
                Debug.LogWarning($"[{repositoryName}] ID为 {item.Id} 的项目已存在");
                return false;
            }
            
            items.Add(item);
            OnItemAdded?.Invoke(item);
            
            if (autoSave)
                Save();
                
            Debug.Log($"[{repositoryName}] 成功添加项目: {item.DisplayName}");
            return true;
        }
        
        public bool Update(T item)
        {
            if (item == null || !item.IsValid())
            {
                Debug.LogWarning($"[{repositoryName}] 尝试更新无效项目");
                return false;
            }
            
            int index = items.FindIndex(i => i.Id == item.Id);
            if (index == -1)
            {
                Debug.LogWarning($"[{repositoryName}] 未找到ID为 {item.Id} 的项目");
                return false;
            }
            
            item.LastModified = DateTime.Now;
            items[index] = item;
            OnItemUpdated?.Invoke(item);
            
            if (autoSave)
                Save();
                
            Debug.Log($"[{repositoryName}] 成功更新项目: {item.DisplayName}");
            return true;
        }
        
        public bool Remove(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;
                
            T item = GetById(id);
            if (item == null)
                return false;
                
            return Remove(item);
        }
        
        public bool Remove(T item)
        {
            if (item == null)
                return false;
                
            bool removed = items.Remove(item);
            if (removed)
            {
                OnItemRemoved?.Invoke(item);
                
                if (autoSave)
                    Save();
                    
                Debug.Log($"[{repositoryName}] 成功删除项目: {item.DisplayName}");
            }
            
            return removed;
        }
        
        public bool Exists(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;
                
            return items.Any(item => item.Id == id);
        }
        
        public void Clear()
        {
            items.Clear();
            OnRepositoryCleared?.Invoke();
            
            if (autoSave)
                Save();
                
            Debug.Log($"[{repositoryName}] 已清空仓库");
        }
        
        public IEnumerable<T> FindWhere(Func<T, bool> predicate)
        {
            if (predicate == null)
                return new List<T>();
                
            return items.Where(predicate).ToList();
        }
        
        public void Save()
        {
            try
            {
                var wrapper = new RepositoryWrapper<T>
                {
                    items = items,
                    repositoryName = repositoryName,
                    savedAt = DateTime.Now.ToString()
                };
                
                string json = JsonUtility.ToJson(wrapper, true);
                File.WriteAllText(saveFilePath, json);
                
                Debug.Log($"[{repositoryName}] 数据已保存到: {saveFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[{repositoryName}] 保存失败: {e.Message}");
            }
        }
        
        public void Load()
        {
            try
            {
                if (!File.Exists(saveFilePath))
                {
                    Debug.LogWarning($"[{repositoryName}] 保存文件不存在: {saveFilePath}");
                    return;
                }
                
                string json = File.ReadAllText(saveFilePath);
                var wrapper = JsonUtility.FromJson<RepositoryWrapper<T>>(json);
                
                if (wrapper != null && wrapper.items != null)
                {
                    items = wrapper.items;
                    Debug.Log($"[{repositoryName}] 成功加载 {items.Count} 个项目");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[{repositoryName}] 加载失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 设置自动保存
        /// </summary>
        /// <param name="autoSave">是否自动保存</param>
        public void SetAutoSave(bool autoSave)
        {
            this.autoSave = autoSave;
        }
        
        /// <summary>
        /// 获取仓库统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        public RepositoryStats GetStats()
        {
            return new RepositoryStats
            {
                TotalItems = Count,
                RepositoryName = repositoryName,
                LastSaveTime = File.Exists(saveFilePath) ? File.GetLastWriteTime(saveFilePath) : DateTime.MinValue
            };
        }
        
        /// <summary>
        /// 备份仓库数据
        /// </summary>
        /// <param name="backupPath">备份路径</param>
        public void Backup(string backupPath = null)
        {
            if (string.IsNullOrEmpty(backupPath))
            {
                backupPath = saveFilePath.Replace(".json", $"_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            }
            
            try
            {
                if (File.Exists(saveFilePath))
                {
                    File.Copy(saveFilePath, backupPath);
                    Debug.Log($"[{repositoryName}] 备份完成: {backupPath}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[{repositoryName}] 备份失败: {e.Message}");
            }
        }
    }
    
    /// <summary>
    /// 用于JSON序列化的包装类
    /// </summary>
    [Serializable]
    public class RepositoryWrapper<T> where T : class, IRepositoryItem
    {
        [SerializeField] public List<T> items;
        [SerializeField] public string repositoryName;
        [SerializeField] public string savedAt;
    }
    
    /// <summary>
    /// 仓库统计信息
    /// </summary>
    [Serializable]
    public class RepositoryStats
    {
        public int TotalItems;
        public string RepositoryName;
        public DateTime LastSaveTime;
        
        public override string ToString()
        {
            return $"仓库: {RepositoryName}, 项目数: {TotalItems}, 最后保存: {LastSaveTime:yyyy-MM-dd HH:mm:ss}";
        }
    }
}

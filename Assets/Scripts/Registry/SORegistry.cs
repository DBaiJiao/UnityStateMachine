using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

// 注册表，用于封装某些SO对象。
[CreateAssetMenu(menuName = "Game/Registry/Registry")]
public class Registry : ScriptableObject
{
    public List<ScriptableObject> allObjects; // 所有对象
    

    // 获取所有对象
    public List<ScriptableObject> GetAllObjects()
    {
        return allObjects;
    }

    // 构建主键到对象的映射，主键由外部指定
    public Dictionary<string, T> GetKeyToObjectMap<T>(System.Func<T, string> keySelector) where T : ScriptableObject
    {
        Dictionary<string, T> keyToObjectMap = new Dictionary<string, T>();
        foreach (var obj in allObjects)
        {
            string key = keySelector(obj as T);
            keyToObjectMap[key] = obj as T;
        }
        return keyToObjectMap;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

// 注册表，用于封装某些SO对象。
[CreateAssetMenu(menuName = "Game/Registry/GORegistry")]
public class GORegistry : ScriptableObject
{
    public List<GameObject> allObjects; // 所有对象
    

    // 获取所有对象
    public List<GameObject> GetAllObjects()
    {
        return allObjects;
    }

    // 构建主键到对象的映射，主键由外部指定
    public Dictionary<string, GameObject> GetKeyToObjectMap(System.Func<GameObject, string> keySelector)
    {
        Dictionary<string, GameObject> keyToObjectMap = new Dictionary<string, GameObject>();
        foreach (var obj in allObjects)
        {
            string key = keySelector(obj);
            keyToObjectMap[key] = obj;
        }
        return keyToObjectMap;
    }
}

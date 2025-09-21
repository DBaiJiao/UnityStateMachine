using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum UILayer
{
    Bot, // 底层。比如角色血条或游戏常驻UI
    Mid, // 中层。普通面板
    Top, // 顶层。比如设置、背包等
    System // 系统。比如弹窗、提示等
}

public class UIManager : MonoBehaviour
{
    // 单例模式
    public static UIManager Instance;

    // 一些存储
    private Dictionary<string, AsyncOperationHandle<GameObject>> panelAssetHandles; // 存储所有载入过的面板Asset的Handle。Key: 面板的Addressable地址, Value: 加载操作的Handle
    private Dictionary<string, BasePanel> panelCache; // 存储已创建的面板
    private Dictionary<UILayer, RectTransform> layerDict; // UI的层级父节点
    public Stack<BasePanel> panelStack; // 面板栈, 用于管理面板的返回功能
    public Transform uiRoot; // UI根节点。在Inspector中设置


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }



    // UIManager的初始化
    public void Init()
    {
        // 初始化存储
        panelAssetHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        panelCache = new Dictionary<string, BasePanel>();
        panelStack = new Stack<BasePanel>();

        // 初始化层级
        layerDict = new Dictionary<UILayer, RectTransform>();
        // 为每个层级创建一个空的GameObject作为父节点
        foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
        {
            // 创建空GameObject，并添加RectTransform组件
            string name = layer.ToString();
            GameObject go = new GameObject(name);
            RectTransform rectTransform = go.AddComponent<RectTransform>();
            go.transform.SetParent(uiRoot);

            // 初始化 RectTransform 为全屏拉伸
            rectTransform.anchorMin = Vector2.zero;    // 左下角锚点
            rectTransform.anchorMax = Vector2.one;     // 右上角锚点
            rectTransform.offsetMin = Vector2.zero;    // 左边和底边的偏移
            rectTransform.offsetMax = Vector2.zero;    // 右边和顶边的偏移
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;

            // 存储到字典中
            layerDict[layer] = rectTransform;
        }

        // 预加载一些常用面板(预加载可以大大减少游戏中加载时间，并且体感可感受区别)
        PreloadPanel("Panel_Attribute");
    }

    // 显示面板
    public async void OpenPanel(string panelAddress, UILayer layer = UILayer.Mid, params object[] args)
    {
        BasePanel panel;

        // 如果面板已存在，直接显示
        if (panelCache.TryGetValue(panelAddress, out panel))
        {
            panel.OnShow();

            // 将面板入栈（可选）
            panelStack.Push(panel);
        }
        else
        {
            // 面板不存在，异步创建新面板
            AsyncOperationHandle<GameObject> handle;

            // 是否在之前加载过该资源
            if (!panelAssetHandles.TryGetValue(panelAddress, out handle))
            {
                // 没有加载过，发起异步加载请求
                handle = Addressables.LoadAssetAsync<GameObject>(panelAddress);
                panelAssetHandles.Add(panelAddress, handle);
            }

            // 等待加载完成或直接获取已完成的资源
            GameObject panelPrefab = await handle.Task;

            // 检查资源是否有效
            if (panelPrefab == null)
            {
                // 资源无效，移除记录
                Debug.LogError($"[UIManager] Failed to load panel prefab at address: {panelAddress}");
                Addressables.Release(handle); // 释放Handle
                panelAssetHandles.Remove(panelAddress);
                return;
            }

            // 检查Prefab是否包含BasePanel组件
            panel = panelPrefab.GetComponent<BasePanel>();
            if (panel == null)
            {
                // 资源不包含BasePanel组件，移除记录
                Debug.LogError($"[UIManager] The prefab at {panelAddress} does not have a BasePanel component.");
                Addressables.Release(handle);
                panelAssetHandles.Remove(panelAddress);
                return;
            }

            // 实例化面板并设置父节点
            GameObject panelGo = Instantiate(panelPrefab, layerDict[layer]);

            // 设置缩放
            panelGo.transform.localScale = Vector3.one;

            // 重新获取实例的BasePanel组件并初始化
            panel = panelGo.GetComponent<BasePanel>();
            
            // 初始化面板
            panel.Init(args);
            panelCache.Add(panelAddress, panel); // 加入已实例化字典
            panel.OnShow();

            // 将面板入栈（可选）
            panelStack.Push(panel);
        }


        Debug.Log($"Time after show:{Time.time}");
    }

    // 关闭面板
    public void ClosePanel(string panelAddress)
    {
        BasePanel panel;

        if (panelCache.TryGetValue(panelAddress, out panel))
        {
            // 面板存在，隐藏
            panel.OnHide();

            // 从栈中移除
            var tempStack = new Stack<BasePanel>();
            while (panelStack.Count > 0)
            {
                var p = panelStack.Pop();
                if (p != panel)
                {
                    tempStack.Push(p);
                }
            }
            while (tempStack.Count > 0)
            {
                panelStack.Push(tempStack.Pop());
            }

            // 从缓存中移除
            panelCache.Remove(panelAddress);

            // 销毁面板对象
            Destroy(panel.gameObject);
        }
        else
        {
            Debug.LogWarning($"[UIManager] Attempted to close non-existent panel: {panelAddress}");
        }

        Debug.Log($"Time after close:{Time.time}");
    }

    // 预加载面板资源
    public async void PreloadPanel(string panelAddress)
    {
        AsyncOperationHandle<GameObject> handle;

        // 如果已经加载过，直接返回
        if (panelAssetHandles.TryGetValue(panelAddress, out handle))
        {
            Debug.LogWarning($"[UIManager] Panel at {panelAddress} is already preloaded.");
            return;
        }

        // 没有加载过，发起异步加载请求
        handle = Addressables.LoadAssetAsync<GameObject>(panelAddress);
        panelAssetHandles.Add(panelAddress, handle);
        // 注册进度更新回调
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"[UIManager] Preloaded panel asset at: {panelAddress}");
            }
            else
            {
                Debug.LogError($"[UIManager] Failed to preload panel asset at address: {panelAddress}");
                panelAssetHandles.Remove(panelAddress);
            }
        };
        // 等待加载完成
        await handle.Task;
    }

    // 获取某个面板
    public T GetPanel<T>(string panelName) where T: BasePanel
    {
        BasePanel panel;
        if (panelCache.TryGetValue(panelName, out panel))
        {
            return panel as T;
        }
        else
        {
            Debug.LogWarning($"[UIManager] Attempted to get non-existent panel: {panelName}");
            return null;
        }
    }

    // 清理所有UI面板和资源
    public void ClearAll()
    {
        // 关闭并销毁所有面板实例
        foreach (var panel in panelCache.Values)
        {
            panel.OnHide();
            Destroy(panel.gameObject);
        }
        panelCache.Clear();
        panelStack.Clear();

        // 释放所有已加载的Addressables资源
        foreach (var handlePair in panelAssetHandles)
        {
            Addressables.Release(handlePair.Value);
        }
        panelAssetHandles.Clear();

        Debug.Log("[UIManager] Cleared all UI panels and released resources.");
    }

    // 每帧更新
    public void Update()
    {
        foreach (var panel in panelCache.Values)
        {
            if (panel.gameObject.activeInHierarchy)
            {
                // 只有激活的面板才调用更新
                panel.OnUpdate();
            }
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum UILayer
{
    Bot, // �ײ㡣�����ɫѪ������Ϸ��פUI
    Mid, // �в㡣��ͨ���
    Top, // ���㡣�������á�������
    System // ϵͳ�����絯������ʾ��
}

public class UIManager : MonoBehaviour
{
    // ����ģʽ
    public static UIManager Instance;

    // һЩ�洢
    private Dictionary<string, AsyncOperationHandle<GameObject>> panelAssetHandles; // �洢��������������Asset��Handle��Key: ����Addressable��ַ, Value: ���ز�����Handle
    private Dictionary<string, BasePanel> panelCache; // �洢�Ѵ��������
    private Dictionary<UILayer, RectTransform> layerDict; // UI�Ĳ㼶���ڵ�
    public Stack<BasePanel> panelStack; // ���ջ, ���ڹ������ķ��ع���
    public Transform uiRoot; // UI���ڵ㡣��Inspector������


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



    // UIManager�ĳ�ʼ��
    public void Init()
    {
        // ��ʼ���洢
        panelAssetHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        panelCache = new Dictionary<string, BasePanel>();
        panelStack = new Stack<BasePanel>();

        // ��ʼ���㼶
        layerDict = new Dictionary<UILayer, RectTransform>();
        // Ϊÿ���㼶����һ���յ�GameObject��Ϊ���ڵ�
        foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
        {
            // ������GameObject�������RectTransform���
            string name = layer.ToString();
            GameObject go = new GameObject(name);
            RectTransform rectTransform = go.AddComponent<RectTransform>();
            go.transform.SetParent(uiRoot);

            // ��ʼ�� RectTransform Ϊȫ������
            rectTransform.anchorMin = Vector2.zero;    // ���½�ê��
            rectTransform.anchorMax = Vector2.one;     // ���Ͻ�ê��
            rectTransform.offsetMin = Vector2.zero;    // ��ߺ͵ױߵ�ƫ��
            rectTransform.offsetMax = Vector2.zero;    // �ұߺͶ��ߵ�ƫ��
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;

            // �洢���ֵ���
            layerDict[layer] = rectTransform;
        }

        // Ԥ����һЩ�������(Ԥ���ؿ��Դ�������Ϸ�м���ʱ�䣬������пɸ�������)
        PreloadPanel("Panel_Attribute");
    }

    // ��ʾ���
    public async void OpenPanel(string panelAddress, UILayer layer = UILayer.Mid, params object[] args)
    {
        BasePanel panel;

        // �������Ѵ��ڣ�ֱ����ʾ
        if (panelCache.TryGetValue(panelAddress, out panel))
        {
            panel.OnShow();

            // �������ջ����ѡ��
            panelStack.Push(panel);
        }
        else
        {
            // ��岻���ڣ��첽���������
            AsyncOperationHandle<GameObject> handle;

            // �Ƿ���֮ǰ���ع�����Դ
            if (!panelAssetHandles.TryGetValue(panelAddress, out handle))
            {
                // û�м��ع��������첽��������
                handle = Addressables.LoadAssetAsync<GameObject>(panelAddress);
                panelAssetHandles.Add(panelAddress, handle);
            }

            // �ȴ�������ɻ�ֱ�ӻ�ȡ����ɵ���Դ
            GameObject panelPrefab = await handle.Task;

            // �����Դ�Ƿ���Ч
            if (panelPrefab == null)
            {
                // ��Դ��Ч���Ƴ���¼
                Debug.LogError($"[UIManager] Failed to load panel prefab at address: {panelAddress}");
                Addressables.Release(handle); // �ͷ�Handle
                panelAssetHandles.Remove(panelAddress);
                return;
            }

            // ���Prefab�Ƿ����BasePanel���
            panel = panelPrefab.GetComponent<BasePanel>();
            if (panel == null)
            {
                // ��Դ������BasePanel������Ƴ���¼
                Debug.LogError($"[UIManager] The prefab at {panelAddress} does not have a BasePanel component.");
                Addressables.Release(handle);
                panelAssetHandles.Remove(panelAddress);
                return;
            }

            // ʵ������岢���ø��ڵ�
            GameObject panelGo = Instantiate(panelPrefab, layerDict[layer]);

            // ��������
            panelGo.transform.localScale = Vector3.one;

            // ���»�ȡʵ����BasePanel�������ʼ��
            panel = panelGo.GetComponent<BasePanel>();
            
            // ��ʼ�����
            panel.Init(args);
            panelCache.Add(panelAddress, panel); // ������ʵ�����ֵ�
            panel.OnShow();

            // �������ջ����ѡ��
            panelStack.Push(panel);
        }


        Debug.Log($"Time after show:{Time.time}");
    }

    // �ر����
    public void ClosePanel(string panelAddress)
    {
        BasePanel panel;

        if (panelCache.TryGetValue(panelAddress, out panel))
        {
            // �����ڣ�����
            panel.OnHide();

            // ��ջ���Ƴ�
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

            // �ӻ������Ƴ�
            panelCache.Remove(panelAddress);

            // ����������
            Destroy(panel.gameObject);
        }
        else
        {
            Debug.LogWarning($"[UIManager] Attempted to close non-existent panel: {panelAddress}");
        }

        Debug.Log($"Time after close:{Time.time}");
    }

    // Ԥ���������Դ
    public async void PreloadPanel(string panelAddress)
    {
        AsyncOperationHandle<GameObject> handle;

        // ����Ѿ����ع���ֱ�ӷ���
        if (panelAssetHandles.TryGetValue(panelAddress, out handle))
        {
            Debug.LogWarning($"[UIManager] Panel at {panelAddress} is already preloaded.");
            return;
        }

        // û�м��ع��������첽��������
        handle = Addressables.LoadAssetAsync<GameObject>(panelAddress);
        panelAssetHandles.Add(panelAddress, handle);
        // ע����ȸ��»ص�
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
        // �ȴ��������
        await handle.Task;
    }

    // ��ȡĳ�����
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

    // ��������UI������Դ
    public void ClearAll()
    {
        // �رղ������������ʵ��
        foreach (var panel in panelCache.Values)
        {
            panel.OnHide();
            Destroy(panel.gameObject);
        }
        panelCache.Clear();
        panelStack.Clear();

        // �ͷ������Ѽ��ص�Addressables��Դ
        foreach (var handlePair in panelAssetHandles)
        {
            Addressables.Release(handlePair.Value);
        }
        panelAssetHandles.Clear();

        Debug.Log("[UIManager] Cleared all UI panels and released resources.");
    }

    // ÿ֡����
    public void Update()
    {
        foreach (var panel in panelCache.Values)
        {
            if (panel.gameObject.activeInHierarchy)
            {
                // ֻ�м�������ŵ��ø���
                panel.OnUpdate();
            }
        }
    }

}

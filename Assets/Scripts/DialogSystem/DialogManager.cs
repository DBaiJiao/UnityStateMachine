using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话管理器
/// </summary>
public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [Header("对话UI")]
    [SerializeField] private DialogUI dialogPanel; // 对话UI

    [Header("对话数据")]
    [SerializeField] private DialogData currentDialogData; // 对话数据
    [SerializeField] private DialogData.DialogNode currentDialogNode; // 当前对话节点


    private void Awake()
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

    private void Start()
    {
        InitDialogManager();
    }

    /// <summary>
    /// 初始化对话管理器
    /// </summary>
    private void InitDialogManager()
    {
        // 初始化对话UI
        if (dialogPanel == null)
        {
            Debug.LogError("[DialogManager] DialogUI is not set.");
        }
        else
        {
            dialogPanel.InitDialogUI();
        }
    }

    /// <summary>
    /// 开始一段对话
    /// </summary>
    /// <param name="dialogData"></param>
    public void StartDialog(DialogData dialogData)
    {
        this.currentDialogData = dialogData;
        this.currentDialogNode = dialogData.dialogNodes[dialogData.startNodeId];

        // 将参数传给对话UI，显示对话UI
        dialogPanel?.ShowNode(currentDialogNode);


    }

    /// <summary>
    /// 显示下一个对话节点
    /// </summary>
    public void GoToNextNode()
    {
        // 检查是否是结束节点
        if (currentDialogNode.isEndNode)
        {
            Debug.Log($"[DialogManager] Dialog end.");
            dialogPanel?.CloseDialogUI();
            return;
        }

        // 检查下一个节点ID是否有效
        int nextNodeId = currentDialogNode.defaultNextNodeId;
        if (nextNodeId < 0 || nextNodeId >= currentDialogData.dialogNodes.Count)
        {
            Debug.LogError("[DialogManager] Next node ID is out of range.");
            return;
        }

        // 显示下一个对话节点
        this.currentDialogNode = currentDialogData.dialogNodes[nextNodeId];
        dialogPanel?.ShowNode(currentDialogNode);
    }
}
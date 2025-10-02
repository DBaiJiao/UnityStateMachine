using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 对话数据
/// </summary>
[CreateAssetMenu(menuName = "Game/Dialog/DialogData")]
[Serializable]
public class DialogData : ScriptableObject
{
    [Header("对话基本信息")]
    public int dialogId; // 对话ID
    public int startNodeId; // 开始节点ID
    
    [Header("对话节点列表")]
    [SerializeField]
    public List<DialogNode> dialogNodes = new List<DialogNode>(); // 对话节点

    /// <summary>
    /// 对话节点
    /// </summary>
    [Serializable]
    public class DialogNode
    {
        [Header("节点基本信息")]
        public int nodeId; // 节点ID
        public int defaultNextNodeId; // 默认下一个节点ID
        public bool isEndNode = false; // 是否是结束节点

        [Header("对话内容")]
        public string characterName = ""; // 角色名称
        public Sprite characterImage; // 角色立绘
        [TextArea(3, 6)]
        public string dialogText = ""; // 对话文本

        [Header("对话选项")]
        public List<DialogOption> dialogOptions = new List<DialogOption>(); // 对话选项

        /// <summary>
        /// 对话选项
        /// </summary>
        [Serializable]
        public class DialogOption
        {
            [Header("选项内容")]
            public string optionText = ""; // 选项文本
            public int nextNodeId = -1; // 下一个节点ID
        }
    }
}

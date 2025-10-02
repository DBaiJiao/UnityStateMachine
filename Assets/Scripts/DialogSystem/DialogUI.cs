using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 对话UI
/// </summary>
public class DialogUI : MonoBehaviour
{
    // 在Inspector中设置UI组件
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI dialogText; // 对话文本
    [SerializeField] private TextMeshProUGUI characterName; // 角色名称
    [SerializeField] private Image characterImage; // 角色立绘
    [SerializeField] private Button continueButton; // 继续按钮

    /// <summary>
    /// 初始化对话UI
    /// </summary>
    public void InitDialogUI()
    {
        this.continueButton.onClick.AddListener(OnContinueButtonClick);

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示对话节点
    /// </summary>
    /// <param name="dialogNode"></param>
    public void ShowNode(DialogData.DialogNode dialogNode)
    {
        this.dialogText.text = dialogNode.dialogText;
        this.characterName.text = dialogNode.characterName;
        this.characterImage.sprite = dialogNode.characterImage;

        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭对话UI
    /// </summary>
    public void CloseDialogUI()
    {
        this.dialogText.text = "";
        this.characterName.text = "";
        this.characterImage.sprite = null;

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 继续按钮点击事件
    /// </summary>
    private void OnContinueButtonClick()
    {
        DialogManager.Instance.GoToNextNode();
    }
    
}
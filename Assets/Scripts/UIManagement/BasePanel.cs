using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    // 在面板初始化时由UIManager调用
    public virtual void Init(params object[] args) { }

    // 在面板展示之前(SetActive(true)之前)调用
    public virtual void OnShow() { }

    // 在面板隐藏之后(SetActive(false)之后)调用
    public virtual void OnHide() { }

    // 在面板每帧更新时调用
    public virtual void OnUpdate() { }

    // 关闭面板的方法
    public virtual void Close()
    {
        UIManager.Instance.ClosePanel(this.GetType().Name);
    }

}

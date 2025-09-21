using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    // ������ʼ��ʱ��UIManager����
    public virtual void Init(params object[] args) { }

    // �����չʾ֮ǰ(SetActive(true)֮ǰ)����
    public virtual void OnShow() { }

    // ���������֮��(SetActive(false)֮��)����
    public virtual void OnHide() { }

    // �����ÿ֡����ʱ����
    public virtual void OnUpdate() { }

    // �ر����ķ���
    public virtual void Close()
    {
        UIManager.Instance.ClosePanel(this.GetType().Name);
    }

}

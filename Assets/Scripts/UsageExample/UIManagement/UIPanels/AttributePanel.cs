using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributePanel : BasePanel
{

    public override void Init(params object[] args)
    {
        base.Init(args);
        // ��ʼ������
        Debug.Log("[AttributePanel] AttributePanel Init");
    }

    public override void OnShow()
    {
        base.OnShow();
        // ��ʾ���ʱ�Ĵ���
        Debug.Log("[AttributePanel] AttributePanel OnShow");
    }
}

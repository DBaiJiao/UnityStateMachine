using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributePanel : BasePanel
{

    public override void Init(params object[] args)
    {
        base.Init(args);
        // 初始化代码
        Debug.Log("[AttributePanel] AttributePanel Init");
    }

    public override void OnShow()
    {
        base.OnShow();
        // 显示面板时的代码
        Debug.Log("[AttributePanel] AttributePanel OnShow");
    }
}

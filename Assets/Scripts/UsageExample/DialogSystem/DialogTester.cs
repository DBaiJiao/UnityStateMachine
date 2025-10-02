using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogTester : MonoBehaviour
{
    [Header("测试用例")]
    public List<DialogData> dialogDatas;

    private int currentDialogIndex = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DialogManager.Instance.StartDialog(dialogDatas[1]);
            currentDialogIndex++;
        }
    }

}

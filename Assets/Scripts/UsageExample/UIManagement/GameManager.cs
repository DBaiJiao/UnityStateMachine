using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log($"Time before show:{Time.time}");
            UIManager.Instance.OpenPanel("Panel_Attribute", UILayer.Bot);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log($"Time before close:{Time.time}");
            UIManager.Instance.ClosePanel("Panel_Attribute");
        }
    }
}

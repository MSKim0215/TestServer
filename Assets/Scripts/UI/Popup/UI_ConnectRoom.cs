using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConnectRoom : MonoBehaviour
{
    private Transform inputFileds;
    private Transform buttons;

    private TMP_InputField input_host, input_port, input_name;
    private Button btn_connect, btn_return;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        inputFileds = transform.Find("Background/InputFields");
        input_host = inputFileds.Find("Group_Host").GetComponentInChildren<TMP_InputField>();
        input_port = inputFileds.Find("Group_Port").GetComponentInChildren<TMP_InputField>();
        input_name = inputFileds.Find("Group_Name").GetComponentInChildren<TMP_InputField>();

        buttons = transform.Find("Background/Buttons");
        btn_connect = buttons.Find("Btn_Connect").GetComponent<Button>();
        btn_return = buttons.Find("Btn_Return").GetComponent<Button>();

        btn_connect.onClick.AddListener(() => OnConnect());
        btn_return.onClick.AddListener(() => OnReturn());
    }

    private void OnConnect()
    {
        Client client = Managers.System.Client;
        if (client == null || client.IsSocketReady) return;
        client.ConnectedToServer();
    }

    private void OnReturn()
    {
        Managers.UI.ClosePopupUI();
    }
}

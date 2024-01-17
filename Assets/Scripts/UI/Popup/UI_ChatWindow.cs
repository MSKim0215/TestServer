using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChatWindow : MonoBehaviour
{
    private Transform top_window, send_window;

    private TextMeshProUGUI tmp_name, tmp_count;
    private TMP_InputField input_send;

    private Button btn_exit, btn_send;

    private void Awake()
    {
        top_window = transform.Find("Background/Top Window");
        send_window = transform.Find("Background/Send Window");

        tmp_name = top_window.Find("TMP_Name").GetComponent<TextMeshProUGUI>();
        tmp_count = top_window.Find("TMP_Count").GetComponent<TextMeshProUGUI>();
        input_send = send_window.Find("Input_Send").GetComponent<TMP_InputField>();

        btn_exit = top_window.Find("Btn_Exit").GetComponent<Button>();
        btn_send = send_window.Find("Btn_Send").GetComponent<Button>();
    }

    public void Init(string roomName, int roomNumber, int personCount)
    {
        tmp_name.text = $"{roomName} ({roomNumber})";
        tmp_count.text = $"참여 인원: {personCount}명";

        btn_exit.onClick.AddListener(() => OnExit());
        btn_send.onClick.AddListener(() => OnSend());
    }

    private void OnExit()
    {
        Client client = Managers.System.Client;
        client?.CloseSocket();

        Managers.UI.ClosePopupUI();
    }

    private void OnSend()
    {
        string message = input_send.text;
        if (message == string.Empty) return;

        Client client = Managers.System.Client;
        if(client != null)
        {
            client.Send(message);
        }
    }
}

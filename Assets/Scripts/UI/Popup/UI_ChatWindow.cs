using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChatWindow : MonoBehaviour
{
    private Transform top_window;

    private TextMeshProUGUI tmp_name, tmp_count;

    private Button btn_exit;

    private void Awake()
    {
        top_window = transform.Find("Background/Top Window");

        tmp_name = top_window.Find("TMP_Name").GetComponent<TextMeshProUGUI>();
        tmp_count = top_window.Find("TMP_Count").GetComponent<TextMeshProUGUI>();

        btn_exit = top_window.Find("Btn_Exit").GetComponent<Button>();
    }

    public void Init(int roomName, int personCount)
    {
        tmp_name.text = $"방 번호: {roomName}";
        tmp_count.text = $"참여 인원: {personCount}명";

        btn_exit.onClick.AddListener(() => OnExit());
    }

    private void OnExit()
    {
        Managers.UI.ClosePopupUI();
    }
}

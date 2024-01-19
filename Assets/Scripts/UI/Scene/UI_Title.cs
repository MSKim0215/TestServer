using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Title : MonoBehaviour
{
    private Transform mainMenu;

    private TextMeshProUGUI tmp_title;
    private TextMeshProUGUI tmp_version;

    private Button btn_createRoom, btn_connectRoom;

    private void Awake()
    {
        mainMenu = transform.Find("MainMenu");

        tmp_title = transform.Find("TMP_Title").GetComponent<TextMeshProUGUI>();
        tmp_version = mainMenu.transform.Find("TMP_Version").GetComponent<TextMeshProUGUI>();
        btn_createRoom = mainMenu.transform.Find("Btn_CreateRoom").GetComponent<Button>();
        btn_connectRoom = mainMenu.transform.Find("Btn_ConnectRoom").GetComponent<Button>();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        tmp_title.text = "TCP통신 채팅 프로그램";

        // 프로그램 버전 세팅
        tmp_version.text = Application.version;

        // 버튼 이벤트 할당
        btn_createRoom.onClick.AddListener(() => { OnCreate(); });
        btn_connectRoom.onClick.AddListener(() => { OnConnect(); });
    }

    private void OnCreate()
    {
        Managers.UI.ShowPopupUI("UI_CreateMenu");
    }

    private void OnConnect()
    {
        Managers.UI.ShowPopupUI("UI_ConnectRoom");
    }
}

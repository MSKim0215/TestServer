using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class Client : MonoBehaviour
{
    private GameObject chatContainer;
    private GameObject messagePrefab;

    private string clientName;

    private bool isSocketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private void Start()
    {
        chatContainer = GameObject.Find("Chat Window");
        messagePrefab = Resources.Load<GameObject>("Prefabs/Message");

        clientName = "이름";
    }

    /// <summary>
    /// 클라이언트가 서버에 연결되었을 때 호출되는 함수
    /// </summary>
    public void ConnectedToServer()
    {
        // 이미 연결되어 있다면 함수 종료
        if (isSocketReady) return;

        // 기본 호스트 / 포트 값
        string host = "127.0.0.1";
        int port = 215;

        // 입력된 값이 있다면 기본 호스트 / 포트 값을 덮어씀
        string ov_h;
        int ov_p;

        GameObject inputFileds = GameObject.Find("Canvas/Login/InputFields");

        TMP_InputField input_host = inputFileds.transform.Find("Input_Host").GetComponent<TMP_InputField>();
        ov_h = input_host.text;
        if(ov_h != "")
        {
            host = ov_h;
        }

        TMP_InputField input_port = inputFileds.transform.Find("Input_Port").GetComponent<TMP_InputField>();
        int.TryParse(input_port.text, out ov_p);
        if(ov_p != 0)
        {
            port = ov_p;
        }

        // 소켓 생성
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            isSocketReady = true;
        }
        catch(Exception e)
        {
            Debug.Log($"소켓 에러: {e.Message}");
        }
    }

    private void Update()
    {
        if(isSocketReady)
        {
            if(stream.DataAvailable)
            {   // 소켓의 입력 스트림에 읽을 수 있는 데이터가 있을 경우
                string data = reader.ReadLine();
                if(data != null)
                {
                    OnIncommingData(data);
                }
            }
        }
    }

    /// <summary>
    /// 서버로부터 수신된 데이터를 처리하는 함수
    /// </summary>
    /// <param name="data">수신된 데이터</param>
    private void OnIncommingData(string data)
    {
        if(data == "%NAME")
        {
            Send($"&NAME | {clientName}");  // 서버에 &NAME 문자열과 클라이언트 이름을 결합하여 전송
            return;
        }

        // 일반 메시지일 경우
        GameObject messageBox = Instantiate(messagePrefab, chatContainer.transform);
        messageBox.GetComponentInChildren<TextMeshProUGUI>().text = data;
    }

    /// <summary>
    /// 연결된 소켓을 통해 서버로 데이터를 전송하는 함수
    /// </summary>
    /// <param name="data">데이터</param>
    private void Send(string data)
    {
        if (!isSocketReady) return;

        writer.WriteLine(data);     // 지정된 데이터를 한 줄로 전송
        writer.Flush();             // 출력 스트림에 있는 모든 데이터를 즉시 전송
    }

    /// <summary>
    /// 전송 버튼 이벤트 함수
    /// </summary>
    private void OnSend()
    {
        string message = GameObject.Find("Input_Send").GetComponent<TMP_InputField>().text;
        Send(message);
    }

    /// <summary>
    /// 소켓을 종료하는 함수
    /// </summary>
    private void CloseSocket()
    {
        if (!isSocketReady) return;

        writer.Close();
        reader.Close();
        socket.Close();
        isSocketReady = false;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }
}

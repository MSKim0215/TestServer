using System;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Client
{
    private Transform chatContainer;

    private string clientName;

    private bool isSocketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public bool isHost = false;

    public TcpClient Socket { get => socket; }

    public bool IsSocketReady { get => isSocketReady; }

    public string Host { get => Managers.System.Host; set => Managers.System.Host = value; }
    public int Port { get => Managers.System.Port; set => Managers.System.Port = value; }

    public void Init()
    {
        // Scene UI 생성
        Managers.UI.ShowSceneUI("UI_Title");
    }

    /// <summary>
    /// 클라이언트가 서버에 연결되었을 때 호출되는 함수
    /// </summary>
    public void ConnectedToServer()
    {
        // 이미 연결되어 있다면 함수 종료
        if (isSocketReady) return;

        // 기본 호스트/포트 번호, 닉네임
        Host = "127.0.0.1";
        Port = 215;
        clientName = $"Guest{UnityEngine.Random.Range(0, 10001)}";

        // 입력된 값이 있다면 기본 호스트/포트 번호, 닉네임을 덮어씀
        string ov_h;
        int ov_p;
        string ov_n;

        GameObject inputFileds = GameObject.Find("UI_CreateMenu/Background/InputFields");
        if(inputFileds == null)
        {
            inputFileds = GameObject.Find("UI_ConnectRoom/Background/InputFields");
        }

        TMP_InputField input_host = inputFileds.transform.Find("Group_Host/Input_Host").GetComponent<TMP_InputField>();
        ov_h = input_host.text;
        if(ov_h != "")
        {
            Host = ov_h;
        }

        TMP_InputField input_port = inputFileds.transform.Find("Group_Port/Input_Port").GetComponent<TMP_InputField>();
        int.TryParse(input_port.text, out ov_p);
        if(ov_p != 0)
        {
            Port = ov_p;
        }

        TMP_InputField input_name = inputFileds.transform.Find("Group_Name/Input_Name").GetComponent<TMP_InputField>();
        ov_n = input_name.text;
        if(ov_n != "")
        {
            clientName = ov_n;
        }

        // 소켓 생성
        try
        {
            socket = new TcpClient(Host, Port);
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

    public void OnUpdate()
    {
        if(IsSocketReady)
        {
            if(stream.DataAvailable)
            {   // 소켓의 입력 스트림에 읽을 수 있는 데이터가 있다면 실행
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
        if (data.Contains("%INFO"))
        {
            if(!isHost)
            {
                string[] info = data.Split('|');
                UI_ChatWindow window = Managers.UI.ShowPopupUI("UI_ChatWindow").GetComponent<UI_ChatWindow>();
                window.Init(info[1], int.Parse(info[3]), int.Parse(info[2]));
                return;
            }
        }

        if (data == "%NAME")
        {
            Send($"&NAME | {clientName}");  // 서버에 &NAME 문자열과 클라이언트 이름을 결합하여 전송
            return;
        }

        if(chatContainer == null)
        {
            ScrollRect chat_window = UnityEngine.Object.FindObjectOfType<UI_ChatWindow>().transform.Find("Background/Chat Window").GetComponent<ScrollRect>();
            chatContainer = chat_window.content;
        }

        if(data.Contains(":"))
        {   // 일반 메시지일 경우
            GameObject speech = default;

            string[] datas = data.Split(':');
            if (datas[0].Contains(clientName))
            {   // 자기 자신이라면 owenr_speech 사용
                speech = UnityEngine.Object.Instantiate(Managers.Resource.SpeechLoad(Define.SpeechType.Owner_Speech), chatContainer);
            }
            else
            {   // 자기 자신이 아니라면 other_speech 사용
                speech = UnityEngine.Object.Instantiate(Managers.Resource.SpeechLoad(Define.SpeechType.Other_Speech), chatContainer);
            }

            speech.transform.Find("Speech").GetComponentInChildren<TextMeshProUGUI>().text = datas[1];
        }
        else
        {   // 참가 메시지일 경우
            UnityEngine.Object.Instantiate(Managers.Resource.SpeechLoad(Define.SpeechType.NoticeMessage), chatContainer).GetComponentInChildren<TextMeshProUGUI>().text = data;
        }

        Fit();
        //Invoke("Fit", 0.03f);
    }

    private void Fit() => LayoutRebuilder.ForceRebuildLayoutImmediate(chatContainer.GetComponent<RectTransform>());

    /// <summary>
    /// 연결된 소켓을 통해 서버로 데이터를 전송하는 함수
    /// </summary>
    /// <param name="data">데이터</param>
    public void Send(string data)
    {
        if (!isSocketReady) return;

        writer.WriteLine(data);     // 지정된 데이터를 한 줄로 전송
        writer.Flush();             // 출력 스트림에 있는 모든 데이터를 즉시 전송
    }

    /// <summary>
    /// 소켓을 종료하는 함수
    /// </summary>
    public void CloseSocket()
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

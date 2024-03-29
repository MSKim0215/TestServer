using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class Server
{
    private List<ServerClient> connectList;         // 연결된 클라이언트 목록
    private List<ServerClient> disconnectList;      // 연결 해제된 클라이언트 목록

    private string roomName = string.Empty;

    private TcpListener server;     // TCP 네트워크 클라이언트에서 연결을 수신
    private bool serverStarted;     // 서버 시작 체크

    private int personCount;

    public int PersonCount { get => connectList.Count; set => personCount = value; }

    public int Port { get => Managers.System.Port; set => Managers.System.Port = value; }
    public string RoomName { get => roomName; set => roomName = value; }

    public bool ServerStarted { get => serverStarted; }

    public void Init()
    {
        connectList = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        // 서버 연결 시도
        try
        {
            GameObject inputFileds = GameObject.Find("UI_CreateMenu/Background/InputFields");
            TMP_InputField input_port = inputFileds.transform.Find("Group_Port/Input_Port").GetComponent<TMP_InputField>();
            int ov_p;
            int.TryParse(input_port.text, out ov_p);
            if (ov_p != 0)
            {
                Port = ov_p;
            }

            TMP_InputField input_room = inputFileds.transform.Find("Group_Room/Input_Room").GetComponent<TMP_InputField>();
            string ov_r = input_room.text;
            if(ov_r != string.Empty)
            {
                roomName = ov_r;
            }

            // IPAdress.Any: 모든 네트워크 인터페이스에서 들어오는 연결을 수락하도록 설정한다.
            server = new TcpListener(IPAddress.Any, Port);
            server.Start();     // 들어오는 연결 요청의 수신을 시작

            StartListening();
            serverStarted = true;

            Debug.Log($"포트 번호 {Port}로 서버가 시작되었습니다!");
        }
        catch (Exception e)
        {
            Debug.Log($"소켓 에러: {e.Message}");
        }
    }

    /// <summary>
    /// 들어오는 연결 시도를 받아들이는 비동기 작업을 시작하는 함수
    /// </summary>
    private void StartListening()
    {
        // 비동기적으로 클라이언트의 연결을 수락하고, 연결이 수락되면 콜백함수가 실행된다.
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    /// <summary>
    /// 클라이언트의 연결이 수락되어 실행되는 콜백 함수
    /// </summary>
    /// <param name="result">비동기 작업의 상태를 나타내는 인터페이스</param>
    private void AcceptTcpClient(IAsyncResult result)
    {
        TcpListener listener = (TcpListener)result.AsyncState;      // 연결이 수락된 클라이언트

        // 들어오는 연결 시도를 비동기적으로 받아들이고 원격 호스트 통신을 처리할 새로운 TcpClient을 만든다.
        connectList.Add(new ServerClient(listener.EndAcceptTcpClient(result)));
        StartListening();

        // 연결된 클라이언트들에게 메시지를 보냄 (누군가 연결되었다고)
        Broadcast("%NAME", new List<ServerClient>() { connectList[connectList.Count - 1] });

        // TODO: 새로 연결된 클라이언트에게 현재 채팅방의 이름을 넘긴다.
        ServerClient connect_client = connectList[connectList.Count - 1];
        try
        {
            StreamWriter writer = new StreamWriter(connect_client.tcp.GetStream());
            writer.WriteLine("%INFO|" + RoomName + "|" + connectList.Count + "|" + Port);     // data 문자열을 클라이언트로 전송
            writer.Flush();                 // 쓰여진 데이터의 즉각적인 전송을 보장하기 위해 Flush를 StreamWriter에 호출
        }
        catch (Exception e)
        {
            Debug.Log($"쓰기 오류: {connect_client.clientName}로 부터 {e.Message} 메시지 전송");
        }
    }

    public void OnUpdate()
    {
        if (!serverStarted) return;

        foreach(ServerClient client in connectList)
        {
            if(!IsConnected(client.tcp))
            {   // 클라이언트가 연결을 유지하지 않았다면 실행
                if(!disconnectList.Contains(client))
                {   // 퇴장 목록에 존재하지 않다면 실행
                    client.tcp.Close();     // client tcp를 삭제하고, 기본 TCP 연결이 닫히도록 요청한다.
                    disconnectList.Add(client);
                }
                continue;
            }
            else
            {   // 클라이언트가 연결이 유지되고 있다면 실행
                NetworkStream stream = client.tcp.GetStream();      // 데이터를 보내고 받는데 사용되는 NetworkStream을 반환한다.
                if(stream.DataAvailable)
                {   // 데이터를 즉시 읽을 수 있는지 여부 체크
                    // 지정한 바이트 순서 표시 검색 옵션을 사용하여 지정된 스트림에 대해 StreamReader 클래스의 새 인스턴스를 초기화한다.
                    StreamReader reader = new StreamReader(stream, true);
                    string data = reader.ReadLine();
                    if(data != null)
                    {   // 데이터가 있다면 실행
                        OnIncomingData(client, data);
                    }
                }
            }
        }

        for (int i = 0; i < disconnectList.Count; i++)
        {
            Broadcast($"{disconnectList[i].clientName} 님이 퇴장했습니다.", connectList);

            connectList.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }

    /// <summary>
    /// 클라이언트가 여전히 연결되어 있는지 확인하는 함수
    /// </summary>
    /// <param name="client">주어진 클라이언트</param>
    /// <returns>연결 여부 체크</returns>
    private bool IsConnected(TcpClient client)
    {
        try
        {
            if (client != null && client.Client != null && client.Client.Connected)
            {   // 클라이언트가 null이 아니고, 연결이 되어있다면 실행
                // 소켓의 상태를 결정
                if (client.Client.Poll(0, SelectMode.SelectRead))   // 폴링 모드 값에 따른 Socket의 상태
                {   // SelectRead: 호출되고 연결이 보류 중이거나, 데이터를 읽을 수 있거나, 연결이 닫혔거나, 다시 설정되거나, 종료된 경우 true 반환
                    
                    // 데이터를 수신할 수 있는지 확인
                    // 지정된 SocketFlag를 사용하여 바인딩된 Socket에서 수신 버퍼로 데이터를 받는다.
                    // Peek 플래그를 사용하면 데이터를 읽지만, 버퍼에서 제거하지 않는다.
                    // 따라서 0을 반환하면, 데이터를 수신할 수 없다는 의미이다.
                    return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else return false; 
            // 1. TcpClient 객체가 null일 경우
            // 2. TcpClient 객체가 연결되어 있지 않을 경우
            // 3. 소켓의 상태가 SelectRead 상태가 아닐 경우
            // 4. 데이터를 수신할 수 없을 경우
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 클라이언트가 보낸 데이터를 처리하고, 상황에 맞게 다른 클라이언트에게 전달하는 함수
    /// </summary>
    /// <param name="client">클라이언트</param>
    /// <param name="data">데이터</param>
    private void OnIncomingData(ServerClient client, string data)
    {
        if(data.Contains("&NAME"))
        {   // &NAME이 포함되어 있는지 체크
            client.clientName = data.Split('|')[1];     // |를 기준으로 분리하여 1번째 배열을 클라이언트 이름으로 설정
            Broadcast($"{client.clientName}님이 참가했습니다.", connectList);  // 모든 연결된 클라이언트에게 해당 클라이언트가 연결되었다고 알림
            return;
        }

        Broadcast($"{client.clientName}: {data}", connectList);     // 해당 클라이언트의 메시지를 모든 연결된 클라이언트에게 전송
    }

    /// <summary>
    /// 연결된 클라이언트에게 지정된 문자열 데이터를 보내는 함수
    /// </summary>
    /// <param name="data">전송 데이터</param>
    /// <param name="clients">연결된 클라이언트</param>
    private void Broadcast(string data, List<ServerClient> clients)
    {
        foreach(ServerClient client in clients)
        {
            if (disconnectList.Contains(client)) continue;      // 퇴장한 사용자라면 무시

            try
            {
                // 클라이언트의 TcpClient 개체에서 얻은 스트림을 사용하여 StreamWriter 개체를 인스턴스화
                // StreamWriter는 클라이언트의 네트워크 스트림에 텍스트를 쓸 수 있게 해준다.
                StreamWriter writer = new StreamWriter (client.tcp.GetStream());
                writer.WriteLine(data);     // data 문자열을 클라이언트로 전송
                writer.Flush();             // 쓰여진 데이터의 즉각적인 전송을 보장하기 위해 Flush를 StreamWriter에 호출
            }
            catch (Exception e)
            {
                Debug.Log($"쓰기 오류: {client.clientName}로 부터 {e.Message} 메시지 전송");
            }
        }
    }
}

/// <summary>
/// 각 클라이언트의 연결 정보를 저장하는 클래스
/// </summary>
public class ServerClient
{
    public TcpClient tcp;               // 클라이언트 소켓
    public string clientName;           // 클라이언트 이름

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guset";       // 기본 클라이언트 이름을 Guest로 설정
        tcp = clientSocket;         // 클라이언트 소켓 설정
    }
}
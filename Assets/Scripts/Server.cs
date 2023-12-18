using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour
{
    private List<ServerClient> connectList;             // 연결된 클라이언트 목록
    private List<ServerClient> disconnectList;      // 연결 해제된 클라이언트 목록

    public int port = 215;      // 서버가 수신 대기할 포트 번호

    private TcpListener server;     // TCP 네트워크 클라이언트에서 연결을 수신
    private bool serverStarted;     // 서버 시작 체크

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        connectList = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        // 서버 연결 시도
        try
        {
            // IPAdress.Any: 모든 네트워크 인터페이스에서 들어오는 연결을 수락하도록 설정한다.
            server = new TcpListener(IPAddress.Any, port);
            server.Start();     // 들어오는 연결 요청의 수신을 시작

            StartListening();
            serverStarted = true;

            Debug.Log($"포트 번호 {port}로 서버가 시작되었습니다!");
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

        Debug.Log($"{connectList[connectList.Count - 1].clientName} 님이 채팅에 참가했습니다.");
    }

    private void Update()
    {
        if (!serverStarted) return;

        foreach(ServerClient client in connectList)
        {
            if(!IsConnected(client.tcp))
            {   // 클라이언트가 연결을 유지하지 않았다면 실행
                client.tcp.Close();     // client tcp를 삭제하고, 기본 TCP 연결이 닫히도록 요청한다.
                disconnectList.Add(client);
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
                if (client.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else return false;
        }
        catch
        {
            return false;
        }
    }

    private void OnIncomingData(ServerClient client, string data)
    {
        Debug.Log($"{client.clientName}: {data}");
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
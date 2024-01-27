using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager
{
    private Client client;
    private Server server;

    private string host;
    private int port;

    public Client Client { get => client; }
    public Server Server { get => server; }

    public string Host { get => host; set => host = value; }
    public int Port { get => port; set => port = value; }

    public void Init()
    {
        client = new Client();
        client.Init();
    }

    public void StartServer()
    {
        server = new Server();
    }

    public void OnUpdate()
    {
        if(Client.isHost)
        {
            Server?.OnUpdate();
        }

        Client?.OnUpdate();
    }
}

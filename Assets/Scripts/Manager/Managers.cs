using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers instance;

    private UIManager _ui = new UIManager();
    private SystemManager _system = new SystemManager();
    private ResourceManager _resource = new ResourceManager();
    private EventManager _event = new EventManager();

    public static UIManager UI => Instance._ui;
    public static SystemManager System => Instance._system;
    public static ResourceManager Resource => Instance._resource;
    public static EventManager Event => Instance._event;

    private static Managers Instance { get { Init(); return instance; } }

    private void Start()
    {
        Init();
    }

    private static void Init()
    {
        if(instance == null)
        {
            GameObject obj = GameObject.Find("@Managers");
            if(obj == null)
            {
                obj = new GameObject { name = "@Managers" };
                obj.AddComponent<Managers>();
            }

            DontDestroyOnLoad(obj);
            instance = obj.GetComponent<Managers>();

            System.Init();
            Resource.Init();
        }
    }

    private void Update()
    {
        System.OnUpdate();
    }

    private void OnApplicationQuit()
    {
        System.Client.CloseSocket();
    }
}

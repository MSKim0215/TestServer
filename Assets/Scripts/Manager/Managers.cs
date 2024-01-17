using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers instance;

    private UIManager ui = new UIManager();
    private SystemManager system = new SystemManager();

    public static UIManager UI => Instance.ui;
    public static SystemManager System => Instance.system;

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

            instance.system.Init();
        }
    }

    private void Update()
    {
        instance.system.OnUpdate();
    }
}

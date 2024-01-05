using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private Stack<GameObject> popupStack = new Stack<GameObject>();

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if(root == null)
            {
                root = new GameObject { name = "@UI_Root" };
            }
            return root;
        }
    }

    public GameObject ShowSceneUI(string name)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/UI/Scene/{name}");
        GameObject scene = Object.Instantiate(prefab);
        scene.transform.SetParent(Root.transform);
        scene.name = name;

        // 랜더 카메라 세팅
        scene.GetComponent<Canvas>().worldCamera = Camera.main;

        // sorting layer, order 세팅
        scene.GetComponent<Canvas>().sortingLayerName = "SceneUI";
        scene.GetComponent<Canvas>().sortingOrder = 0;

        return scene;
    }

    public GameObject ShowPopupUI(string name)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/UI/Popup/{name}");
        GameObject popup = Object.Instantiate(prefab);
        popup.transform.SetParent(Root.transform);
        popup.name = name;
        popupStack.Push(popup);

        // 랜더 카메라 세팅
        popup.GetComponent<Canvas>().worldCamera = Camera.main;

        // sorting layer, order 세팅
        popup.GetComponent<Canvas>().sortingLayerName = "PopupUI";
        popup.GetComponent<Canvas>().sortingOrder = popupStack.Count;

        return popup;
    }

    public void ClosePopupUI()
    {
        if (popupStack.Count <= 0) return;

        GameObject popup = popupStack.Pop();
        Object.Destroy(popup);
    }
}

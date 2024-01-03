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

    public GameObject ShowPopupUI(string name)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/UI/Popup/{name}");
        GameObject popup = Object.Instantiate(prefab);
        popup.transform.SetParent(Root.transform);
        popupStack.Push(popup);
        return popup;
    }
}

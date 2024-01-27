using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    // 이벤트를 수신받길 원하는 리스너 정보를 담고 있다.
    private Dictionary<Define.EventType, List<IListener>> listeners = new Dictionary<Define.EventType, List<IListener>>();

    public void AddListner(Define.EventType type, IListener listener)
    {
        List<IListener> listenList = null;

        // 이벤트 형식 키가 존재하는지 검사.
        // 존재한다면 리스트에 추가
        if(listeners.TryGetValue(type, out listenList))
        {
            listenList.Add(listener);
            return;
        }

        // 없으면 새로운 리스트 생성
        listenList = new List<IListener>();
        listenList.Add(listener);
        listeners.Add(type, listenList);    // 리스너 리스트에 추가
    }

    public void PostNotification<T>(Define.EventType type, T sender, object param = null)
    {
        List<IListener> listenList = null;

        if (!listeners.TryGetValue(type, out listenList)) return;
        
        for(int i = 0; i < listenList.Count; i++)
        {
            listenList?[i].OnEvent(type, sender, param);
        }
    }

    public void RemoveEvent(Define.EventType type) => listeners.Remove(type);

    public void RemoveRedundancies()
    {
        Dictionary<Define.EventType, List<IListener>> newListeners = new Dictionary<Define.EventType, List<IListener>>();

        foreach(KeyValuePair<Define.EventType, List<IListener>> item in listeners)
        {
            for (int i = item.Value.Count - 1; i >= 0; i--)
            {
                if (item.Value[i].Equals(null))
                {
                    item.Value.RemoveAt(i);
                }
            }

            if(item.Value.Count > 0)
            {
                newListeners.Add(item.Key, item.Value);
            }
        }

        listeners = newListeners;
    }

    private void OnClear()
    {
        RemoveRedundancies();
    }
}
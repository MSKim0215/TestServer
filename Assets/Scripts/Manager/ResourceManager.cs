using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager
{
    // 말풍선 오브젝트
    private Dictionary<Define.SpeechType, GameObject> prefab_speechDict = new Dictionary<Define.SpeechType, GameObject>();

    public void Init()
    {
        GameObject[] load_speechs = Resources.LoadAll<GameObject>("Prefabs/UI/Speech");
        for(int i = 0; i < load_speechs.Length; i++)
        {
            Define.SpeechType type = (Define.SpeechType)Enum.Parse(typeof(Define.SpeechType), load_speechs[i].name);
            prefab_speechDict.Add(type, load_speechs[i]);
        }
    }

    /// <summary>
    /// 말풍선 프리팹을 로드하는 함수
    /// </summary>
    /// <param name="type">말풍선 타입</param>
    /// <returns>해당 타입의 말풍선</returns>
    public GameObject SpeechLoad(Define.SpeechType type) => prefab_speechDict[type];
}

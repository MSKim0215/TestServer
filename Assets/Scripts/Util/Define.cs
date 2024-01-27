using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum SpeechType
    {
        Other_Speech, Owner_Speech, NoticeMessage
    }

    public enum EventType
    {
        Connect_Room,       // 방 참여 및 퇴장 이벤트
    }
}

public interface IListener
{
    // 이벤트가 발생할 때, 리스너에서 호출할 함수
    public void OnEvent<T>(Define.EventType type, T sender, object param = null);
}
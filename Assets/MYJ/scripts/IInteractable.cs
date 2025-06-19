using Unity.Multiplayer.Center.Common.Analytics;
using UnityEngine;

public enum InteractionType
{
    Instant,            //상호작용 시 즉시실행(문)
    GaugeThenTiming,    //타이밍 바를 실행하는 상호작용(채집)
    MiniGame            //미니게임을 실행하는 상호작용(낚시, 요리)
}

public interface IInteractable
{
    string GetCursorType(); //커서 이미지 바꾸기
    string GetInteractionID(); //무슨 오브젝트와 상호작용하는지 체크(개별)
    InteractionType GetInteractionType(); //상호작용 후 작동의 묶음 처리를 위한 타입


    void Interact();
    
}


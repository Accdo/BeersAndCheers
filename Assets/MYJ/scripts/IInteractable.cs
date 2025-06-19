using Unity.Multiplayer.Center.Common.Analytics;
using UnityEngine;

public enum InteractionType
{
    Instant,            //��ȣ�ۿ� �� ��ý���(��)
    GaugeThenTiming,    //Ÿ�̹� �ٸ� �����ϴ� ��ȣ�ۿ�(ä��)
    MiniGame            //�̴ϰ����� �����ϴ� ��ȣ�ۿ�(����, �丮)
}

public interface IInteractable
{
    string GetCursorType(); //Ŀ�� �̹��� �ٲٱ�
    string GetInteractionID(); //���� ������Ʈ�� ��ȣ�ۿ��ϴ��� üũ(����)
    InteractionType GetInteractionType(); //��ȣ�ۿ� �� �۵��� ���� ó���� ���� Ÿ��


    void Interact();
    
}


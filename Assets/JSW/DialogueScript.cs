using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Script")]
public class DialogueScript : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;    // 말하는 사람 이름
        public string text;           // 대화 내용
        public FoodItem recommendedFood;  // 추천 메뉴 정보 추가
    }

    public DialogueLine[] lines;      // 대화 내용 배열
} 
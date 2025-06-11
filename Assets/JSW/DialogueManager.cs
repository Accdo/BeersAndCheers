using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel; // 대화창 패널
    public TextMeshProUGUI dialogueText;        // 대화 텍스트
    public Button nextButton;        // 다음 버튼

    private string[] lines;
    private int currentLine;

    void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
        nextButton.onClick.AddListener(NextLine);
    }

    public void StartDialogue(string[] script)
    {
        lines = script;
        currentLine = 0;
        dialoguePanel.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        if (currentLine < lines.Length)
        {
            dialogueText.text = lines[currentLine];
        }
        else
        {
            EndDialogue();
        }
    }

    public void NextLine()
    {
        currentLine++;
        ShowLine();
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}

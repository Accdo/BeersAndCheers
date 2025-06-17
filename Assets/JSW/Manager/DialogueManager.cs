using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI interactableText;

    [Header("Input Settings")]
    public KeyCode nextDialogueKey = KeyCode.N;  // 다음 대화로 넘어가는 키

    private DialogueScript.DialogueLine[] lines;
    private int currentLine;
    private bool isInDialogue = false;

    private CustomerAI nearestCustomer = null;
    private float nearestDistance = float.MaxValue;
    private CustomerAI currentCustomer = null;  // 현재 대화 중인 손님

    void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
        if (interactableText != null)
        {
            interactableText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isInDialogue && Input.GetKeyDown(nextDialogueKey))
        {
            NextLine();
        }
    }

    public void StartDialogue(DialogueScript dialogueScript)
    {
        if (isInDialogue) return;

        lines = dialogueScript.lines;
        currentLine = 0;
        isInDialogue = true;
        dialoguePanel.SetActive(true);
        ShowLine();
        
        // 대화를 시작하는 손님 저장 및 애니메이션 설정
        currentCustomer = nearestCustomer;
        if (currentCustomer != null)
        {
            currentCustomer.anim.SetFloat("SitMotion", 1f);
        }
    }

    void ShowLine()
    {
        if (currentLine < lines.Length)
        {
            var currentDialogue = lines[currentLine];
            
            // 대화 내용 표시
            dialogueText.text = currentDialogue.text;
            
            // 말하는 사람 이름 표시
            if (speakerNameText != null)
            {
                speakerNameText.text = currentDialogue.speakerName;
            }
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
        isInDialogue = false;
        currentLine = 0;
        
        // 대화가 끝날 때 애니메이션을 원래대로 되돌림
        if (currentCustomer != null)
        {
            currentCustomer.anim.SetFloat("SitMotion", 0f);
            currentCustomer = null;
        }
    }

    public void ShowInteractableText(bool show, CustomerAI customer, float distance, string interactText = null)
    {
        interactableText.text = $"상호 작용 [{customer.interactKey}]";
        if (interactableText != null)
        {
            // 더 가까운 손님이 있으면 해당 손님의 텍스트만 표시
            if (show && (nearestCustomer == null || distance < nearestDistance))
            {
                nearestCustomer = customer;
                nearestDistance = distance;
                interactableText.gameObject.SetActive(true);
            }
            // 현재 손님이 가장 가까운 손님이 아니면 텍스트를 끔
            else if (!show && nearestCustomer == customer)
            {
                nearestCustomer = null;
                nearestDistance = float.MaxValue;
                interactableText.gameObject.SetActive(false);
            }
        }
       
    }
}
using UnityEngine;
using TMPro;
using System.Collections;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI;  // The panel
    [SerializeField] private TMP_Text dialogueText;  // The text field

    [TextArea(3, 10)]
    [SerializeField] private string[] dialogueLines; // NPC lines

    [SerializeField] private float typingSpeed = 0.05f; // speed of typing

    private int currentLineIndex = 0;
    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;
    private bool hasTalked = false;

    private Coroutine typingCoroutine;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !hasTalked)
        {
            if (!isDialogueActive)
            {
                StartDialogue();
            }
            else if (dialogueText.text == dialogueLines[currentLineIndex])
            {
                // If line is finished typing, go to next
                NextLine();
            }
            else
            {
                // If still typing, instantly show the full line
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                dialogueText.text = dialogueLines[currentLineIndex];
            }
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueUI.SetActive(true);
        currentLineIndex = 0;
        StartTypingLine();
    }

    void StartTypingLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void NextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueLines.Length)
        {
            StartTypingLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialogueUI.SetActive(false);
        hasTalked = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}

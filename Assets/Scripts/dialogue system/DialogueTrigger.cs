using UnityEngine;
using Febucci.UI;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager _dialogueManager;
    private bool dialogueStarted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StringUtils.Tags.Player) && !dialogueStarted)
        {
            dialogueStarted = true;
            _dialogueManager.StartDialogue();
        }
    }
}
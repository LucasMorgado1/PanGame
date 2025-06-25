using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Febucci.UI;
using DG.Tweening;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TypewriterByCharacter _typewriter;
    [SerializeField] private Image _dialogueTriggerImage;
    [SerializeField] private Image _dialogueInteractImage;
    private DialogueAnimations _dialogueAnimations;

    [Header("Timer variables")]
    [SerializeField] private float _secondsBetweenText = 2f;

    [Header("Scriptable Objects")]
    [SerializeField] private DialogueScriptable _dialogueScriptable;

    [Header("Interacted Variables")]
    [SerializeField] private bool _interactToStartDialogue;
    private bool _playerInteracted = false;

    [Header("Dialogues")]
    private Queue<string> _dialogueQueue = new Queue<string>();
    [TextArea(3, 10)][SerializeField] private List<string> _dialogues = new List<string>();
    #endregion

    #region Get/Set
    public bool GetIsShowingText { get => _typewriter.isShowingText; }
    #endregion

    #region Unity Methods

    private void Awake()
    {
        _dialogueAnimations = GetComponent<DialogueAnimations>();
    }

    private void Start()
    {
        foreach (string dialogue in _dialogues)
        {
            _dialogueQueue.Enqueue(dialogue);
        }

        if (_interactToStartDialogue)
            _dialogueAnimations.FadeInImage(_dialogueInteractImage);
        else
            _dialogueAnimations.FadeInImage(_dialogueTriggerImage);

        _dialogueScriptable._enteredOnce = false;
        _dialogueScriptable._isOnDialogue = false;
    }

    private void FixedUpdate()
    {
        if (!_typewriter.isShowingText && _dialogueScriptable._isOnDialogue)
        {
            _dialogueAnimations.FadeInImage(_dialogueAnimations.GetDialogueArrowImage);
            _dialogueAnimations.Wiggle(_dialogueAnimations.GetDialogueArrowImage, _dialogueAnimations.GetArrowInitialPosition);
        }

        if (!_dialogueScriptable._isOnDialogue && _interactToStartDialogue)
            _dialogueAnimations.Wiggle(_dialogueInteractImage, _dialogueAnimations.GetDialogueIndicatorInitialPosition);
        else if (!_dialogueScriptable._isOnDialogue && !_interactToStartDialogue)
            _dialogueAnimations.Wiggle(_dialogueTriggerImage, _dialogueAnimations.GetDialogueIndicatorInitialPosition);

    }
    #endregion

    #region Player Interacted Functions
    public void PlayerInteracted()
    {
        _playerInteracted = true;
        _dialogueAnimations.FadeOutImage(_dialogueInteractImage);
        _dialogueAnimations.FadeInImage(_dialogueAnimations.GetDialogueBoxImage);
        DisplayNextDialogue();
    }

    public void SkipText ()
    {
        if (_typewriter.isShowingText) { _typewriter.SkipTypewriter(); }
    }

    public void ShowNextText()
    {
        if (_dialogueQueue.Count > 1 && !_typewriter.isShowingText)
        {
            _dialogueAnimations.FadeOutImage(_dialogueAnimations.GetDialogueArrowImage);
            _dialogueQueue.Dequeue(); // Remove o primeiro da fila
            DisplayNextDialogue(); // Chama a atualização para exibir o próximo
            Invoke(nameof(ResetMousePressedOnce), 0.1f);
        }
        else
        {
            _typewriter.ShowText(""); // Limpa caso não haja mais diálogos
            _dialogueAnimations.FadeOutImage(_dialogueAnimations.GetDialogueBoxImage);
            _dialogueAnimations.FadeOutImage(_dialogueAnimations.GetDialogueArrowImage);
            _dialogueScriptable._isOnDialogue = false;
        }
    }

    public void ResetMousePressedOnce()
    {
        _dialogueScriptable._mousePressedOnce = false;
    }
    #endregion

    #region Dialogue System
    public void StartDialogue()
    {
        if (!_interactToStartDialogue)
        {
            _dialogueAnimations.FadeOutImage(_dialogueTriggerImage);
            _dialogueAnimations.FadeInImage(_dialogueAnimations.GetDialogueBoxImage);
            DisplayNextDialogue();
        }
    }

    private void DisplayNextDialogue()
    {
        Debug.Log("count: " + _dialogueQueue.Count);

        if (_dialogueQueue.Count > 0 && !_typewriter.isShowingText)
        {
            _typewriter.ShowText(_dialogueQueue.Peek()); // Mostra o primeiro da fila
        }
    }
    public void CallTimerCoroutine()
    {
        StartCoroutine(TimerBetweenText());
    }

    private IEnumerator TimerBetweenText ()
    {
        while (_dialogueQueue.Count > 0 && !_typewriter.isShowingText && !_playerInteracted)
        {
            _dialogueAnimations.FadeOutImage(_dialogueAnimations.GetDialogueArrowImage);
            yield return new WaitForSeconds(_secondsBetweenText);
            _dialogueQueue.Dequeue(); // Remove o primeiro da fila
            DisplayNextDialogue(); // Chama a atualização para exibir o próximo
        }

        if (_dialogueQueue.Count == 0 && !_typewriter.isShowingText && !_playerInteracted)
            Invoke(nameof(CleanLastText), _secondsBetweenText);

        Invoke(nameof(ResetMousePressedOnce), 0.1f); //reseta o _mousePressedOnce para false, permitindo o jogador pular o proximo texto
    }
    private void CleanLastText ()
    {
        _typewriter.ShowText(""); // Limpa caso não haja mais diálogos
        _dialogueScriptable._enteredOnce = false;
        _dialogueScriptable._isOnDialogue = false;
        gameObject.transform.GetChild(1).GetComponent<BoxCollider2D>().enabled = false;
        _dialogueAnimations.FadeOutImage(_dialogueAnimations.GetDialogueBoxImage);
    }
    #endregion
}

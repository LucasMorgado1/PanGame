using UnityEngine;
using UnityEngine.InputSystem;

public class playerInteract : MonoBehaviour, IInteractable
{
    [Header("References")]
    private Rigidbody2D _rb;

    [Header("Interactables")]
    private int _interactableLayerMask = 6;
    private bool _interacted = false;
    private bool _mousePressedOnce = false;
    private bool _enteredNPCHitbox = false;
    private bool _leftNPCHitbox = false;

    [Header("Dialogue")]
    [SerializeField] private DialogueScriptable _dialogueScriptable;
    [SerializeField] private DialogueManager _dialogueManager;
    private bool _isMouseButtonPressed = false;
    public float SetGravityScale { set => _rb.gravityScale = value; }

    public void InteractHandler(InputAction.CallbackContext ctx)
    {
        _interacted = ctx.performed;
    }

    public void MouseButtonPressedHandler(InputAction.CallbackContext ctx)
    {
        _isMouseButtonPressed = ctx.performed;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _dialogueScriptable._mousePressed = false;
        _dialogueScriptable._mousePressedOnce = false;
    }

    void FixedUpdate()
    {
        if (_isMouseButtonPressed && !_dialogueScriptable._mousePressedOnce && _dialogueScriptable._isOnDialogue)
        {
            if (_dialogueManager.GetIsShowingText)
            {
                _dialogueScriptable._mousePressedOnce = true;
                _dialogueManager.SkipText();
            }
            else
            {
                _dialogueScriptable._mousePressedOnce = true;
                _dialogueManager.ShowNextText();
            }
        }
    }
    
    public void Interact()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(StringUtils.Tags.NPC) && collision.gameObject.layer == _interactableLayerMask)
            _enteredNPCHitbox = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_interacted)
        {
            if (collision.gameObject.CompareTag(StringUtils.Tags.NPC)
                && collision.gameObject.layer == _interactableLayerMask
                && !_dialogueScriptable._enteredOnce)
            {
                _dialogueScriptable._enteredOnce = true;
                _dialogueScriptable._isOnDialogue = true;
                collision.gameObject.transform.GetChild(0).GetComponent<DialogueManager>().PlayerInteracted();
                Debug.Log("Interacted with a NPC");
            }

            if (collision.gameObject.CompareTag(StringUtils.Tags.Teleport)
                && collision.gameObject.layer == _interactableLayerMask)
            {
                Debug.Log("Initiating teleport");
                ChangeGravityScale(0);
                collision.gameObject.GetComponent<TeleportPlayer>().SetPlayerInteraction(this.gameObject);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(StringUtils.Tags.NPC) && collision.gameObject.layer == _interactableLayerMask)
            _leftNPCHitbox = true;
    }
    
    public void ChangeGravityScale(int x)
    {
        SetGravityScale = x;
    }
}

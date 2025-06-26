using UnityEngine;
using static player;

public class playerWalk : MonoBehaviour, IWalkable
{
    public enum MovementState { Idle, Walking, Stop };

    #region Variables

    [Header("References")]
    private MovementState mState;
    private PlayerControls _playerControls;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float velPower;
    [SerializeField] private float frictionAmout;
    [HideInInspector] public float direction = default;
    public float moveDirection = default;
    private bool isFacingRight = true;
    private float saveMoveSpeed = 7;
    private float saveFrictionAmount = 0.55f;

    [Header("Original Movement Values")]

    private float originalMoveSpeed = 7;
    private float originalAcceleration = 12;
    private float originalDecceleration = 16;
    private float originalVelPower = 0.85f;
    private float originalFrictionAmout = 0.55f;

    #endregion
    public MovementState SetIdle() => mState = MovementState.Idle;
    public MovementState SetWalk() => mState = MovementState.Walking;
    public MovementState SetStop() => mState = MovementState.Stop;
    public bool GetIdle => mState == MovementState.Idle;
    public bool GetWalk => mState == MovementState.Walking;
    public float SetMoveSpeed { set => moveSpeed = value; }
    public float OriginalMoveSpeed { get => saveMoveSpeed; private set => saveMoveSpeed = value; }
    public float FrictionAmount { get => frictionAmout; set => frictionAmout = value; }
    public float OriginalFrictionAmount { get => saveFrictionAmount; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
  
    private void Awake()
    {
        _playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        _playerControls.Player.Move.performed += ctx => moveDirection = ctx.ReadValue<float>();
    }

    void Start()
    {
        SetIdle();
    }

    private void Update()
    {
        if (!isFacingRight && moveDirection > 0f)
            FlipSprite();
        else if (isFacingRight && moveDirection < 0f)
            FlipSprite();
    }

    void FixedUpdate()
    {
        if (mState != MovementState.Stop)
        {
            if (moveDirection == 0)
                SetIdle();
            else
            {
                Walk();
                SetDirection();
            }
            Friction();
        }
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    public void Walk()
    {
        //calcula a direcao que queremos nos mover na velocidade desejada
        float targetSpeed = moveDirection * moveSpeed;

        //calcula a diferente entre a velocidade atual e a velocidade desejada
        float speedDif = targetSpeed - rb.linearVelocity.x;

        //muda a taxa de acelera��o dependnedo da situa��o
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01) ? acceleration : decceleration;

        //aplica a acelera��o na diferen�a de velocidade, a acelera��o aumenta com velocidades maiores
        //multiplica com o Sign para reaplicar a dire��o
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        //aplica a for�a ao rb, multiplicando pelo Vector2.right, afetando apenas em X
        rb.AddForce(movement * Vector2.right);

        SetWalk();
    }

    public void SetDirection()
    {
        if (moveDirection > 0)
        {
            direction = 1;
        }
        else if (moveDirection < 0)
        {
            direction = -1;
        }
    }

    private void FlipSprite()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void DisablePlayerMovement()
    {
        SetMoveSpeed = 0;
        FrictionAmount = 5;
        //jumpForce = 0;
        //_canDash = false;
    }

    public void EnablePlayerMovement()
    {
        SetMoveSpeed = originalMoveSpeed;
        frictionAmout = originalFrictionAmout;
        //jumpForce = originalJumpForce;
        //_canDash = true;
    }

    private void Friction()
    {
        if (Mathf.Abs(moveDirection) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.linearVelocity.x), Mathf.Abs(frictionAmout));

            amount *= Mathf.Sign(rb.linearVelocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }
}

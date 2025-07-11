using UnityEngine;

public class playerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce;  // Força inicial do pulo
    [SerializeField] private int maxJumps = 2;  // Número máximo de pulos
    [SerializeField] private float originalJumpForce = 5f;
    [SerializeField] private GroundChecker groundCheck;
    [SerializeField] private LayerMask groundLayer;
    public float fallMultiplier = 2.5f;    // Multiplicador de queda para controlar a velocidade de queda
    public float lowJumpMultiplier = 2f;   // Multiplicador de pulo baixo para controlar o arco do pulo
    private float playerYpositionBeforeJump;
    private int currentJumps = 0;     // Pulos atuais
    private bool isJumping = false;   // Está pulando?  2
    private Rigidbody2D rb;
    private PlayerControls _playerControls;

    public bool GetIsJumping { get => isJumping; }

    void Start()
    {
        jumpForce = originalJumpForce;
        rb = GetComponent<Rigidbody2D>();
        _playerControls = new PlayerControls();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && currentJumps < maxJumps)
        {
            playerYpositionBeforeJump = this.transform.position.y;

            if (!isJumping)
            {
                Jump();
                isJumping = true;
            }
            else if (isJumping && currentJumps <= maxJumps - 1)
            {
                Jump();
            }
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Aplica o multiplicador de pulo baixo para controlar o arco do pulo
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        currentJumps++;
    }

    public void DisablePlayerJump()
    {
        jumpForce = 0;
    }

    public void RestorePlayerJump()
    {
        jumpForce = originalJumpForce;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(StringUtils.Tags.Ground) && groundCheck.IsGrounded())
        {
            currentJumps = 0;
            isJumping = false;
        }
    }
}

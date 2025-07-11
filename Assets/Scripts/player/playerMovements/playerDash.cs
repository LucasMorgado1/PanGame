using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerDash : MonoBehaviour, IDash
{
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashForce;
    private float dashTime = 0.2f;
    private bool _pressedDash = default;
    private bool _canDash = true;
    private Rigidbody2D _rb;
    private float _moveDirection = 0;
    private float _direction = 0f;

    public bool SetCanDash { get => _canDash; private set => _canDash = value; }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _moveDirection = GetComponent<playerWalk>().GetMoveDirection;
        _direction = GetComponent<playerWalk>().GetDirection;
    }

    private void FixedUpdate()
    {
        if (_pressedDash)
        {
            Debug.Log("deu dash");
            if (_canDash)
            {
                _canDash = false;
                //PerformDash();
                StartCoroutine(((IDash)this).Dash());
            }
        }
    }

    public void DashHandler(InputAction.CallbackContext ctx)
    {
        _pressedDash = ctx.performed;
    }

    IEnumerator IDash.Dash()
    {
        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0;

        float dashSpeed = (dashForce + GetComponent<playerWalk>().GetMoveDirection) * GetComponent<playerWalk>().GetDirection;
        Vector2 dashDirection = new Vector2(this.transform.position.x + dashSpeed, this.transform.position.y);

        _rb.AddForce(dashDirection);

        yield return new WaitForSeconds(dashTime);

        _rb.gravityScale = originalGravity;

        yield return new WaitForSeconds(dashCooldown);

        _canDash = true;
    }

    public bool DisablePlayerDash()
    {
        return SetCanDash = false;
    }

    public bool EnablePlayerDash()
    {
        return SetCanDash = true;
    }
}

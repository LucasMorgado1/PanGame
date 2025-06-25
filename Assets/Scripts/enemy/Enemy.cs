using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum Side { left, right, idle, chasing, returning}
    enum Action { attacking, walking, idle, chasing, combat, returning}
    enum Name { Slime, Gnome, Cyclope, Golem, None }

    [Header("Enemy Variables")]
    [SerializeField] private turnbasedScript _turnbasedScript;
    [SerializeField] private EnemyScriptableObject _enemyScriptableObject;
    private Rigidbody2D _rb;
    private Animator _animator;
    private BoxCollider2D _boxCollider;

    [Header("Combat Variables")]
    private int _hp;
    private int _maxHP;
    private int _normalDamage;
    private int _strongDamage;
    private SpriteRenderer _spriteRenderer;
    private int _healAmount;
    private EnemyType _enemyType;
    private int _enemyIndex;

    [Header("State")]
    private Side _side = Side.right;
    private Action _action = Action.walking;
    [SerializeField] private Name _name;

    [Header("Enemies Variables")]
    private float _velocity;
    private float _originalVelocity;
    private float _distance = 3f;
    private Vector2 _leftPoint;
    private Vector2 _rightPoint;
    private Vector2 _startPosition;
    private bool _isFacingRight;
    private Side _lastWalkingSide;
    [SerializeField] private float _distanceOffSet;
    [SerializeField] private float _maxChasingDistance;
    private Vector2 _moveDirection; //move into target direction when chasing

    [Header("Target")]
    [SerializeField] private GameObject _target;

    #region Getter/Setter
    public int GetLife { get => _hp; }
    public int SetLife { set => _hp = value; }
    public int GetTotalLife { get => _maxHP; }
    public int GetNormalDamage { get => _normalDamage; }
    public int GetStrongDamage { get => _strongDamage; }
    public int GetIndex { get => _enemyIndex; }
    public int GetHealAmount => _healAmount;
    public Sprite GetSprite { get => _spriteRenderer.sprite; }
    public EnemyType GetEnemyType { get => _enemyType; }
    #endregion

    private void Awake()
    {
        _enemyIndex = GetComponent<Index>().GetIndex;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();  
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();

        _maxHP = _enemyScriptableObject._hp;
        _hp = _maxHP;

        _normalDamage = _enemyScriptableObject._normalAttack;
        _strongDamage = _enemyScriptableObject._strongAttack;
        _enemyType = _enemyScriptableObject._enemyType;
        _healAmount = _enemyScriptableObject._healAmount;
        _spriteRenderer.sprite = _enemyScriptableObject._sprite;
        _healAmount = _enemyScriptableObject._healAmount;
        _velocity = _enemyScriptableObject._velocity;
        _distance = _enemyScriptableObject._distance;
    }

    private void Start()
    {
        _startPosition = this.transform.position;
        _originalVelocity = _velocity;
        _leftPoint = new Vector2(_startPosition.x - _distance, _startPosition.y);
        _rightPoint = new Vector2(_startPosition.x + _distance, _startPosition.y);
        _side = Side.right;
        _action = Action.walking;

        Debug.Log("Name: " + _name + ", distance: " + _distance + ", velocity: " + _velocity);
    }

    private void Update()
    {

        if (!_isFacingRight && _side == Side.left)
            FlipSprite();
        else if (_isFacingRight && _side == Side.right)
            FlipSprite();
    }

    private void FixedUpdate()
    {
        switch (_action)
        {
            case Action.attacking:
                break;
            case Action.walking:
                if (_side == Side.right || _side == Side.left)
                    Walking();
                break;
            case Action.idle:
                break;
            case Action.chasing:
                if (_side == Side.chasing)
                    ChasePlayer();
                break;
            case Action.combat:
                if (_side == Side.idle)
                    StopEnemyMovement();
                break;
            case Action.returning:
                if (_side == Side.returning)
                    ReturnToStartPosition();
                break;
            default:
                break;
        }

        CalculateDistanceBetweenPlayer();
    }

    #region Walk Behavior

    private void Walk(float velocity)
    {
        _rb.linearVelocity = new Vector3(velocity, 0, 0);
    }
    
    private void Walking ()
    {
        _velocity = _originalVelocity;

        if (_side == Side.right && _action == Action.walking)
        {
            if (_rightPoint.x > transform.position.x)
            {
                _isFacingRight = true;
                _velocity = Mathf.Abs(_velocity);
                Walk(_velocity);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

                if (Vector2.Distance(_rightPoint, transform.position) < 0.9f)
                {
                    StartCoroutine(Idle());
                    return;
                }
            }
        }

        if (_side == Side.left && _action == Action.walking)
        {
            if (_leftPoint.x < transform.position.x)
            {
                _lastWalkingSide = Side.right;
                _isFacingRight = false;
                _velocity = Mathf.Abs(_velocity) * -1;
                Walk(_velocity);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);

                if (Vector2.Distance(_leftPoint, transform.position) < 0.9f)
                {
                    StartCoroutine(Idle());
                    return;
                }
            }
        }
    }

    IEnumerator  Idle ()
    {
        _side = Side.idle;
        _action = Action.idle;

        if (_side == Side.idle && _action == Action.idle)
        {
            _velocity = 0f;
            Walk(_velocity);
        }

        yield return new WaitForSeconds(2f);

        _side = (_isFacingRight == true) ? Side.left : Side.right;
        _action = Action.walking;
        Walking();
    }

    private void FlipSprite()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    #endregion

    #region Chase the Player

    private void CalculateDistanceBetweenPlayer()
    {
        if (Vector3.Distance(_target.transform.position, this.transform.position) < this._boxCollider.size.x + _distanceOffSet && _side != Side.returning && _action != Action.returning)
        {
            Vector3 direction = (_target.transform.position - this.transform.position).normalized;
            _moveDirection = direction;
            _side = Side.chasing;
            _action = Action.chasing;
        }
    }

    private void ChasePlayer()
    {
        if (_side == Side.chasing && _action == Action.chasing) 
        {
            _velocity = Mathf.Abs(_velocity);
            _rb.linearVelocity = new Vector3(_moveDirection.x, 0, 0) * _velocity;

            if (Vector3.Distance(this.transform.position, _startPosition) > _maxChasingDistance)
            {
                Debug.Log("I've gone too far, so Im coming back to my original position");
                _side = Side.returning;
                _action = Action.returning;
            }
        }
    }

    private void ReturnToStartPosition()
    {
        if (_side == Side.returning && _action == Action.returning)
        {
            if (this.transform.position.x < _startPosition.x)
            {
                _velocity = Mathf.Abs(_velocity);
                Walk(_velocity);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

                if (Vector2.Distance(_startPosition, this.transform.position) <= 1f)
                {
                    UpdateWalkingPoints();
                    _side = Side.right;
                    _action = Action.walking;
                }
            }

            if (this.transform.position.x > _startPosition.x)
            {
                _velocity = Mathf.Abs(_velocity) * -1;
                Walk(_velocity);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);

                if (Vector2.Distance(_startPosition, this.transform.position) <= 1f)
                {
                    UpdateWalkingPoints();
                    _side = Side.right;
                    _action = Action.walking;
                }
            }
        }
    }

    private void UpdateWalkingPoints()
    {
        _leftPoint = new Vector2(_startPosition.x - _distance, _startPosition.y);
        _rightPoint = new Vector2(_startPosition.x + _distance, _startPosition.y);
    }

    #endregion

    private void StopEnemyMovement()
    {
        _velocity = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag(StringUtils.Tags.Player))
        {
            _side = Side.idle;
            _action = Action.combat;
        }
    }

    #region Enemy Take Damage
    public bool TakeDamage(int damage)
    { 
        _hp -= damage;

        if (_hp <= 0)
            return true;
        else
            return false;
    }

    public void HealEnemy (int healAmount)
    {
        _hp += healAmount;
    }

    public void RestoreFullHP ()
    {
        _hp = _maxHP;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_leftPoint, _rightPoint);
    }
    #endregion
}

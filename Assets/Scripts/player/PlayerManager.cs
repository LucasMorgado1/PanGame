using UnityEngine;

public class PlayerManager : MonoBehaviour, IKillable
{
    #region References
    private playerWalk _playerMovement;
    #endregion

    #region Variables
    [Header("Player Max Health")]
    [SerializeField] private int _maxHealth = 10;

    [Header("Player Attack Damage")]
    [SerializeField] private int _attackDamage = 1;
    private int _currentHealth;
    #endregion

    #region Getter/Setter
    public int GetCurrentHealth { get => _currentHealth; }
    public int GetMaxHealth { get => _maxHealth; }
    public int GetAttackDamage { get => _attackDamage; }
    public playerWalk GetPlayerWalkScript { get => _playerMovement; }
    #endregion

    #region Functions
    void Awake()
    {
        _playerMovement = GetComponent<playerWalk>();
    }

    void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void HealHealth(int health)
    {
        _currentHealth += health;

        if (_currentHealth >= _maxHealth)
            _currentHealth = _maxHealth;
    }

    public bool TakeDamage(int damage)
    {
        _currentHealth -= damage;
        
        if (_currentHealth <= 0)
            return true;
        else
            return false;

    }

    public void RestoreMaxHealth()
    {
        _currentHealth = _maxHealth;
    }
    #endregion
}

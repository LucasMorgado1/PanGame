using System.Collections;
using Febucci.UI;
using UnityEngine;

public class EnemyTurn : MonoBehaviour
{

    [Header("References")]
    private Enemy _enemy;
    private turnbasedScript _turnbasedManager;
    private TypewriterByWord _textAnimator;
    private PlayerTurn _playerTurn;
    private PlayerManager _playerManager;

    [Header("Booleans")]
    private bool _enemyHasHealed = false;

    [Header("Getter/Setter")]
    public Enemy SetEnemy { set => _enemy = value; }
    public Enemy GetEnemyFunctions { get => _enemy; }

    void Awake()
    {
        _turnbasedManager = GetComponent<turnbasedScript>();
    }

    void Start()
    {
        _textAnimator = _turnbasedManager.GetTextAnimator;
        _playerTurn = GetComponent<PlayerTurn>();
        _playerManager = _playerTurn.GetPlayerManager;
    }

    #region Enemy Turn
    public IEnumerator EnemyAttack()
    {
        bool isDead = false; ;
        _turnbasedManager.GetDialogueText.text = _enemy.name + " attacks...";

        if (_enemy.GetLife <= Mathf.Abs(_enemy.GetTotalLife / 3) && !_enemyHasHealed)
        {
            if (_turnbasedManager.RollDice() > 5)
            {
                _textAnimator.ShowText(_enemy.name + " healed, restauring " + _enemy.GetHealAmount + " HP!!");
                _enemy.HealEnemy(_enemy.GetHealAmount);
                _enemyHasHealed = true;
                _turnbasedManager.UpdateHPUI(_turnbasedManager.GetEnemyText, _enemy.GetLife, _enemy.GetTotalLife);
            }
            else
            {
                _textAnimator.ShowText(_enemy.name + " tried to Heal, but has <bounce>failed...</bounce>");
            }

        }
        else
        {
            CallRandomAttack(_enemy.GetEnemyType, isDead);
        }

        yield return new WaitForSeconds(4f);

        if (isDead)
        {
            _turnbasedManager.State = BattleState.Lost;
            StartCoroutine(_turnbasedManager.EndBattle());
        }
        else
        {
            _turnbasedManager.State = BattleState.PlayerTurn;
            _playerTurn.PlayerPhase();
            //StartCoroutine(_playerTurn.PlayerPhase());
        }
    }

    private void CallRandomAttack(EnemyType enemyType, bool isDead)
    {
        switch (enemyType)
        {
            case EnemyType.Weak:
                RandomEnemyAttack(_enemy.GetNormalDamage, _enemy.GetStrongDamage, enemyType, 1, 9, 0, 0, isDead);
                break;
            case EnemyType.Medium:
                RandomEnemyAttack(_enemy.GetNormalDamage, _enemy.GetStrongDamage, enemyType, 1, 8, 9, 9, isDead);
                break;
            case EnemyType.Strong:
                RandomEnemyAttack(_enemy.GetNormalDamage, _enemy.GetStrongDamage, enemyType, 1, 6, 7, 8, isDead);
                break;
            case EnemyType.Hard:
                RandomEnemyAttack(_enemy.GetNormalDamage, _enemy.GetStrongDamage, enemyType, 1, 5, 6, 8, isDead);
                break;
            default:
                break;
        }
    }

    private void RandomEnemyAttack(int normalAttack, int strongAttack, EnemyType enemyType, int minNormal, int maxNormal, int minStrong, int maxStrong, bool isDead)
    {
        int roll = _turnbasedManager.RollDice();
        int damage = 0;
        string message;

        if (roll >= minNormal && roll <= maxNormal)
        {
            damage = normalAttack;
            message = $"{_enemy.name} attacks causing {damage} damage.";
        }
        else if (roll >= minStrong && roll <= maxStrong)
        {
            damage = strongAttack;
            message = $"{_enemy.name.ToUpper()} attacks causing {damage} damage.";
        }
        else
        {
            isDead = false;
            _turnbasedManager.UpdateHPUI(_turnbasedManager.GetPlayerText, _playerManager.GetCurrentHealth, _playerManager.GetMaxHealth);
            _turnbasedManager.GetDialogueText.text = $"{_enemy.name.ToUpper()} has missed his attack!!";
            return;
        }

        isDead = _playerManager.TakeDamage(damage);
        _turnbasedManager.UpdateHPUI(_turnbasedManager.GetPlayerText, _playerManager.GetCurrentHealth, _playerManager.GetMaxHealth);
        _turnbasedManager.GetDialogueText.text = message;
    }
    #endregion
}

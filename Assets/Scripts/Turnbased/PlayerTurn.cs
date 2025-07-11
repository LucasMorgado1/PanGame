using System.Collections;
using Febucci.UI;
using UnityEngine;

public class PlayerTurn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerManager _playerManager;
    private turnbasedScript _turnbasedManager;
    private TypewriterByWord _textAnimator;
    private EnemyTurn _enemyTurn;

    public PlayerManager GetPlayerManager { get => _playerManager; }

    void Awake()
    {
        _turnbasedManager = GetComponent<turnbasedScript>();
        _enemyTurn = GetComponent<EnemyTurn>();
    }

    void Start()
    {
        _textAnimator = _turnbasedManager.GetTextAnimator;
    }

    //#region Player Turn
    public void PlayerPhase()
    {
        _textAnimator.ShowText("Choose an action...");
        _turnbasedManager.GetAttackCanvas.SetActive(true);
    }

    public void OnAttackButton ()
    {
        if (_turnbasedManager.State != BattleState.PlayerTurn)
            return;

        _turnbasedManager.GetAttackCanvas.SetActive(false);
        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton ()
    {
        if (_turnbasedManager.State != BattleState.PlayerTurn)
            return;

        _turnbasedManager.GetAttackCanvas.SetActive(false);
        StartCoroutine(HealPlayer());
    }

    public void OnRunButton ()
    {
        if (_turnbasedManager.State != BattleState.PlayerTurn)
            return;

        StartCoroutine(_turnbasedManager.Run());
    }

    public IEnumerator PlayerAttack ()
    {
        bool isDead;

        if (_turnbasedManager.RollDice() <= 9)
        {
            //damage the enemy
            isDead = _enemyTurn.GetEnemyFunctions.TakeDamage(_playerManager.GetAttackDamage);
            _turnbasedManager.UpdateHPUI(_turnbasedManager.GetEnemyText, _enemyTurn.GetEnemyFunctions.GetLife, _enemyTurn.GetEnemyFunctions.GetTotalLife);
            //_dialogueText.text = "The attack is succesful!!";
            _textAnimator.ShowText("The attack is <color=yellow><wave>succesful</color></wave>!!");
        }
        else
        {
            //_dialogueText.text = "The attack has missed the enemy!!";
            _textAnimator.ShowText("The attack has <color=red>missed</color> the enemy!!");
            isDead = false;
        }

        yield return new WaitForSeconds(4f);

        //check if the enemy is dead
        if (isDead)
        {
            _turnbasedManager.State = BattleState.Won;
            StartCoroutine(_turnbasedManager.EndBattle());
            yield return null;
        }
        else
        {
            _turnbasedManager.State = BattleState.EnemyTurn;
            StartCoroutine(_enemyTurn.EnemyAttack());
            yield return null;
        }
    }

    IEnumerator HealPlayer ()
    {
        _playerManager.HealHealth(3);
        _turnbasedManager.UpdateHPUI(_turnbasedManager.GetPlayerText, _playerManager.GetCurrentHealth, _playerManager.GetMaxHealth);
        //_dialogueText.text = "After driking some refreshing juice, you feel new again!!";
        _textAnimator.ShowText("You feel <wave><color=green>healed</color></wave> after drinking some juice");

        yield return new WaitForSeconds(4f);

        _turnbasedManager.State = BattleState.EnemyTurn;
        _turnbasedManager.GetAttackCanvas.SetActive(false);
        StartCoroutine(_enemyTurn.EnemyAttack());
    }
}

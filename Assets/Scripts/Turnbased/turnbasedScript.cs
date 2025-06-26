using System.Collections;
using UnityEngine;
using TMPro;
using Febucci.UI;
using UnityEngine.UI;
using Unity.VisualScripting;

public enum BattleState { Start, PreparingPlayer, PlayerTurn, EnemyTurn, Won, Lost}

public class turnbasedScript : MonoBehaviour
{
    [Header("References")]
    private Camera _camera;
    [SerializeField] private EnemiesManager _enemiesManager;
    [SerializeField] private Checkpoint _checkpoint;

    [Header("BattlePosition")]
    [SerializeField] private Image _playerBattlePosition;
    [SerializeField] private Image _enemyBattlePosition;

    [Header("Player")]
    [SerializeField] private player _player;

    [Header("Canvas")]
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _tbCanvas;
    private GameObject _attackCanvas;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _playerLife;
    [SerializeField] private TextMeshProUGUI _enemyHPtxt;
    private GameObject _lifeUI;

    [Header("Enemy")]
    private Enemy _enemy;
    private bool _enemyHasHealed = false;

    private BattleState _state = BattleState.PlayerTurn;

    private TypewriterByWord _textAnimator;

    #region Getter & Setters
    public Enemy SetEnemyScript { set => _enemy = value; }
    //public GameObject SetEnemySelectedByThePlayer { set => _enemySelectedByThePlayer = value; }
    #endregion

    private void Start()
    {
        _state = BattleState.Start;
        _camera = Camera.main;
       
        _attackCanvas = this.gameObject.transform.GetChild(2).transform.GetChild(0).gameObject;
        _lifeUI = this.gameObject.transform.GetChild(2).transform.GetChild(1).gameObject;

        _textAnimator = this.gameObject.transform.GetChild(1).GetComponent<TypewriterByWord>();

        _lifeUI.SetActive(false);
        _attackCanvas.SetActive(false);
    }

    #region Setup Battle
    IEnumerator SetupBattle ()
    {
        _enemy = GetComponent<Enemy>();

        _textAnimator.ShowText("The Battle starts");

        yield return new WaitForSeconds(3f);

        _state = BattleState.PlayerTurn;

        _lifeUI.SetActive(true);

        UpdateHPUI(_playerLife, _player.GetLife, _player.GetTotalLife);
        UpdateHPUI(_enemyHPtxt, _enemy.GetLife, _enemy.GetTotalLife);

        _playerLife.transform.parent.gameObject.GetComponent<TextMeshProUGUI>().text = _player.name.ToUpper(); //change the text according to the player's name
        _enemyHPtxt.transform.parent.gameObject.GetComponent<TextMeshProUGUI>().text = _enemy.name.ToUpper(); //change the text according to the enemy name

        _playerBattlePosition.sprite = _player.GetComponent<SpriteRenderer>().sprite;
        _enemyBattlePosition.sprite = _enemy.GetSprite;

        PlayerTurn();

    }

    public void ActivateTurnBased()
    {
        _tbCanvas.SetActive(true);
        StartCoroutine(SetupBattle());
        transform.position = _player.gameObject.transform.position;
        _camera.orthographicSize = 3;
    }
    #endregion

    #region Player Turn
    private void PlayerTurn ()
    {
        _textAnimator.ShowText("Choose an action...");
        _attackCanvas.SetActive(true);
    }

    public void OnAttackButton ()
    {
        if (_state != BattleState.PlayerTurn)
            return;

        _attackCanvas.SetActive(false);
        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton ()
    {
        if (_state != BattleState.PlayerTurn)
            return;

        _attackCanvas.SetActive(false);
        StartCoroutine(HealPlayer());
    }

    public void OnRunButton ()
    {
        if (_state != BattleState.PlayerTurn)
            return;

        StartCoroutine(Run());
    }

    IEnumerator PlayerAttack ()
    {
        bool isDead;

        if (RollDice() <= 9)
        {
            //damage the enemy
            isDead = _enemy.TakeDamage(_player.GetAttackDamge);
            UpdateHPUI(_enemyHPtxt, _enemy.GetLife, _enemy.GetTotalLife);
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
            _state = BattleState.Won;
            StartCoroutine(EndBattle());
        } 
        else
        {
            _state = BattleState.EnemyTurn;
            StartCoroutine(EnemyAttack());
        }
    }

    IEnumerator HealPlayer ()
    {
        _player.HealHP(3);
        UpdateHPUI(_playerLife, _player.GetLife, _player.GetTotalLife);
        //_dialogueText.text = "After driking some refreshing juice, you feel new again!!";
        _textAnimator.ShowText("You feel <wave><color=green>healed</color></wave> after drinking some juice");

        yield return new WaitForSeconds(4f);

        _state = BattleState.EnemyTurn;
        _attackCanvas.SetActive(false);
        StartCoroutine(EnemyAttack());
    }
    #endregion

    #region Enemy Turn
    IEnumerator EnemyAttack ()
    {
        bool isDead = false; ;
        _dialogueText.text = _enemy.name + " attacks...";

        if (_enemy.GetLife <= Mathf.Abs(_enemy.GetTotalLife / 3) && !_enemyHasHealed)
        {
            if (RollDice() > 5)
            {
                _textAnimator.ShowText(_enemy.name + " healed, restauring " + _enemy.GetHealAmount + " HP!!");
                _enemy.HealEnemy(_enemy.GetHealAmount);
                _enemyHasHealed = true;
                UpdateHPUI(_enemyHPtxt, _enemy.GetLife, _enemy.GetTotalLife);
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
            _state = BattleState.Lost;
            StartCoroutine(EndBattle());
        } 
        else
        {
            _state = BattleState.PlayerTurn;
            PlayerTurn();
            //StartCoroutine(PlayerAttack());
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

    private void RandomEnemyAttack (int normalAttack, int strongAttack, EnemyType enemyType, int minNormal, int maxNormal, int minStrong, int maxStrong, bool isDead)
    {
        if (RollDice() >= minNormal && RollDice() <= maxNormal)
        {
            isDead = _player.TakeDamage(normalAttack);
            UpdateHPUI(_playerLife, _player.GetLife, _player.GetTotalLife);
            _dialogueText.text = _enemy.name + " attacks causing " + normalAttack + " of damage";
        }
        else if (RollDice() >= minStrong && RollDice() <= maxStrong)
        {
            isDead = _player.TakeDamage(strongAttack);
            UpdateHPUI(_playerLife, _player.GetLife, _player.GetTotalLife);
            _dialogueText.text = _enemy.name.ToUpper() + " attacks causing " + strongAttack + " of damage";
        }
        else
        {
            isDead = false;
            UpdateHPUI(_playerLife, _player.GetLife, _player.GetTotalLife);
            _dialogueText.text = _enemy.name.ToUpper() + " has missed his attack!!";
        }
    }

    #endregion

    #region End Battle
    IEnumerator EndBattle ()
    {
        if (_state == BattleState.Won)
        {
            _dialogueText.text = "CONGRATS!! You has everything that a Hero needs...";
            _lifeUI.SetActive(false);
            _attackCanvas.SetActive(false);

            yield return new WaitForSeconds(2.5f);

            _tbCanvas.SetActive(false);
            _camera.orthographicSize = 5;

            _enemyHasHealed = false;
            _enemiesManager.DisableEnemy(_enemy.GetIndex);
            _player.RestoreHP();
            _player.EnablePlayerMovement();

        } 
        else
        {
            _dialogueText.text = "DEFEATED!! Best luck in your next time.";
            _lifeUI.SetActive(false);

            yield return new WaitForSeconds(2.5f);

            _player.EnablePlayerMovement();
            _checkpoint.ReturnToCheckpoint();
            _enemiesManager.EnableAllEnemies(); //later Ill have to change this to enable all the enemies that wasnt defeated

            _enemyHasHealed = false;
            _player.RestoreHP();
            _enemy.RestoreFullHP();

            _tbCanvas.SetActive(false); //change this later to leantween animation
            _camera.orthographicSize = 5;
        }
        yield return null;
    }
    #endregion

    IEnumerator Run ()
    {
        _dialogueText.text = "You decided to RUN...";

        yield return new WaitForSeconds(2f);
        
        if (RollDice() > 7)
        {
            _dialogueText.text = "But you failed...";

            yield return new WaitForSeconds(3f);

            _attackCanvas.SetActive(false);
            _state = BattleState.EnemyTurn;
            StartCoroutine(EnemyAttack());
        }
        else
        {
            _dialogueText.text = "After fearing your enemy, you RAN!!";

            yield return new WaitForSeconds(3f);
            
            _tbCanvas.SetActive(false);
            _camera.orthographicSize = 5;
            //the player does not heal himself when running from a battle
            _enemiesManager.DisableEnemy(_enemy.GetIndex);
            _player.EnablePlayerMovement();
        }
        yield return null;
    }

    private void UpdateHPUI (TextMeshProUGUI lifeTxt, int currentLife, int totalLife)
    {
        lifeTxt.text = currentLife + "/" + totalLife;
    }

    private int RollDice ()
    {
        return Random.Range(1, 11);
    }
}

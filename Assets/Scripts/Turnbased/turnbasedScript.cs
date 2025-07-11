using System.Collections;
using UnityEngine;
using TMPro;
using Febucci.UI;
using UnityEngine.UI;
using Unity.VisualScripting;

public enum BattleState { Start, PreparingPlayer, PlayerTurn, EnemyTurn, Won, Lost}

public class turnbasedScript : MonoBehaviour
{
    #region Variables
    [Header("References")]
    private Camera _camera;
    [SerializeField] private EnemiesManager _enemiesManager;
    [SerializeField] private Checkpoint _checkpoint;
    private PlayerTurn _playerTurn;
    private EnemyTurn _enemyTurn;

    [Header("BattlePosition")]
    [SerializeField] private Image _playerBattlePosition;
    [SerializeField] private Image _enemyBattlePosition;

    [Header("Player")]
    [SerializeField] private PlayerManager _playerManager;
    private playerWalk _playerMovement;

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

    private bool isTurnBasedActive;
    [SerializeField] private TextMeshProUGUI debugtext;
    #endregion

    #region Getter & Setters
    public Enemy SetEnemyScript { set => _enemyTurn.SetEnemy = value; }
    public TypewriterByWord GetTextAnimator { get => _textAnimator; private set => _textAnimator = value; }
    public GameObject GetAttackCanvas { get => _attackCanvas; }
    public BattleState State { get => _state; set => _state = value; }
    public PlayerManager GetPlayerManager { get => _playerManager; }
    public TextMeshProUGUI GetPlayerText { get => _playerLife; }
    public TextMeshProUGUI GetEnemyText { get => _enemyHPtxt; }
    public TextMeshProUGUI GetDialogueText { get => _dialogueText; set => _dialogueText = value; }
    #endregion

    void Awake()
    {
        _playerTurn = GetComponent<PlayerTurn>();
        _enemyTurn = GetComponent<EnemyTurn>();
        _attackCanvas = this.gameObject.transform.GetChild(2).transform.GetChild(0).gameObject;
        _lifeUI = this.gameObject.transform.GetChild(2).transform.GetChild(1).gameObject;
        _textAnimator = this.gameObject.transform.GetChild(1).GetComponent<TypewriterByWord>();
    }

    private void Start()
    {
        _state = BattleState.Start;
        _camera = Camera.main;
        _playerMovement = _playerManager.GetPlayerWalkScript;
        _lifeUI.SetActive(false);
        _attackCanvas.SetActive(false);
    }

    private void Update() {
        if (isTurnBasedActive)
            debugtext.text = "Current State: " + _state.ToString();
    }

    #region Setup Battle
    IEnumerator SetupBattle ()
    {
        isTurnBasedActive = true;
        //_enemy = GetComponent<Enemy>();

        _textAnimator.ShowText("The Battle starts");

        yield return new WaitForSeconds(3f);

        _state = BattleState.PlayerTurn;

        _lifeUI.SetActive(true);

        UpdateHPUI(_playerLife, _playerTurn.GetPlayerManager.GetCurrentHealth, _playerTurn.GetPlayerManager.GetMaxHealth);
        UpdateHPUI(_enemyHPtxt, _enemyTurn.GetEnemyFunctions.GetLife, _enemyTurn.GetEnemyFunctions.GetTotalLife);

        _playerLife.transform.parent.gameObject.GetComponent<TextMeshProUGUI>().text = _playerTurn.GetPlayerManager.name.ToUpper(); //change the text according to the player's name
        _enemyHPtxt.transform.parent.gameObject.GetComponent<TextMeshProUGUI>().text = _enemyTurn.GetEnemyFunctions.name.ToUpper(); //change the text according to the enemy name

        _playerBattlePosition.sprite = _playerManager.GetComponent<SpriteRenderer>().sprite;
        _enemyBattlePosition.sprite = _enemyTurn.GetEnemyFunctions.GetSprite;

        _playerTurn.PlayerPhase();

    }

    public void ActivateTurnBased()
    {
        _tbCanvas.SetActive(true);
        StartCoroutine(SetupBattle());
        transform.position = _playerManager.gameObject.transform.position;
        _camera.orthographicSize = 3;
    }
    #endregion
    
    /*#region Player Turn
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
            isDead = _enemy.TakeDamage(_playerManager.GetAttackDamage);
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
        _playerManager.HealHealth(3);
        UpdateHPUI(_playerLife, _playerManager.GetCurrentHealth, _playerManager.GetMaxHealth);
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
            _playerTurn.PlayerPhase();
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

    private void RandomEnemyAttack(int normalAttack, int strongAttack, EnemyType enemyType, int minNormal, int maxNormal, int minStrong, int maxStrong, bool isDead)
    {
        int roll = RollDice();
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
            UpdateHPUI(_playerLife, _playerManager.GetCurrentHealth, _playerManager.GetMaxHealth);
            _dialogueText.text = $"{_enemy.name.ToUpper()} has missed his attack!!";
            return;
        }

        isDead = _playerManager.TakeDamage(damage);
        UpdateHPUI(_playerLife, _playerManager.GetCurrentHealth, _playerManager.GetMaxHealth);
        _dialogueText.text = message;
    }

    #endregion*/

    #region End Battle
    public IEnumerator EndBattle ()
    {
        if (_state == BattleState.Won)
        {
            _dialogueText.text = $"CONGRATS!! You has everything that a Hero needs...";
            _lifeUI.SetActive(false);
            _attackCanvas.SetActive(false);

            yield return new WaitForSeconds(2.5f);
        } 
        else
        {
            _dialogueText.text = "DEFEATED!! Best luck in your next time.";
            _lifeUI.SetActive(false);

            yield return new WaitForSeconds(2.5f);

            _checkpoint.ReturnToCheckpoint();
            _enemiesManager.EnableAllEnemies(); //later Ill have to change this to enable all the enemies that wasnt defeated
        }

        _playerMovement.EnablePlayerMovement();
        _enemyHasHealed = false;
        _playerManager.RestoreMaxHealth();
        _enemy.RestoreFullHP();
        _tbCanvas.SetActive(false); //change this later to leantween animation
        _camera.orthographicSize = 5;
        yield return null;
    }
    #endregion

    public IEnumerator Run ()
    {
        _dialogueText.text = "You decided to RUN...";

        yield return new WaitForSeconds(2f);
        
        if (RollDice() > 7)
        {
            _dialogueText.text = "But you failed...";

            yield return new WaitForSeconds(3f);

            _attackCanvas.SetActive(false);
            _state = BattleState.EnemyTurn;
            StartCoroutine(_enemyTurn.EnemyAttack());
        }
        else
        {
            _dialogueText.text = "The enemy was too strong... for now!";

            yield return new WaitForSeconds(3f);
            
            _tbCanvas.SetActive(false);
            _camera.orthographicSize = 5;
            _enemiesManager.DisableEnemy(_enemy.GetIndex);
            _playerMovement.EnablePlayerMovement();
        }
        yield return null;
    }

    public void UpdateHPUI (TextMeshProUGUI lifeTxt, int currentLife, int totalLife)
    {
        lifeTxt.text = currentLife + "/" + totalLife;
    }

    public int RollDice ()
    {
        return Random.Range(1, 11);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTurnBased : MonoBehaviour
{
    [SerializeField] private turnbasedScript _turnbasedScript;
    [SerializeField] private SmoothFollow _cameraSmoothFollow;
    private playerWalk _playerMovement;
    private playerJump _playerJump;
    private int playerLife = 10;
    private bool _playerIsJumping = false;
    private bool once = false;

    #region Getter & Setters
    public int GetPlayerLife { get => playerLife; }
    public int SetPlayerLife { set => playerLife = value; }

    #endregion

    private void Awake()
    {
        _playerMovement = GetComponent<playerWalk>();
        _playerJump = GetComponent<playerJump>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag(StringUtils.Tags.Enemy))
        {
            _cameraSmoothFollow.smoothTime = 0;
            if (!_playerJump.GetIsJumping)
            {
                _playerMovement.DisablePlayerMovement();
                _turnbasedScript.ActivateTurnBased();
                _turnbasedScript.SetEnemyScript = collision.GetComponent<Enemy>();
            } 
            else
            {
                _playerMovement.DisablePlayerMovement();
                _playerIsJumping = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(StringUtils.Tags.Enemy))
        {
            if (_playerIsJumping && !once)
            {
                if (!_playerJump.GetIsJumping)
                {
                    once = true;
                    _turnbasedScript.ActivateTurnBased();
                    _turnbasedScript.SetEnemyScript = collision.GetComponent<Enemy>();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        once = false;
        _cameraSmoothFollow.smoothTime = 0.3f;
    }

    public void OnExitBattle ()
    {
        _playerMovement.SetMoveSpeed = _playerMovement.OriginalMoveSpeed;
        _playerMovement.FrictionAmount = _playerMovement.OriginalFrictionAmount;
    }
}

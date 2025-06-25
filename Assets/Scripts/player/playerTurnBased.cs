using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTurnBased : MonoBehaviour
{
    [SerializeField] private turnbasedScript _turnbasedScript;
    [SerializeField] private SmoothFollow _cameraSmoothFollow;
    private player _player;

    private int playerLife = 10;

    private bool _playerIsJumping = false;
    private bool once = false;

    #region Getter & Setters
    public player GetPlayerScript { get => _player; }
    public int GetPlayerLife { get => playerLife; }
    public int SetPlayerLife { set => playerLife = value; }

    #endregion

    private void Awake()
    {
        _player = GetComponent<player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Enemy"))
        {
            _cameraSmoothFollow.smoothTime = 0;
            if (!_player.isJumping)
            {
                _player.DisablePlayerMovement();
                _turnbasedScript.ActivateTurnBased();
                _turnbasedScript.SetEnemyScript = collision.GetComponent<Enemy>();
            } 
            else
            {
                _player.DisablePlayerMovement();
                _playerIsJumping = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (_playerIsJumping && !once)
            {
                if (!_player.isJumping)
                {
                    once = true;
                    _turnbasedScript.ActivateTurnBased();
                    _turnbasedScript.SetEnemyScript = collision.GetComponent<Enemy>();
                }
            }
            Debug.Log("is touching here");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        once = false;
        _cameraSmoothFollow.smoothTime = 0.3f;
    }

    public void OnExitBattle ()
    {
        _player.SetMoveSpeed = _player.OriginalMoveSpeed;
        _player.FrictionAmount = _player.OriginalFrictionAmount;
    }
}

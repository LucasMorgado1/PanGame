using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private CheckpointScriptable _checkpointData;
    private bool _enableToTeleport;
    private GameObject _player;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ReturnToCheckpoint();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _enableToTeleport = true;
            _checkpointData.checkpointPosition = this.gameObject.transform;
            _player = collision.gameObject;
            _spriteRenderer.color = Color.yellow;
        }
    }

    public void ReturnToCheckpoint ()
    {
        if (_enableToTeleport)
        {
            _player.transform.position = _checkpointData.checkpointPosition.position;
        }
    }

}

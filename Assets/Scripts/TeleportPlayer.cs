using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform[] _teleportPoints = new Transform[5];
    private int _nextPosIndex;
    private Transform _nextPos;
    [SerializeField] private float _teleportSpeed;

    private bool _playerInteracted = false;
    private GameObject _playerCharacter;
    #endregion

    #region Unity Methoids
    void Start()
    {
        _nextPos = _teleportPoints[0];
    }

    private void Update()
    {
        if (_playerInteracted)
        {
            MovingPlayerThroughPoints();
        }
    }
    #endregion

    #region Moving the Player
    private void MovingPlayerThroughPoints ()
    {
        if (_playerCharacter.transform.position == _nextPos.position && _playerInteracted)
        {
            _nextPosIndex++;
            if (_nextPosIndex >= _teleportPoints.Length)
            {
                _playerCharacter.transform.parent = null;
                _playerInteracted = false;
                _playerCharacter.GetComponent<player>().ChangeGravityScale(1);
                return;
            }
            _nextPos = _teleportPoints[_nextPosIndex];
        }
        else
        {
            _playerCharacter.transform.position = Vector3.MoveTowards(_playerCharacter.transform.position, _nextPos.position, _teleportSpeed);
        }
    }
    #endregion

    #region Getting the player
    public bool SetPlayerInteraction(GameObject obj) 
    {
        obj.transform.parent = this.transform;
        _playerCharacter = obj;
        return _playerInteracted = true;
    }
    #endregion
}

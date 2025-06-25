using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurnbasedInfo", menuName = "Turnbased")]
public class TurnbasedData : ScriptableObject
{
    public GameObject enemySelected;

    public bool enemyWasSelected = false;
}

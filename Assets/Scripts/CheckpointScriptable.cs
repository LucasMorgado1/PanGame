using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Checkpoints", menuName = "ScriptableObjects/CheckPoint", order = 1)]
public class CheckpointScriptable : ScriptableObject
{
    public Transform checkpointPosition;
    public GameObject objectToMove;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Index : MonoBehaviour
{
    [Header("Index Number")]
    [SerializeField] private int nIndex = default;

    public int GetIndex { get => nIndex; }
}

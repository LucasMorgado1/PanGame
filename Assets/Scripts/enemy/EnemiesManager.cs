using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemiesInScene;
    [SerializeField] private int indexTest;

    private void Awake()
    {
        EnableAllEnemies();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) 
        {
            DisableEnemy(indexTest);
        }
    }

    public void EnableAllEnemies ()
    {
        foreach (GameObject enemy in _enemiesInScene)
        {
            enemy.SetActive(true);
        }
    }

    public void DisableEnemy(int index)
    {
        transform.GetChild(index).gameObject.SetActive(false);
    }
}

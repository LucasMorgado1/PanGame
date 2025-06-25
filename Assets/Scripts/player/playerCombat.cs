using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCombat : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask enemyMask;

    [Header("Damage")]
    [SerializeField] private int playerDamage;
    [SerializeField] private float playerAttackSpeed;
    private bool canAttack = true;

    void Awake()
    {
        attackPoint = transform.GetChild(1).transform;  
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && canAttack)
        {
            Debug.Log("ATTACKED");
            Attack();
            canAttack = false;
            ApplyAttackCD();
        }   
    }

    private void Attack ()
    {
        Collider2D[] enemiesHitted = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyMask);

        foreach(Collider2D enemy in enemiesHitted)
        {
            enemy.GetComponent<Enemy>().TakeDamage(playerDamage);            
        }
    }

    private void ApplyAttackCD ()
    {
        playerAttackSpeed *= Time.deltaTime;
        Debug.Log("playerAttackSpeed: " + playerAttackSpeed);

        if (playerAttackSpeed == 0.3f)
        {
            canAttack = true;
            playerAttackSpeed = 0;
            Debug.Log("the player can attack again");
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null)
            return;
        Gizmos.color = Color.green;
       // Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

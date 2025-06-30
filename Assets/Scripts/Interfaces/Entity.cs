using System.Collections;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);
}

public interface IWalkable
{
    void Walk();
}

public interface IJump
{
    void Jump();
}

public interface IDash
{
    IEnumerator Dash();
}


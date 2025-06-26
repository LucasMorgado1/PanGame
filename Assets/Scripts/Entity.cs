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


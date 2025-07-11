public interface IKillable
{
    bool TakeDamage(int damage);
    void HealHealth(int health);
    void RestoreMaxHealth();
}

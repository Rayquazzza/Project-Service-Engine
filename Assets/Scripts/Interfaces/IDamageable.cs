public interface IDamageable
{
    float Health { get; }
    void TakeDamage(float amount);
    bool IsDead { get; }
}
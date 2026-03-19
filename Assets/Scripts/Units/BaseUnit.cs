using UnityEngine;

public abstract class BaseUnit : IOccupant, IDamageable
{
    private const string LogTag = "<b><color=#797528>[Unit]</color></b>";

    public Player OwnerId { get; private set; }
    public Vector2Int GridPosition { get; set; }

    public float Health { get; protected set; }
    public int MoveRange { get; protected set; }
    public int AttackPower { get; protected set; }

    public UnitDataSO UnitData { get; protected set; }

    float IDamageable.Health => Health;

    public bool IsDead => Health <= 0;

    protected BaseUnit(UnitDataSO data, Player owner)
    {
        UnitData = data;
        Health = data.MaxHealth;
        MoveRange = data.MoveRange;
        AttackPower = data.AttackPower;
        OwnerId = owner;
    }

    public virtual void OnPlaced(Vector2Int position)
    {
        GridPosition = position;
        //Debug.Log($"{LogTag} {GetType().Name} placé en {position}");
    }

    public virtual void OnRemoved()
    {
        //Debug.Log($"{LogTag} {GetType().Name} retiré de la grille");
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;

        Debug.Log($"{LogTag} {UnitData.UnitName} du joueur {OwnerId} a subi {amount} dégâts. Reste : {Health}");
    }
}
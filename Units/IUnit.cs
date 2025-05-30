using UnityEngine;

public interface IUnit
{
    public void SetSiegeTransform(Transform transform);
    public void SetDefendTransform(Transform transform);
    public void SetSpawnerTransform(Transform transform);
    public ETeam GetTeam();
    public void TakeDamage(int damage);
    public int GetEnergyCost();
}
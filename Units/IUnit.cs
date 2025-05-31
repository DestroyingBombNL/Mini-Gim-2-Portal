using UnityEngine;

public interface IUnit
{
    public void SetSiegeTransform(Transform transform);
    public void SetDefendTransform(Transform transform);
    public void SetSpawnerTransform(Transform transform);
    public void SetTeam(ETeam team);
    public ETeam GetTeam();
    public EUnit GetUnitType();
    public void TakeDamage(int damage);
    public int GetEnergyCost();
    public bool GetIsSieging();
}
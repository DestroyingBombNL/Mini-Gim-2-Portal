using UnityEngine;

public interface IUnit
{
    public void SetAlliedPortalTransform(Transform transform);
    public void SetEnemyPortalTransform(Transform transform);
    public ETeam GetTeam();
    public void TakeDamage(int damage);
    public int GetEnergyCost();
}
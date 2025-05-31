using UnityEngine;

public interface IUnitSystem
{
    public bool SpawnUnit(ETeam team, EUnit unitType);
    public void TakeDamage(ETeam team, int damage);
    public GameObject getUnitGameObject(ETeam team, EUnit unitType);
    public Transform getSiegeTransform(ETeam team);
}
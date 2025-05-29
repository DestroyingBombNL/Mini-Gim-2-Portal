using UnityEngine;

public interface IUnitSystem
{
    public void SpawnUnit(EUnit unitType);
    public void TakeDamage(int damage);
    public ETeam GetTeam();
    public GameObject getUnitGameObject(EUnit unitType);
}
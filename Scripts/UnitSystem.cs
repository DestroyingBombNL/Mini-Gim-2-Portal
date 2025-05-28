using System.Collections.Generic;
using UnityEngine;

public class UnitSystem : MonoBehaviour, IUnitSystem
{
    [SerializeField] private ETeam team;
    [SerializeField] private int health;
    [SerializeField] private Transform alliedPortalTransform;
    [SerializeField] private Transform enemyPortalTransform;
    [SerializeField] private Dictionary<EUnit, GameObject> unitPrefabMap;
    private IResourceSystem resourceSystem;

    void Start()
    {
        this.resourceSystem = ServiceLocator.Get<IResourceSystem>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            OnDefeated();
        }
    }

    public void SpawnUnit(EUnit unitType)
    {
        GameObject unitGameObject = unitPrefabMap[unitType];
        IUnit unitScript = unitGameObject.GetComponent<IUnit>();
        if (this.resourceSystem.RemoveEnergy(unitScript))
        {
            GameObject instantiatedUnit = Instantiate(unitGameObject, this.transform.position, Quaternion.identity);
            IUnit instantiatedUnitScript = instantiatedUnit.GetComponent<IUnit>();
            instantiatedUnitScript.SetAlliedPortalTransform(alliedPortalTransform);
            instantiatedUnitScript.SetEnemyPortalTransform(enemyPortalTransform);
        }
    }

    public void OnDefeated()
    {
        Debug.Log("Game Over");
    }
}

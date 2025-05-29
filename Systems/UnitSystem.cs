using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitPrefabEntry
{
    public EUnit unit;
    public GameObject prefab;
}

public class UnitSystem : MonoBehaviour, IUnitSystem
{
    [SerializeField] private ETeam team;
    [SerializeField] private int health;
    [SerializeField] private Transform alliedPortalTransform;
    [SerializeField] private Transform enemyPortalTransform;
    [SerializeField] private List<UnitPrefabEntry> unitPrefabList;
    private IResourceSystem resourceSystem;

    void Start()
    {
        this.resourceSystem = ServiceLocator.Get<ResourceSystem>();
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
        GameObject unitGameObject = null;

        foreach (var entry in unitPrefabList)
        {
            if (entry.unit == unitType)
            {
                unitGameObject = entry.prefab;
                break;
            }
        }

        if (unitGameObject == null)
        {
            Debug.LogError($"No prefab found for unit type {unitType}");
            return;
        }

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

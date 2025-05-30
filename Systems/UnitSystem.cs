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
    [SerializeField] private List<UnitPrefabEntry> unitPrefabList;
    [SerializeField] private float spawnYOffsetMin; //0.25f
    [SerializeField] private float spawnYOffsetMax; //0.5f
    [SerializeField] private Transform spawnerTransform;
    [SerializeField] private Transform defendTransform;
    [SerializeField] private Transform siegeTransform;
    [SerializeField] private Transform unitContainerTransform; //Where units in Editor spawn

    private IResourceSystem resourceSystem;

    void Start()
    {
        this.resourceSystem = ServiceLocator.Get<ResourceSystem>();
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            OnDefeated();
        }
    }

    public ETeam GetTeam()
    {
        return this.team;
    }

    public void SpawnUnit(EUnit unitType)
    {
        GameObject unitGameObject = getUnitGameObject(unitType);

        if (this.resourceSystem == null)
        {
            this.resourceSystem = ServiceLocator.Get<ResourceSystem>();
        }

        if (unitGameObject == null)
        {
            Debug.LogError($"No prefab found for unit type {unitType}");
            return;
        }

        IUnit unitScript = unitGameObject.GetComponent<IUnit>();
        if (this.resourceSystem.RemoveEnergy(unitScript))
        {
            Vector3 spawnPosition = this.transform.position;
            spawnPosition.y += UnityEngine.Random.Range(spawnYOffsetMin, spawnYOffsetMax);
            GameObject instantiatedUnit = Instantiate(unitGameObject, spawnPosition, Quaternion.identity, unitContainerTransform);
            IUnit instantiatedUnitScript = instantiatedUnit.GetComponent<IUnit>();
            instantiatedUnitScript.SetSpawnerTransform(spawnerTransform);
            instantiatedUnitScript.SetDefendTransform(defendTransform);
            instantiatedUnitScript.SetSiegeTransform(siegeTransform);
        }
    }

    public void OnDefeated()
    {
        Debug.Log("Game Over");
    }

    public GameObject getUnitGameObject(EUnit unitType)
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
        return unitGameObject;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitPrefabEntry
{
    public EUnit unit;
    public GameObject prefab;
}

[System.Serializable]
public class TeamPassiveEnergyGainEntry
{
    public ETeam team;
    public int passiveEnergyGain;
}

[System.Serializable]
public class TeamPassiveEnergyGainEntry
{
    public ETeam team;
    public int passiveEnergyGain;
}

[System.Serializable]
public class TeamPassiveEnergyGainEntry
{
    public ETeam team;
    public int passiveEnergyGain;
}

[System.Serializable]
public class TeamPassiveEnergyGainEntry
{
    public ETeam team;
    public int passiveEnergyGain;
}


public class UnitSystem : MonoBehaviour, IUnitSystem
{
    [SerializeField] private List<UnitPrefabEntry> unitPrefabList;
    [SerializeField] private TeamHealthEntry[] TeamHealthEntries;
    [SerializeField] private TeamSpawnerTransformEntry[] TeamSpawnerTransformEntries;
    [SerializeField] private TeamDefendTransformEntry[] TeamDefendTransformEntries;
    [SerializeField] private TeamSiegeTransformEntry[] TeamSiegeTransformEntries;
    [SerializeField] private TeamUnitContainerTransformEntry[] TeamUnitContainerTransformEntries;

    private IEnergySystem energySystem;

    void Awake()
    {
        foreach (var entry in teamEnergies)
        {
            teamEnergyMap[entry.team] = entry.energy;
        }
    }

    void Start()
    {
        this.energySystem = ServiceLocator.Get<EnergySystem>();
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

        if (this.energySystem == null)
        {
            this.energySystem = ServiceLocator.Get<EnergySystem>();
        }

        if (unitGameObject == null)
        {
            Debug.LogError($"No prefab found for unit type {unitType}");
            return;
        }

        IUnit unitScript = unitGameObject.GetComponent<IUnit>();
        if (this.energySystem.RemoveEnergy(team, unitScript))
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

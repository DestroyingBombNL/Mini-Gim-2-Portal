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
public class TeamUnitEntry
{
    public ETeam team;
    public int health;
    public Transform spawnerTransform;
    public Transform defendTransform;
    public Transform siegeTransform;
    public Transform unitContainerTransform;
    public UnitPrefabEntry[] unitPrefabEntries;
}

public class UnitSystem : MonoBehaviour, IUnitSystem
{
    [SerializeField] private float spawnYOffsetMin; //0.25f
    [SerializeField] private float spawnYOffsetMax; //0.5f
    [SerializeField] private TeamUnitEntry[] teamUnitEntries;
    [SerializeField] private int playerUnitHealthBuff;
    private Dictionary<ETeam, TeamUnitEntry> teamMap = new();
    private IEnergySystem energySystem;

    void Awake()
    {
        foreach (var entry in teamUnitEntries)
        {
            teamMap[entry.team] = entry;
            if (entry.team == ETeam.Ally)
            {
                teamMap[entry.team].health += playerUnitHealthBuff;
            }
        }
    }

    void Start()
    {
        this.energySystem = ServiceLocator.Get<EnergySystem>();
    }

    public void TakeDamage(ETeam team, int damage)
    {
        ETeam teamWhichTakesDamage = ETeam.Ally == team ? ETeam.Enemy : ETeam.Ally; //Needs tp be reversed, units give the team they are on as parameter
        teamMap[teamWhichTakesDamage].health -= damage;
        if (teamMap[teamWhichTakesDamage].health <= 0)
        {
            OnDefeated(teamWhichTakesDamage);
        }
    }

    public bool SpawnUnit(ETeam team, EUnit unitType)
    {
        var entry = teamMap[team];
        GameObject unitGameObject = getUnitGameObject(team, unitType);

        if (energySystem == null)
        {
            energySystem = ServiceLocator.Get<EnergySystem>();
        }

        if (unitGameObject == null)
        {
            Debug.LogError($"No prefab found for unit type {unitType}");
            return false;
        }

        IUnit unitScript = unitGameObject.GetComponent<IUnit>();
        if (energySystem.RemoveEnergy(team, unitScript))
        {
            Vector3 spawnPosition = teamMap[team].spawnerTransform.position;
            spawnPosition.y += UnityEngine.Random.Range(spawnYOffsetMin, spawnYOffsetMax);
            GameObject instantiatedUnit = Instantiate(unitGameObject, spawnPosition, Quaternion.identity, entry.unitContainerTransform);
            IUnit instantiatedUnitScript = instantiatedUnit.GetComponent<IUnit>();
            instantiatedUnitScript.SetSpawnerTransform(entry.spawnerTransform);
            instantiatedUnitScript.SetDefendTransform(entry.defendTransform);
            instantiatedUnitScript.SetSiegeTransform(entry.siegeTransform);
            instantiatedUnitScript.SetTeam(team);
            return true;
        }
        else
        {
            Debug.Log("Not enough energy to spawn unit!");
            return false;
        }
    }

    private void OnDefeated(ETeam teamWhoLost)
    {
        if (teamWhoLost == ETeam.Ally)
        {
            Debug.Log("Game Over");
        }
        else
        {
            Debug.Log("Victorious");
        }
        Time.timeScale = 0f;
    }

    public GameObject getUnitGameObject(ETeam team, EUnit unitType)
    {
        var unitEntries = teamMap[team].unitPrefabEntries;

        foreach (var entry in unitEntries)
        {
            if (entry.unit == unitType)
            {
                return entry.prefab;
            }
        }

        return null;
    }

    public Transform getSiegeTransform(ETeam team)
    {
        return teamMap[team].siegeTransform;
    }
}

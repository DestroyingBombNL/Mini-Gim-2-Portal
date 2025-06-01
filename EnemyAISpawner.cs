using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TeamUnitsGameObjectEntry
{
    public ETeam team;
    public GameObject units;
}

public class EnemyAISpawner : MonoBehaviour
{
    [SerializeField] private TeamUnitsGameObjectEntry[] teamUnitsGameObjectEntries;
    private ETeam team = ETeam.Enemy;
    private UnitSystem unitSystem;
    private ActionSystem actionSystem;
    private EUnit unitOfChoice;
    private Dictionary<ETeam, GameObject> teamUnitsGameObjectMap = new();
    private float nextActionTime = 0f;
    private float nextUnitTime = 0f;
    private EUnit favoriteUnit;
    private System.Random random = new();

    public void Initialize()
    {
        foreach (var entry in teamUnitsGameObjectEntries)
        {
            teamUnitsGameObjectMap[entry.team] = entry.units;
        }
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        this.actionSystem = ServiceLocator.Get<ActionSystem>();
        favoriteUnit = (EUnit)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EUnit)).Length);

        DetermineNextUnit();
        this.actionSystem.SetEAction(team, EAction.Defend);
        nextActionTime = 0f;
        nextUnitTime = 0f;
        random = new();
    } 

    void Awake()
    {
        foreach (var entry in teamUnitsGameObjectEntries)
        {
            teamUnitsGameObjectMap[entry.team] = entry.units;
        }
    }

    void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        this.actionSystem = ServiceLocator.Get<ActionSystem>();
    }

    void Update()
    {
        if (Time.time >= nextActionTime)
        {
            nextActionTime = Time.time + UnityEngine.Random.Range(3f, 10f);
            DetermineNextEAction();
            //Debug.Log("Next Action: " + actionSystem.GetEAction(team));
        }

        if (Time.time >= nextUnitTime)
        {
            nextUnitTime = Time.time + UnityEngine.Random.Range(3f, 6f);
            DetermineNextUnit();
            //Debug.Log("Next Unit: " + this.unitOfChoice);
            StartCoroutine(this.unitSystem.SpawnUnitCoroutine(team, unitOfChoice));
        }
    }

    private void DetermineNextUnit()
    {
        // Map the amount of allied units per type
        Dictionary<EUnit, int> unitCounts = new();
        foreach (Transform child in teamUnitsGameObjectMap[team].transform)
        {
            IUnit unit = child.GetComponent<IUnit>();
            if (unit == null) continue;

            if (!unitCounts.ContainsKey(unit.GetUnitType()))
                unitCounts[unit.GetUnitType()] = 0;

            unitCounts[unit.GetUnitType()]++;
        }

        // If scavengerCount is lower than 3, next unit will be a scavenger
        int scavengerCount = unitCounts.ContainsKey(EUnit.Scavenger) ? unitCounts[EUnit.Scavenger] : 0;
        if (scavengerCount < 3)
        {
            unitOfChoice = EUnit.Scavenger;
            return;
        }

        // Base odds for next unit, higher = more likely
        Dictionary<EUnit, float> weights = new()
        {
            { EUnit.Scavenger, scavengerCount < 6 ? 1.75f : 0f }, // Max 6 scavengers
            { EUnit.Tank, 1f },
            { EUnit.Archer, 2f },
            { EUnit.Soldier, 3f }
        };

        // Favourite unit gets higher odds
        var keys = weights.Keys.ToList();
        foreach (var unit in keys)
        {
            if (unit == favoriteUnit)
                weights[unit] *= 1.75f;
        }


        // Add all the odds up into a range
        float totalWeight = 0f;
        foreach (var w in weights.Values)
        {
            totalWeight += w;
        }

        // Rolls a random number between 0 and range
        float roll = (float)random.NextDouble() * totalWeight;

        // See each weight as an area on a 2d bar. 
        // Keep adding unit area's until roll gets surpassed, then that unit will be spawned next.
        float cumulative = 0f;
        foreach (var entry in weights)
        {
            cumulative += entry.Value;
            if (cumulative > roll)
            {
                unitOfChoice = entry.Key;
                return;
            }
        }
    }

    private void DetermineNextEAction()
    {
        // Count the amount of allied units
        int unitCount = teamUnitsGameObjectMap[team].transform.childCount;

        // Map the amount of scavengers
        int nonScavengerCount = 0;
        foreach (Transform child in teamUnitsGameObjectMap[team].transform)
        {
            IUnit unit = child.GetComponent<IUnit>();
            if (unit != null && unit.GetUnitType() != EUnit.Scavenger)
            {
                nonScavengerCount++;
            }
        }

        // If there are at least 6 units of which 2 non-scavengers, roll a dice for whether the ai will play agressively
        // If there aren't, check how many non-scavengers ally has. If ai outnumbers enemy, then siege/keep sieging. Otherwise defend. (Overwhelm)
        if (unitCount >= 6 && nonScavengerCount >= 2)
        {
            GameObject allyUnits = teamUnitsGameObjectMap[ETeam.Ally];
            int allyNonScavenger = 0;

            foreach (Transform child in allyUnits.transform)
            {
                IUnit unit = child.GetComponent<IUnit>();
                if (unit != null && unit.GetUnitType() != EUnit.Scavenger)
                {
                    allyNonScavenger++;
                }
            }

            // Base aggression from non-scavenger count
            float aggression = Mathf.Clamp01(nonScavengerCount / 15f);

            // Calculate unit count difference (ai - ally)
            int unitDifference = unitCount - allyNonScavenger;

            // Normalize difference to 0..1, 10 or more units difference = 1
            float diffNormalized = Mathf.Clamp01(unitDifference / 10f);

            // Apply exponential ramp-up (e.g. square for smooth low and strong high)
            float diffAggression = Mathf.Pow(diffNormalized, 2);

            // Combine base aggression and difference aggression (e.g. weighted sum)
            float combinedAggression = Mathf.Clamp01(aggression + diffAggression);

            if (combinedAggression > 0.85f)
                actionSystem.SetEAction(team, EAction.Siege);
            else
                actionSystem.SetEAction(team, EAction.Defend);
        }
        else
        {
            bool anySieging = false;

            foreach (Transform child in teamUnitsGameObjectMap[team].transform)
            {
                IUnit unit = child.GetComponent<IUnit>();
                if (unit != null && unit.GetIsSieging())
                {
                    anySieging = true;
                    break;
                }
            }

            if (anySieging)
            {
                actionSystem.SetEAction(team, EAction.Siege);
            }
            else
            {
                actionSystem.SetEAction(team, EAction.Defend);
            }
        }
    }
}

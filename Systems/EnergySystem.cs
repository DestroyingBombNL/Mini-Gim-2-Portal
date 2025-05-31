using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TeamEnergyEntry
{
    public ETeam team;
    public int energy;
    public int passiveEnergyGain;
    public TextMeshProUGUI text;
    
}

public class EnergySystem : MonoBehaviour, IEnergySystem
{
    public event System.Action<ETeam, int> OnEnergyChanged;
    [SerializeField] private TeamEnergyEntry[] teamEnergyEntries;
    [SerializeField] private float passiveEnergyTimer; // = 0f;
    [SerializeField] private float passiveEnergyInterval; // = 5f;
    private Dictionary<ETeam, TeamEnergyEntry> teamEnergyMap = new();
    

    void Awake()
    {
        foreach (var entry in teamEnergyEntries)
        {
            teamEnergyMap[entry.team] = entry;
        }
    }

    void Start()
    {
        SetNewEnergyText();
    }

    void Update()
    {
        passiveEnergyTimer += Time.deltaTime;

        if (passiveEnergyTimer >= passiveEnergyInterval)
        {
            foreach (var entry in teamEnergyMap)
            {
                AddEnergy(entry.Key, entry.Value.passiveEnergyGain);
            }

            passiveEnergyTimer = 0f;
        }
    }

    public int GetEnergy(ETeam team)
    {
        return this.teamEnergyMap[team].energy;
    }

    public void SetEnergy(ETeam team, int amount)
    {
        this.teamEnergyMap[team].energy = amount;
        SetNewEnergyText();
        OnEnergyChanged?.Invoke(team, this.teamEnergyMap[team].energy);
    }

    public void AddEnergy(ETeam team, int amount)
    {
        this.teamEnergyMap[team].energy += amount;
        SetNewEnergyText();
        OnEnergyChanged?.Invoke(team, this.teamEnergyMap[team].energy);
    }

    public void AddEnergy(ETeam team, IFruit fruit)
    {
        int amount = fruit.GetEnergyAmount();
        this.teamEnergyMap[team].energy += amount;
        SetNewEnergyText();
        OnEnergyChanged?.Invoke(team, this.teamEnergyMap[team].energy);
    }

    public bool RemoveEnergy(ETeam team, IUnit unit)
    {
        int unitEnergyCost = unit.GetEnergyCost();
        if (this.teamEnergyMap[team].energy >= unitEnergyCost)
        {
            this.teamEnergyMap[team].energy -= unitEnergyCost;
            SetNewEnergyText();
            OnEnergyChanged?.Invoke(team, this.teamEnergyMap[team].energy);
            return true;
        }
        return false;
    }

    public bool RemoveEnergy(ETeam team, int amount)
    {
        if (this.teamEnergyMap[team].energy >= amount)
        {
            this.teamEnergyMap[team].energy -= amount;
            SetNewEnergyText();
            OnEnergyChanged?.Invoke(team, this.teamEnergyMap[team].energy);
            return true;
        }
        return false;
    }

    private void SetNewEnergyText()
    {
        foreach (var entry in teamEnergyMap)
        {
            if (entry.Value.text)
            {
                entry.Value.text.text = entry.Value.energy.ToString();
            }
        }
    }
}
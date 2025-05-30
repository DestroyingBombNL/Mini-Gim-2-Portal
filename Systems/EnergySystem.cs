using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TeamEnergyEntry
{
    public ETeam team;
    public int energy;
}

[System.Serializable]
public class TeamTextEntry
{
    public ETeam team;
    public TextMeshProUGUI text;
}

[System.Serializable]
public class TeamPassiveEnergyGainEntry
{
    public ETeam team;
    public int passiveEnergyGain;
}

public class EnergySystem : MonoBehaviour, IEnergySystem
{
    public event System.Action<ETeam, int> OnEnergyChanged;
    [SerializeField] private TeamEnergyEntry[] teamEnergyEntries;
    [SerializeField] private TeamPassiveEnergyGainEntry[] teamPassiveEnergyGainEntries;
    [SerializeField] private TeamTextEntry[] teamTexts;
    [SerializeField] private float passiveEnergyTimer; // = 0f;
    [SerializeField] private float passiveEnergyInterval; // = 5f;

    private Dictionary<ETeam, int> teamEnergyMap = new();
    private Dictionary<ETeam, TextMeshProUGUI> teamTextMap = new();
    private Dictionary<ETeam, int> teamPassiveEnergyGainMap = new();
    

    void Awake()
    {
        foreach (var entry in teamEnergyEntries)
        {
            teamEnergyMap[entry.team] = entry.energy;
        }

        foreach (var entry in teamTexts)
        {
            teamTextMap[entry.team] = entry.text;
        }

        foreach (var entry in teamPassiveEnergyGainEntries)
        {
            teamPassiveEnergyGainMap[entry.team] = entry.passiveEnergyGain;
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
            foreach (var entry in teamPassiveEnergyGainMap)
            {
                AddEnergy(entry.Key, entry.Value);
            }

            passiveEnergyTimer = 0f;
        }
    }

    public int GetEnergy(ETeam team)
    {
        return this.teamEnergyMap[team];
    }

    public void SetEnergy(ETeam team, int amount)
    {
        this.teamEnergyMap[team] = amount;
        SetNewEnergyText();
        OnEnergyChanged?.Invoke(team, this.teamEnergyMap[team]);
    }

    public void AddEnergy(ETeam team, int amount)
    {
        this.teamEnergyMap[team] += amount;
        SetNewEnergyText();
        OnEnergyChanged?.Invoke(team, this.teamEnergyMap[team]);
    }

    public void AddEnergy(ETeam team, IFruit fruit)
    {
        int amount = fruit.GetEnergyAmount();
        this.teamEnergyMap[team] += amount;
        SetNewEnergyText();
        OnEnergyChanged?.Invoke(team, this.teamEnergyMap[team]);
    }

    public bool RemoveEnergy(ETeam team, IUnit unit)
    {
        int unitEnergyCost = unit.GetEnergyCost();
        if (this.teamEnergyMap[team] >= unitEnergyCost)
        {
            this.teamEnergyMap[team] -= unitEnergyCost;
            SetNewEnergyText();
            OnEnergyChanged?.Invoke(team, this.teamEnergyMap[team]);
            return true;
        }
        return false;
    }

    public bool RemoveEnergy(ETeam team, int amount)
    {
        if (this.teamEnergyMap[team] >= amount)
        {
            this.teamEnergyMap[team] -= amount;
            SetNewEnergyText();
            OnEnergyChanged?.Invoke(team, this.teamEnergyMap[team]);
            return true;
        }
        return false;
    }

    private void SetNewEnergyText()
    {
        foreach (var entry in teamTextMap)
        {
            entry.Value.text = teamEnergyMap[entry.Key].ToString();
        }
    }
}
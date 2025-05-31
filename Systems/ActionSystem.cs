using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TeamActionEntry
{
    public ETeam team;
    public EAction action;
}

public class ActionSystem : MonoBehaviour, IActionSystem
{
    public event System.Action<ETeam, EAction> OnEActionChanged;
    [SerializeField] private TeamActionEntry[] teamActionEntries;
    private Dictionary<ETeam, TeamActionEntry> teamMap = new();

    void Awake()
    {
        foreach (var entry in teamActionEntries)
        {
            teamMap[entry.team] = entry;
        }
    }

    public EAction GetEAction(ETeam team)
    {
        return this.teamMap[team].action;
    }

    public void SetEAction(ETeam team, EAction action)
    {
        this.teamMap[team].action = action;
        OnEActionChanged?.Invoke(team, this.teamMap[team].action);
    }
}
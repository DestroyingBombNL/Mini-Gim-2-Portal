using System;
using UnityEngine;

public interface ITreeSystem
{
    public event Action<ETeam, int> OnScavengerSpotsAvailableChanged;
    public GameObject GetNearestTree(ETeam team);
    public void IncreaseScavengerSpotsAvailable(ETeam team, int amount);
    public void ReduceScavengerSpotsAvailable(ETeam team, int amount);
    public int GetScavengerSpotsAvailable(ETeam team);
}
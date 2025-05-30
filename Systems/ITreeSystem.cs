using UnityEngine;

public interface ITreeSystem
{
    public event System.Action OnAlliedScavengerSpotsAvailableChanged;
    public GameObject GetNearestTree(ETeam team);
    public void IncreaseAlliedScavengerSpotsAvailable(int amount);
    public void ReduceAlliedScavengerSpotsAvailable(int amount);
    public int GetAlliedScavengerSpotsAvailable();
}
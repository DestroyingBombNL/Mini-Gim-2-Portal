using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class TeamTreeEntry
{
    public ETeam team;
    public GameObject[] trees;
}

[System.Serializable]
public class TeamPortalEntry
{
    public ETeam team;
    public Transform portal;
}


public class TreeSystem : MonoBehaviour, ITreeSystem
{
    public event Action<ETeam, int> OnScavengerSpotsAvailableChanged;
    [SerializeField] private TeamTreeEntry[] teamTrees;
    [SerializeField] private TeamPortalEntry[] teamPortals;
    private Dictionary<ETeam, GameObject[]> teamTreeMap = new();
    private Dictionary<ETeam, Transform> teamPortalMap = new();
    private Dictionary<ETeam, int> scavengerSpotsAvailable = new();

    void Awake()
    {
        foreach (var entry in teamTrees)
            teamTreeMap[entry.team] = entry.trees;

        foreach (var entry in teamPortals)
            teamPortalMap[entry.team] = entry.portal;
    }

    void Start()
    {
        // Initialize scavenger spot counts
        foreach (var team in teamTreeMap.Keys)
        {
            scavengerSpotsAvailable[team] = 0;

            foreach (GameObject tree in teamTreeMap[team])
            {
                ITree treeScript = tree.GetComponent<ITree>();
                scavengerSpotsAvailable[team] += treeScript.GetScavengerMaxCapacity() - treeScript.GetScavengerCurrentCapacity();
            }

            OnScavengerSpotsAvailableChanged?.Invoke(team, scavengerSpotsAvailable[team]);
        }
    }

    public GameObject GetNearestTree(ETeam team)
    {
        float closestDistance = float.MaxValue;
        GameObject closestTree = null;
        Transform portal = teamPortalMap[team];

        foreach (GameObject tree in teamTreeMap[team])
        {
            ITree treeScript = tree.GetComponent<ITree>();
            if (treeScript.GetScavengerCurrentCapacity() >= treeScript.GetScavengerMaxCapacity())
                continue;

            float distance = Vector3.Distance(tree.transform.position, portal.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTree = tree;
            }
        }

        return closestTree;
    }

    public void IncreaseScavengerSpotsAvailable(ETeam team, int amount)
    {
        scavengerSpotsAvailable[team] += amount;
        OnScavengerSpotsAvailableChanged?.Invoke(team, scavengerSpotsAvailable[team]);
    }

    public void ReduceScavengerSpotsAvailable(ETeam team, int amount)
    {
        scavengerSpotsAvailable[team] -= amount;
        OnScavengerSpotsAvailableChanged?.Invoke(team, scavengerSpotsAvailable[team]);
    }

    public int GetScavengerSpotsAvailable(ETeam team)
    {
        return scavengerSpotsAvailable[team];
    }

    public void RepopulateAllTreesWithFruit(ETeam team)
    {
        foreach (var tree in teamTreeMap[team])
        {
            ITree treeScript = tree.GetComponent<ITree>();
            treeScript.PopulateFruits();
        }
    }
}

using UnityEngine;

public class TreeSystem : MonoBehaviour, ITreeSystem
{
    public event System.Action OnAlliedScavengerSpotsAvailableChanged;
    [SerializeField] private int alliedScavengerSpotsAvailable;
    [SerializeField] private GameObject[] treeGameObjects;
    [SerializeField] private Transform alliedPortalTransform;
    [SerializeField] private Transform enemyPortalTransform;

    void Start()
    {
        foreach (GameObject treeGameObject in treeGameObjects)
        {
            ITree treeScript = treeGameObject.GetComponent<ITree>();
            if (treeScript.GetTeam() == ETeam.Ally)
            {
                alliedScavengerSpotsAvailable += treeScript.GetScavengerMaxCapacity() - treeScript.GetScavengerCurrentCapacity();
            }
        }
    }

    public GameObject GetNearestTree(ETeam team)
    {
        Transform portalTransform = team == ETeam.Ally ? alliedPortalTransform : enemyPortalTransform;

        float closestDistance = float.MaxValue;
        GameObject closestTreeGameObject = null;

        foreach (GameObject treeGameObject in treeGameObjects)
        {

            ITree treeScript = treeGameObject.GetComponent<ITree>();
            if (treeScript.GetTeam() != team || treeScript.GetScavengerCurrentCapacity() == treeScript.GetScavengerMaxCapacity())
                continue;

            float distance = Vector3.Distance(treeGameObject.transform.position, portalTransform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTreeGameObject = treeGameObject;
            }
        }

        return closestTreeGameObject;
    }

    public void IncreaseAlliedScavengerSpotsAvailable(int amount)
    {
        this.alliedScavengerSpotsAvailable += amount;
        OnAlliedScavengerSpotsAvailableChanged?.Invoke();
    }
    public void ReduceAlliedScavengerSpotsAvailable(int amount)
    {
        this.alliedScavengerSpotsAvailable -= amount;
        OnAlliedScavengerSpotsAvailableChanged?.Invoke();
    }

    public int GetAlliedScavengerSpotsAvailable()
    {
        return this.alliedScavengerSpotsAvailable;
    }
}

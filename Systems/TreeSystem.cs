using UnityEngine;

public class TreeSystem : MonoBehaviour, ITreeSystem
{
    [SerializeField] private GameObject[] treeGameObjects;
    [SerializeField] private Transform alliedPortalTransform;
    [SerializeField] private Transform enemyPortalTransform;

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
}

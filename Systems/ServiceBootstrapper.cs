using UnityEngine;

public class ServiceBootstrapper : MonoBehaviour
{
    [SerializeField] private TreeSystem treeSystem;
    [SerializeField] private UnitSystem unitSystem;
    [SerializeField] private ResourceSystem resourceSystem;
    [SerializeField] private ActionSystem actionSystem;

    void Awake()
    {
        if (resourceSystem == null)
        {
            resourceSystem = FindFirstObjectByType<ResourceSystem>();
        }
        ServiceLocator.Register(resourceSystem);

        if (treeSystem == null)
        {
            treeSystem = FindFirstObjectByType<TreeSystem>();
        }
        ServiceLocator.Register(treeSystem);

        if (unitSystem == null)
        {
            unitSystem = FindFirstObjectByType<UnitSystem>();
        }
        ServiceLocator.Register(unitSystem);

        if (actionSystem == null)
        {
            actionSystem = FindFirstObjectByType<ActionSystem>();
        }
        ServiceLocator.Register(actionSystem);
    }
}

using UnityEngine;

public class ServiceBootstrapper : MonoBehaviour
{
    [SerializeField] private TreeSystem treeSystem;
    [SerializeField] private UnitSystem unitSystem;
    [SerializeField] private ResourceSystem resourceSystem;

    void Awake()
    {
        treeSystem = FindFirstObjectByType<TreeSystem>();
        ServiceLocator.Register<ITreeSystem>(treeSystem);

        unitSystem = FindFirstObjectByType<UnitSystem>();
        ServiceLocator.Register<IUnitSystem>(unitSystem);
        
        resourceSystem = FindFirstObjectByType<ResourceSystem>();
        ServiceLocator.Register<IResourceSystem>(resourceSystem);
    }
}

using UnityEngine;

public class ServiceBootstrapper : MonoBehaviour
{
    [SerializeField] private TreeSystem treeSystem;
    [SerializeField] private UnitSystem unitSystem;
    [SerializeField] private EnergySystem energySystem;
    [SerializeField] private ActionSystem actionSystem;

    void Awake()
    {
        ServiceLocator.Register(treeSystem);
        ServiceLocator.Register(unitSystem);
        ServiceLocator.Register(energySystem);
        ServiceLocator.Register(actionSystem);
    }
}

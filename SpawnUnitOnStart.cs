using UnityEngine;

public class DefaultScavengerSpawn : MonoBehaviour
{
    [SerializeField] private EUnit unitType;
    private ResourceSystem resourceSystem;
    private UnitSystem unitSystem;
    
    void Start()
    {
        this.resourceSystem = ServiceLocator.Get<ResourceSystem>();
        this.unitSystem = ServiceLocator.Get<UnitSystem>();

        int unitEnergyCost = unitSystem.getUnitGameObject(unitType).GetComponent<IUnit>().GetEnergyCost();
        this.resourceSystem.AddEnergy(unitEnergyCost);
        this.unitSystem.SpawnUnit(unitType);
    }
}

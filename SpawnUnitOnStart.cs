using UnityEngine;

public class DefaultScavengerSpawn : MonoBehaviour
{
    [SerializeField] private EUnit unitType;
    [SerializeField] private ETeam team;
    private EnergySystem energySystem;
    private UnitSystem unitSystem;
    
    void Start()
    {
        this.energySystem = ServiceLocator.Get<EnergySystem>();
        this.unitSystem = ServiceLocator.Get<UnitSystem>();

        int unitEnergyCost = unitSystem.getUnitGameObject(unitType).GetComponent<IUnit>().GetEnergyCost();
        this.energySystem.AddEnergy(team, unitEnergyCost);
        this.unitSystem.SpawnUnit(unitType);
    }
}

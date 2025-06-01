using UnityEngine;

public class DefaultSpawner : MonoBehaviour, IDefaultSpawner
{
    [SerializeField] private EUnit unitType;
    [SerializeField] private ETeam team;
    private EnergySystem energySystem;
    private UnitSystem unitSystem;

    void Start()
    {
        this.energySystem = ServiceLocator.Get<EnergySystem>();
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
    }

    public void SpawnUnit()
    {
        int unitEnergyCost = unitSystem.getUnitGameObject(team, unitType).GetComponent<IUnit>().GetEnergyCost();
        this.energySystem.AddEnergy(team, unitEnergyCost);
        StartCoroutine(this.unitSystem.SpawnUnitCoroutine(team, unitType));
    }
}

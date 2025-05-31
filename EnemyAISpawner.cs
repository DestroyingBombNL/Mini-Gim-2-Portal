using System;
using System.Collections;
using UnityEngine;

public class EnemyAISpawner : MonoBehaviour
{
    private ETeam team = ETeam.Enemy;
    private UnitSystem unitSystem;
    private ActionSystem actionSystem;
    private EnergySystem energySystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        this.actionSystem = ServiceLocator.Get<ActionSystem>();
        this.energySystem = ServiceLocator.Get<EnergySystem>();
    
        foreach (EUnit unitType in Enum.GetValues(typeof(EUnit)))
        {
            int unitEnergyCost = unitSystem.getUnitGameObject(team, unitType).GetComponent<IUnit>().GetEnergyCost();

            this.energySystem.AddEnergy(team, unitEnergyCost);
            this.unitSystem.SpawnUnit(team, unitType);
        }

        // EUnit unitType = EUnit.Tank;
        // int unitEnergyCost = unitSystem.getUnitGameObject(team, unitType).GetComponent<IUnit>().GetEnergyCost();

        // this.energySystem.AddEnergy(team, unitEnergyCost);
        // this.unitSystem.SpawnUnit(team, unitType);
        
        this.actionSystem.SetEAction(ETeam.Enemy, EAction.Siege);
        //StartCoroutine(SwitchEnemyToDefendAfterDelay());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private IEnumerator SwitchEnemyToDefendAfterDelay()
    {
        yield return new WaitForSeconds(12f);
        this.actionSystem.SetEAction(ETeam.Enemy, EAction.Defend);
    }

}

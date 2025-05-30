using UnityEngine;

public class EnemyAISpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        this.unitSystem.SpawnUnit(unitType);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;
using System.Collections.Generic;

public class AutoSpawner : MonoBehaviour
{
    [SerializeField] private ETeam team;
    [SerializeField] private GameObject autoSpawnGameObject;
    private UnitSystem unitSystem;
    private float spawnTimer;
    private float tankCooldownTimer;

    private const float TankCooldownDuration = 10f;

    private List<EUnit> spawnableUnits = new();

    void Start()
    {
        unitSystem = ServiceLocator.Get<UnitSystem>();

        // Collect all units except Scavenger
        foreach (EUnit unit in System.Enum.GetValues(typeof(EUnit)))
        {
            if (unit != EUnit.Scavenger)
                spawnableUnits.Add(unit);
        }

        ResetSpawnTimer();
        tankCooldownTimer = 0f; // tank can spawn immediately
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        tankCooldownTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnRandomUnit();
            ResetSpawnTimer();
        }
    }

    public ETeam GetTeam()
    {
        return this.team;
    }

    private void SpawnRandomUnit()
    {
        if (spawnableUnits.Count == 0) return;

        // Try picking a unit, if it's a tank and tank cooldown is active, pick another unit
        EUnit unit;
        int attempts = 0;
        do
        {
            unit = spawnableUnits[Random.Range(0, spawnableUnits.Count)];
            attempts++;
        }
        while (unit == EUnit.Tank && tankCooldownTimer > 0f && attempts < 10);

        // If after attempts still tank with cooldown, skip spawning tank
        if (unit == EUnit.Tank && tankCooldownTimer > 0f)
        {
            // pick any other non-tank unit forcibly
            unit = spawnableUnits.Find(u => u != EUnit.Tank);
            if (unit == default) return; // no other unit found
        }

        unitSystem.MockSpawnUnit(team, unit, autoSpawnGameObject.transform.position);

        // Reset tank cooldown if we just spawned a tank
        if (unit == EUnit.Tank)
        {
            tankCooldownTimer = TankCooldownDuration;
        }
    }

    private void ResetSpawnTimer()
    {
        spawnTimer = Random.Range(0.5f, 1f); // Random delay between spawns
    }
}

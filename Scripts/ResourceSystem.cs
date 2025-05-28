using UnityEngine;

public class ResourceSystem : MonoBehaviour, IResourceSystem
{
    [SerializeField] private int energyAmount;
    public int GetEnergy()
    {
        return this.energyAmount;
    }

    public void SetEnergy(int amount)
    {
        this.energyAmount = amount;
    }

    public void AddEnergy(int amount)
    {
        this.energyAmount += amount;
    }

    public void AddEnergy(IFruit fruit)
    {
        this.energyAmount += fruit.GetEnergyAmount();
    }

    public bool RemoveEnergy(IUnit unit)
    {
        int unitEnergyCost = unit.GetEnergyCost();
        if (energyAmount >= unitEnergyCost)
        {
            energyAmount -= unitEnergyCost;
            return true;
        }
        return false;
    }

    public bool RemoveEnergy(int amount)
    {
        if (energyAmount >= amount)
        {
            energyAmount -= amount;
            return true;
        }
        return false;
    }
}
using TMPro;
using UnityEngine;

public class ResourceSystem : MonoBehaviour, IResourceSystem
{
    public event System.Action<int> OnEnergyChanged;
    [SerializeField] private int energyAmount;
    [SerializeField] private TextMeshProUGUI energyText;

    void Start()
    {
        SetNewEnergyText();
    }
    
    public int GetEnergy()
    {
        return this.energyAmount;
    }

    public void SetEnergy(int amount)
    {
        this.energyAmount = amount;
        SetNewEnergyText();

    }

    public void AddEnergy(int amount)
    {
        this.energyAmount += amount;
        SetNewEnergyText();
    }

    public void AddEnergy(IFruit fruit)
    {
        this.energyAmount += fruit.GetEnergyAmount();
        SetNewEnergyText();
    }

    public bool RemoveEnergy(IUnit unit)
    {
        int unitEnergyCost = unit.GetEnergyCost();
        if (energyAmount >= unitEnergyCost)
        {
            energyAmount -= unitEnergyCost;
            SetNewEnergyText();
            return true;
        }
        return false;
    }

    public bool RemoveEnergy(int amount)
    {
        if (energyAmount >= amount)
        {
            energyAmount -= amount;
            SetNewEnergyText();
            return true;
        }
        return false;
    }

    private void SetNewEnergyText()
    {
        energyText.text = GetEnergy().ToString();
        OnEnergyChanged?.Invoke(GetEnergy());
    }
}
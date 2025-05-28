using UnityEngine;

public class Fruit : MonoBehaviour, IFruit
{
    private string fruitName;
    private int energyAmount;

    public Fruit(string fruitName, int energyAmount)
    {
        this.fruitName = fruitName;
        this.energyAmount = energyAmount;
    }

    public int GetEnergyAmount()
    {
        return this.energyAmount;
    }

    public Fruit Clone()
    {
        return new Fruit(
            this.fruitName,
            this.energyAmount
        );
    }

    public bool IsClone(Fruit fruit)
    {
        return
        this.fruitName == fruit.fruitName &&
        this.energyAmount == fruit.energyAmount;
    }
}

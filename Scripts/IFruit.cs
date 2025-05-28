public interface IFruit
{
    public int GetEnergyAmount();
    public Fruit Clone();
    public bool IsClone(Fruit fruit);
}
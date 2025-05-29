public interface IResourceSystem
{
    public event System.Action<int> OnEnergyChanged;
    public int GetEnergy();
    public void SetEnergy(int amount);
    public void AddEnergy(int amount);
    public void AddEnergy(IFruit fruit);
    public bool RemoveEnergy(int amount);
    public bool RemoveEnergy(IUnit unit);               
}
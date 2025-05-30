public interface IEnergySystem
{
    public event System.Action<ETeam, int> OnEnergyChanged;
    public int GetEnergy(ETeam team);
    public void SetEnergy(ETeam team, int amount);
    public void AddEnergy(ETeam team, int amount);
    public void AddEnergy(ETeam team, IFruit fruit);
    public bool RemoveEnergy(ETeam team, IUnit unit);
    public bool RemoveEnergy(ETeam team, int amount);
}
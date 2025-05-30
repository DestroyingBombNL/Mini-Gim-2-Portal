public interface IActionSystem
{
    public event System.Action OnIsSiegingChanged;
    public bool GetIsSieging();
    public void SetIsSieging(bool isSieging);
}

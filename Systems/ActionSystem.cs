using UnityEngine;

public class ActionSystem : MonoBehaviour, IActionSystem
{
    public event System.Action OnIsSiegingChanged;
    [SerializeField] private bool isSieging; //true = sieging, false = defending

    public bool GetIsSieging()
    {
        return this.isSieging;
    }

    public void SetIsSieging(bool isSieging)
    {
        this.isSieging = isSieging;
        OnIsSiegingChanged?.Invoke();
    }
}
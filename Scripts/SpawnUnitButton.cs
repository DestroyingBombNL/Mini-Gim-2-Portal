using UnityEngine;
using UnityEngine.UI;

public class SpawnUnitButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image imageRenderer;
    [SerializeField] private Sprite sprite;
    [SerializeField] private EUnit unitType;
    private UnitSystem unitSystem;

    void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        this.imageRenderer.sprite = this.sprite;

        this.button.onClick.AddListener(() =>
        {
            this.unitSystem.SpawnUnit(unitType);
        });
    }
}

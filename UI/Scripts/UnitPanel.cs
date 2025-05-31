using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitPanel : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image imageRenderer;
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI priceField;
    [SerializeField] private EUnit unitType;
    private ETeam team = ETeam.Ally;
    private UnitSystem unitSystem;
    private EnergySystem energySystem;
    private int unitEnergyCost;

    void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        this.energySystem = ServiceLocator.Get<EnergySystem>();

        GameObject unit = this.unitSystem.getUnitGameObject(team, unitType);

        string unitName = unit.name; //Name of the prefab
        Sprite unitSprite = unit.GetComponent<SpriteRenderer>().sprite;
        this.unitEnergyCost = unit.GetComponent<IUnit>().GetEnergyCost();

        this.nameField.text = unitName;
        this.imageRenderer.sprite = unitSprite;
        this.priceField.text = unitEnergyCost.ToString();

        this.button.onClick.AddListener(() =>
        {
            this.unitSystem.SpawnUnit(team, unitType);
        });

        energySystem.OnEnergyChanged += UpdateUI;
    }

    private void UpdateUI(ETeam team, int newEnergy)
    {
        if (this.team != team)
        {
            return;
        }
        
        this.button.interactable = newEnergy >= this.unitEnergyCost ? true : false;
        Color color = this.imageRenderer.color;
        color.a = newEnergy >= this.unitEnergyCost ? 1f : 0.5f;
        this.imageRenderer.color = color;
    }
}

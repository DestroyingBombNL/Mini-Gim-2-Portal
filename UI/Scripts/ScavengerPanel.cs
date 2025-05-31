using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScavengerPanel : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image imageRenderer;
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI priceField;
    private ETeam team = ETeam.Ally;
    private EUnit unitType = EUnit.Scavenger;
    private UnitSystem unitSystem;
    private EnergySystem energySystem;
    private TreeSystem treeSystem;
    private int unitEnergyCost;

    void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        this.energySystem = ServiceLocator.Get<EnergySystem>();
        this.treeSystem = ServiceLocator.Get<TreeSystem>();

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

        this.energySystem.OnEnergyChanged += UpdateUIEnergyChanged;
        this.treeSystem.OnScavengerSpotsAvailableChanged += UpdateUIScavengerSpotsAvailableChanged;
    }

    private void UpdateUIEnergyChanged(ETeam team, int newEnergy)
    {
        if (this.team != team) {
            return;
        }

        bool energyRequirementReached = newEnergy >= this.unitEnergyCost;
        bool scavengerSlotsAvailable = this.treeSystem.GetScavengerSpotsAvailable(team) > 0;
        bool allRequirementsMet = energyRequirementReached && scavengerSlotsAvailable;

        this.button.interactable = allRequirementsMet ? true : false;
        Color color = this.imageRenderer.color;
        color.a = allRequirementsMet ? 1f : 0.5f;
        this.imageRenderer.color = color;
    }
    
    private void UpdateUIScavengerSpotsAvailableChanged(ETeam team, int spotsAvailable)
    {
        if (this.team != team) {
            return;
        }

        bool energyRequirementReached = this.energySystem.GetEnergy(team) >= this.unitEnergyCost;
        bool scavengerSlotsAvailable = spotsAvailable > 0;
        bool allRequirementsMet = energyRequirementReached && scavengerSlotsAvailable;

        this.button.interactable = allRequirementsMet ? true : false;
        Color color = this.imageRenderer.color;
        color.a = allRequirementsMet ? 1f : 0.5f;
        this.imageRenderer.color = color;
    }
}

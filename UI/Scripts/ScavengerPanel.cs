using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScavengerPanel : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image imageRenderer;
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI priceField;
    private EUnit unitType = EUnit.Scavenger;
    private UnitSystem unitSystem;
    private ResourceSystem resourceSystem;
    private TreeSystem treeSystem;
    private int unitEnergyCost;

    void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        this.resourceSystem = ServiceLocator.Get<ResourceSystem>();
        this.treeSystem = ServiceLocator.Get<TreeSystem>();

        GameObject unit = this.unitSystem.getUnitGameObject(unitType);

        string unitName = unit.name; //Name of the prefab
        Sprite unitSprite = unit.GetComponent<SpriteRenderer>().sprite;
        this.unitEnergyCost = unit.GetComponent<IUnit>().GetEnergyCost();

        this.nameField.text = unitName;
        this.imageRenderer.sprite = unitSprite;
        this.priceField.text = unitEnergyCost.ToString();

        this.button.onClick.AddListener(() =>
        {
            this.unitSystem.SpawnUnit(unitType);
        });

        this.resourceSystem.OnEnergyChanged += UpdateUI;
        this.treeSystem.OnAlliedScavengerSpotsAvailableChanged += UpdateUI;
    }

    private void UpdateUI(int newEnergy)
    {
        bool energyRequirementReached = newEnergy >= this.unitEnergyCost;
        bool scavengerSlotsAvailable = this.treeSystem.GetAlliedScavengerSpotsAvailable() > 0;
        bool allRequirementsMet = energyRequirementReached && scavengerSlotsAvailable;

        this.button.interactable = allRequirementsMet ? true : false;
        Color color = this.imageRenderer.color;
        color.a = allRequirementsMet ? 1f : 0.5f;
        this.imageRenderer.color = color;
    }
    
    private void UpdateUI()
    {
        bool energyRequirementReached = this.resourceSystem.GetEnergy() >= this.unitEnergyCost;
        bool scavengerSlotsAvailable = this.treeSystem.GetAlliedScavengerSpotsAvailable() > 0;
        bool allRequirementsMet = energyRequirementReached && scavengerSlotsAvailable;

        this.button.interactable = allRequirementsMet ? true : false;
        Color color = this.imageRenderer.color;
        color.a = allRequirementsMet ? 1f : 0.5f;
        this.imageRenderer.color = color;
    }
}

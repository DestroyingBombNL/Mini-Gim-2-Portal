using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScavengerPanel : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image imageRenderer;
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI priceField;
    [SerializeField] private Image imageBackground;
    private ETeam team = ETeam.Ally;
    private EUnit unitType = EUnit.Scavenger;
    private UnitSystem unitSystem;
    private EnergySystem energySystem;
    private TreeSystem treeSystem;
    private int unitEnergyCost;
    private bool cooldown;
    private bool pendingEnergyUpdate = false;
    private bool pendingScavengerUpdate = false;

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
            StartCoroutine(this.unitSystem.SpawnUnitCoroutine(team, unitType));
        });

        this.energySystem.OnEnergyChanged += UpdateUIEnergyChanged;
        this.treeSystem.OnScavengerSpotsAvailableChanged += UpdateUIScavengerSpotsAvailableChanged;
        this.unitSystem.OnUnitSpawned += HandleOnSpawned;
    }

    private void UpdateUIEnergyChanged(ETeam team, int newEnergy)
    {
        if (this.team != team)
            return;

        if (cooldown)
        {
            pendingEnergyUpdate = true;
            return;
        }

        ApplyUIState();
    }

    private void UpdateUIScavengerSpotsAvailableChanged(ETeam team, int spotsAvailable)
    {
        if (this.team != team)
            return;

        if (cooldown)
        {
            pendingScavengerUpdate = true;
            return;
        }

        ApplyUIState();
    }

    private void HandleOnSpawned(ETeam team, float duration)
    {
        if (this)
        {
            StartCoroutine(DisableUI(team, duration));
        }

    }

    private void ApplyUIState()
    {
        bool energyRequirementReached = this.energySystem.GetEnergy(team) >= this.unitEnergyCost;
        bool scavengerSlotsAvailable = this.treeSystem.GetScavengerSpotsAvailable(team) > 0;
        bool allRequirementsMet = energyRequirementReached && scavengerSlotsAvailable;

        this.button.interactable = allRequirementsMet;
        Color color = this.imageRenderer.color;
        color.a = allRequirementsMet ? 1f : 0.5f;
        this.imageRenderer.color = color;
    }

    private IEnumerator DisableUI(ETeam team, float duration)
    {
        if (this.team == team)
        {
            this.cooldown = true;
            button.interactable = false;

            Color colorBackground = this.imageBackground.color;
            colorBackground.a = 0.5f;
            this.imageBackground.color = colorBackground;

            Color colorLogo = this.imageRenderer.color;
            colorLogo.a = 0.5f;
            this.imageRenderer.color = colorLogo;

            yield return StartCoroutine(EnableButtonAfterFill(duration));

            this.cooldown = false;

            // Apply any pending updates
            if (pendingEnergyUpdate || pendingScavengerUpdate)
            {
                pendingEnergyUpdate = false;
                pendingScavengerUpdate = false;
                ApplyUIState();
            }
        }

        yield break;
    }

    private IEnumerator EnableButtonAfterFill(float fillDuration)
    {
        float timer = 0f;
        imageBackground.fillAmount = 0f;

        while (timer < fillDuration)
        {
            timer += Time.deltaTime;
            imageBackground.fillAmount = timer / fillDuration;
            yield return null;
        }

        imageBackground.fillAmount = 1f;
    }
}

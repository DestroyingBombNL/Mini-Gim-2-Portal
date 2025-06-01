using System.Collections;
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
    [SerializeField] private Image imageBackground;
    private ETeam team = ETeam.Ally;
    private UnitSystem unitSystem;
    private EnergySystem energySystem;
    private int unitEnergyCost;
    private bool cooldown;

    void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        this.energySystem = ServiceLocator.Get<EnergySystem>();
        this.unitSystem.OnUnitSpawned += HandleOnSpawned;

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

        energySystem.OnEnergyChanged += UpdateUI;
    }

    private void UpdateUI(ETeam team, int newEnergy)
    {
        if (this.team != team)
            return;

        if (cooldown)
        {
            StartCoroutine(WaitForCooldownThenUpdateUI(newEnergy));
            return;
        }

        ApplyUIState(newEnergy);
    }

    private IEnumerator WaitForCooldownThenUpdateUI(int newEnergy)
    {
        yield return new WaitUntil(() => cooldown == false);
        ApplyUIState(newEnergy);
    }

    private void ApplyUIState(int newEnergy)
    {
        button.interactable = newEnergy >= unitEnergyCost;
        Color color = imageRenderer.color;
        color.a = newEnergy >= unitEnergyCost ? 1f : 0.5f;
        imageRenderer.color = color;

        Color colorBackground = this.imageBackground.color;
        colorBackground.a = newEnergy >= unitEnergyCost ? 1f : 0.5f;
        this.imageBackground.color = colorBackground;
    }

    private void HandleOnSpawned(ETeam team, float duration)
    {
        StartCoroutine(DisableUI(team, duration));
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
            ApplyUIState(this.energySystem.GetEnergy(team));
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

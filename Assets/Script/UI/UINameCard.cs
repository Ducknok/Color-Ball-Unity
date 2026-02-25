using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UINameCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data Source")]
    [SerializeField] private UpgradeCardData cardData;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI typeTxt;
    [SerializeField] private Image cardIcon;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI description;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem glowParticle;

    [Header("Rarity Colors")]
    [SerializeField] private Color commonColor = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private Color rareColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color epicColor = new Color(0.8f, 0.2f, 1f);
    [SerializeField] private Color legendaryColor = new Color(1f, 0.8f, 0f);

    private Action<UpgradeCardData> _onCardClickCallback;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovering = false;
    private int levelToShow = 1;

    private void Awake()
    {
        if (typeTxt == null) typeTxt = transform.Find("Type_bg/Type_txt")?.GetComponent<TextMeshProUGUI>();
        if (cardIcon == null) cardIcon = transform.Find("UpgradeTypeIcon_bg")?.GetComponent<Image>();
        if (cardName == null) cardName = transform.Find("UpgradeName_txt")?.GetComponent<TextMeshProUGUI>();
        if (description == null) description = transform.Find("Description_txt")?.GetComponent<TextMeshProUGUI>();

        if (glowParticle == null)
        {
            Transform glowTrans = transform.Find("GlowEffect_particle");
            if (glowTrans != null) glowParticle = glowTrans.GetComponent<ParticleSystem>();
        }

        originalScale = transform.localScale;
        if (originalScale.x == 0 || originalScale.y == 0) originalScale = Vector3.one;

        targetScale = originalScale;

        if (glowParticle != null)
        {
            var main = glowParticle.main;
            main.loop = true;
            main.playOnAwake = false;

            main.useUnscaledTime = true;

            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            glowParticle.Stop();
            glowParticle.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * 15f);
    }

    public void Setup(UpgradeCardData data, int level, Action<UpgradeCardData> onCardSelected)
    {
        cardData = data;
        levelToShow = level;
        _onCardClickCallback = onCardSelected;

        transform.localScale = Vector3.zero;

        targetScale = originalScale;

        isHovering = false;
        if (glowParticle != null) glowParticle.gameObject.SetActive(false);

        RefreshUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_onCardClickCallback != null)
        {
            targetScale = originalScale;
            isHovering = false;
            if (glowParticle != null) glowParticle.gameObject.SetActive(false);
            _onCardClickCallback.Invoke(cardData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = originalScale * 1.05f;

        if (glowParticle != null)
        {
            glowParticle.gameObject.SetActive(true);
            glowParticle.transform.localPosition = new Vector3(0, 0, -10f);
            glowParticle.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetScale = originalScale;

        if (glowParticle != null)
        {
            glowParticle.Stop();
            glowParticle.gameObject.SetActive(false);
        }
    }

    private void RefreshUI()
    {
        if (cardData == null) return;

        if (cardIcon != null) cardIcon.sprite = cardData.icon;
        if (typeTxt != null) typeTxt.text = cardData.rarity.ToString();

        if (cardName != null)
            cardName.text = $"{cardData.upgradeType} <size=70%>Lv.{levelToShow}</size>";

        var levelData = cardData.GetLevelData(levelToShow);
        if (description != null) description.text = levelData != null ? levelData.description : "No description";

        Color rarityColor = GetColorByRarity(cardData.rarity);

        if (typeTxt != null) typeTxt.color = rarityColor;
        if (cardName != null) cardName.color = rarityColor;

        if (glowParticle != null)
        {
            var main = glowParticle.main;
            main.startColor = new Color(rarityColor.r, rarityColor.g, rarityColor.b, 0.6f);
        }
    }

    private Color GetColorByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.COMMON: return commonColor;
            case Rarity.RARE: return rareColor;
            case Rarity.EPIC: return epicColor;
            case Rarity.LEGENDARY: return legendaryColor;
            default: return Color.white;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        EditorApplication.delayCall += OnEditorValidate;
    }

    private void OnEditorValidate()
    {
        if (this == null || gameObject == null) return;
        if (cardData != null)
        {
            string newName = $"UI_{cardData.upgradeType}";
            if (gameObject.name != newName)
            {
                gameObject.name = newName;
                EditorUtility.SetDirty(gameObject);
            }
            RefreshUI();
        }
    }
#endif
}
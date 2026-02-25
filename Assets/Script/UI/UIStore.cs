using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro; // Thư viện để dùng Text

public class UIStore : MonoBehaviour
{
    public static UIStore Instance; // Singleton để các thẻ Card dễ gọi tới

    [Header("Data")]
    [SerializeField] private List<SkinData> allSkins;

    [Header("UI References")]
    [SerializeField] private UISkinCard skinCardPrefab;
    [SerializeField] private Transform contentContainer;

    [Header("Coin UI")]
    [Tooltip("Kéo Text hiển thị số tiền góc trên Shop vào đây")]
    [SerializeField] private TextMeshProUGUI coinDisplayText;

    private List<UISkinCard> spawnedCards = new List<UISkinCard>();

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        UpdateCoinDisplay();
    }

    private void Start()
    {
        SpawnAndSortCards();
        ApplyEquippedSkinToPlayer(); 
    }

    public void ApplyEquippedSkinToPlayer()
    {
        string equippedSkinName = PlayerPrefs.GetString("EquippedSkin", "");
        if (string.IsNullOrEmpty(equippedSkinName) && allSkins.Count > 0)
        {
            equippedSkinName = allSkins[0].skinName;
            PlayerPrefs.SetString("EquippedSkin", equippedSkinName);
        }

        SkinData equippedData = allSkins.FirstOrDefault(s => s.skinName == equippedSkinName);
        if (equippedData != null && PlayerController.Instance != null)
        {
            PlayerController.Instance.ApplySkin(equippedData);
        }
    }
    public void UpdateCoinDisplay()
    {
        if (coinDisplayText != null && DataManager.Instance != null)
        {
            coinDisplayText.text = DataManager.Instance.TotalCoins.ToString();
        }
    }

    public void RefreshAllCards()
    {
        foreach (var card in spawnedCards)
        {
            // Dùng hàm Setup truyền lại đúng data cũ của nó để nó check lại trạng thái
            card.Setup(card.skinData);
        }
    }

    [ContextMenu("Spawn And Sort Cards")]
    public void SpawnAndSortCards()
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
        spawnedCards.Clear();

        if (allSkins == null || allSkins.Count == 0) return;

        var sortedSkins = allSkins
            .OrderBy(skin => skin.rarity)
            .ThenBy(skin => skin.price)
            .ToList();

        foreach (var skinData in sortedSkins)
        {
            UISkinCard newCard = Instantiate(skinCardPrefab, contentContainer);
            newCard.Setup(skinData);

            // Lưu lại danh sách thẻ đã đẻ ra để sau này Refresh
            spawnedCards.Add(newCard);
        }
    }
}
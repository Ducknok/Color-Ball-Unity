using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUpgrade : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private List<UpgradeCardData> listCard;

    [Header("Special Cards")]
    [SerializeField] private UpgradeCardData goldRewardCard; // Kéo thẻ Tặng Vàng vào đây (Bắt buộc)
    [SerializeField] private int maxActiveSkills = 4; // Giới hạn số lượng kỹ năng tối đa

    [Header("UI References")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Transform upgradeContainer;
    [SerializeField] private UINameCard cardPrefab;

    // Dictionary để lưu cấp độ hiện tại của từng kỹ năng
    // Key: Loại kỹ năng, Value: Cấp độ hiện tại
    private Dictionary<UpgradeType, int> skillLevels = new Dictionary<UpgradeType, int>();

    // Biến cờ để chặn việc gọi hàm 2 lần liên tiếp
    private bool isProcessing = false;

    private void Awake()
    {
        upgradePanel = this.gameObject;

        if (upgradeContainer == null)
        {
            Transform containerTrans = transform.Find("UICardContainer");
            if (containerTrans != null) upgradeContainer = containerTrans;
            else
            {
                containerTrans = transform.Find("Panel/UICardContainer");
                if (containerTrans != null) upgradeContainer = containerTrans;
            }
        }
    }

    private void Start()
    {
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (listCard == null || listCard.Count == 0) LoadAllCard();
    }

    [ContextMenu("Load All Card")]
    private void LoadAllCard()
    {
        UpgradeCardData[] loadedCards = Resources.LoadAll<UpgradeCardData>("Upgrades");
        listCard = new List<UpgradeCardData>(loadedCards);
        Debug.Log($"Đã load xong {listCard.Count} thẻ!");
    }

    // --- HÀM GỌI KHI LÊN CẤP ---
    public void ShowUpgradeOptions()
    {
        if (isProcessing || upgradePanel.activeSelf) return;

        isProcessing = true;
        this.upgradePanel.SetActive(true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }
        else
        {
            Time.timeScale = 0;
            if (PlayerController.Instance != null) PlayerController.Instance.isGameActive = false;
        }

        // Xóa sạch thẻ cũ
        foreach (Transform child in upgradeContainer)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        upgradeContainer.DetachChildren();

        List<UpgradeCardData> randomCards = GetRandomCards(3);

        StartCoroutine(SpawnCardsRoutine(randomCards));
    }

    private IEnumerator SpawnCardsRoutine(List<UpgradeCardData> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            var data = cards[i];
            UINameCard newCard = Instantiate(cardPrefab, upgradeContainer);

            // Tính Level để hiển thị
            // Nếu là thẻ Vàng (không nằm trong list skill) thì mặc định level 1 hoặc custom
            int currentLv = GetCurrentLevel(data.upgradeType);
            int nextLv = currentLv + 1;

            newCard.Setup(data, nextLv, OnCardSelected);

            // Fix lỗi flash
            newCard.transform.localScale = Vector3.zero;

            yield return new WaitForSecondsRealtime(0.15f);
        }
    }

    private void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
        isProcessing = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGameRoutine();
        }
        else
        {
            Time.timeScale = 1;
            if (PlayerController.Instance != null) PlayerController.Instance.isGameActive = true;
        }
    }

    public int GetCurrentLevel(UpgradeType type)
    {
        if (skillLevels.ContainsKey(type))
            return skillLevels[type];
        return 0;
    }

    private void OnCardSelected(UpgradeCardData selectedCardData)
    {

        if (selectedCardData == goldRewardCard)
        {
            ApplyUpgradeEffect(selectedCardData, 1); 
            HideUpgradePanel();
            return;
        }

        // Tăng cấp độ lên 1 cho Skill thường
        if (!skillLevels.ContainsKey(selectedCardData.upgradeType))
        {
            skillLevels[selectedCardData.upgradeType] = 0;
        }
        skillLevels[selectedCardData.upgradeType]++;
        int newLevel = skillLevels[selectedCardData.upgradeType];

        //Debug.Log($"Nâng cấp: {selectedCardData.upgradeType} lên Lv.{newLevel}. Số skill đang có: {skillLevels.Count}/{maxActiveSkills}");
        if (UISkillHUD.Instance != null)
        {
            UISkillHUD.Instance.AddSkillIcon(selectedCardData);
        }

        ApplyUpgradeEffect(selectedCardData, newLevel);
        HideUpgradePanel();
    }

    private void ApplyUpgradeEffect(UpgradeCardData data, int level)
    {
        if (PlayerController.Instance == null && GameManager.Instance == null) return;

        // Nếu là thẻ vàng đặc biệt
        if (data == goldRewardCard)
        {
            if (GameManager.Instance != null && GameManager.Instance.uiInfo != null)
            {
                float goldAmount = data.GetLevelData(1).value;
                GameManager.Instance.uiInfo.AddCoin((int)goldAmount); 
                Debug.Log($"Đã cộng {goldAmount} vàng!");
            }
            return;
        }

        // Các skill khác
        var levelInfo = data.GetLevelData(level);
        float value = levelInfo.value;

        switch (data.upgradeType)
        {
            case UpgradeType.Magnet:
                if (MagnetSkill.Instance != null) MagnetSkill.Instance.ActivateSkill(value);
                break;
            case UpgradeType.Shield:
                if (ShieldSkill.Instance != null) ShieldSkill.Instance.UnlockShield(value);
                break;
            case UpgradeType.DoubleGold:
                if (DoubleGoldSkill.Instance != null) DoubleGoldSkill.Instance.ActivateSkill(value);
                break;
            case UpgradeType.Ghost:
                if (GhostSkill.Instance != null) GhostSkill.Instance.ActivateSkill(value);
                break;
            case UpgradeType.FastLearner:
                if (FastLearnerSkill.Instance != null) FastLearnerSkill.Instance.ActivateSkill(value);
                    break;
            case UpgradeType.Overdrive:
                if (OverdriveSkill.Instance != null) OverdriveSkill.Instance.ActivateSkill(value);
                break;
            case UpgradeType.NanoTech:
                if (NanoTechSkill.Instance != null) NanoTechSkill.Instance.ActivateSkill(value);
                break;
        }
    }

    private List<UpgradeCardData> GetRandomCards(int amount)
    {
        List<UpgradeCardData> result = new List<UpgradeCardData>();
        List<UpgradeCardData> validPool = new List<UpgradeCardData>();

        int ownedSkillsCount = skillLevels.Count;
        bool isFullSlots = ownedSkillsCount >= maxActiveSkills;

        if (listCard != null)
        {
            foreach (var card in listCard)
            {
                if (card == goldRewardCard) continue;

                int currentLv = GetCurrentLevel(card.upgradeType);
                bool isOwned = currentLv > 0;
                bool isMaxed = false;

                if (card.levels != null && currentLv >= card.levels.Count)
                {
                    isMaxed = true;
                }

                if (isMaxed) continue;

                if (!isFullSlots || (isFullSlots && isOwned))
                {
                    validPool.Add(card);
                }
            }
        }

        if (validPool.Count == 0)
        {
            if (goldRewardCard != null)
            {
                result.Add(goldRewardCard);
                result.Add(goldRewardCard);
                result.Add(goldRewardCard);
            }
            return result;
        }

        if (validPool.Count < amount)
        {
            for (int i = 0; i < amount; i++)
            {
                // Lấy xoay vòng: 0, 1, 0, 1... hoặc 0, 0, 0...
                var card = validPool[i % validPool.Count];
                result.Add(card);
            }
            return result;
        }

        List<UpgradeCardData> tempRandomPool = new List<UpgradeCardData>(validPool);
        for (int i = 0; i < amount; i++)
        {
            if (tempRandomPool.Count == 0) break;

            int randomIndex = Random.Range(0, tempRandomPool.Count);
            result.Add(tempRandomPool[randomIndex]);

            tempRandomPool.RemoveAt(randomIndex);
        }

        return result;
    }
}
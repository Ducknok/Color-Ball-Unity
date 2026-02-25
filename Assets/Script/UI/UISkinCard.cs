using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkinCard : MonoBehaviour
{
    [Header("Set up")]
    [SerializeField] public SkinData skinData; // Đổi sang public để UIStore lấy được data

    [SerializeField] private TextMeshProUGUI typeSkinText;
    [SerializeField] private Transform ownedImage;
    [SerializeField] private TextMeshProUGUI nameSkinText;
    [SerializeField] private GameObject ballSkinObject;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("Buttons")]
    [Tooltip("Kéo Nút bấm (Button component) của thẻ vào đây")]
    [SerializeField] private Button actionButton;

    [Header("Rarity Colors")]
    [SerializeField] private Color commonColor = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private Color rareColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color epicColor = new Color(0.8f, 0.2f, 1f);
    [SerializeField] private Color legendaryColor = new Color(1f, 0.8f, 0f);

    public void Setup(SkinData data)
    {
        if (data == null) return;
        this.skinData = data;

        if (nameSkinText != null) nameSkinText.text = data.skinName;
        if (typeSkinText != null) typeSkinText.text = data.rarity.ToString();

        Color rarityColor = GetColorByRarity(data.rarity);
        if (typeSkinText != null) typeSkinText.color = rarityColor;
        if (nameSkinText != null) nameSkinText.color = rarityColor;

        if (ballSkinObject != null)
        {
            MeshRenderer meshRenderer = ballSkinObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null && data.material != null)
                meshRenderer.material = data.material;
        }

        // --- KIỂM TRA LƯU TRỮ (PLAYER PREFS) ---
        // Khóa lưu trữ sẽ có dạng: "SkinOwned_TênSkin"
        string saveKey = "SkinOwned_" + data.skinName;
        bool isOwned = PlayerPrefs.GetInt(saveKey, 0) == 1;

        // Mặc định giá = 0 thì coi như đã sở hữu
        if (data.price == 0) isOwned = true;

        if (ownedImage != null) ownedImage.gameObject.SetActive(isOwned);

        // --- CẬP NHẬT GIAO DIỆN NÚT BẤM ---
        // Kiểm tra xem skin này có đang được trang bị không
        string currentEquipped = PlayerPrefs.GetString("EquippedSkin", "");
        bool isEquipped = (currentEquipped == data.skinName);

        // LÀM MỜ VÀ KHÓA NÚT NẾU ĐANG TRANG BỊ
        if (actionButton != null)
        {
            actionButton.interactable = !isEquipped;
        }

        if (priceText != null)
        {
            if (isEquipped)
            {
                priceText.text = "ĐANG DÙNG";
                priceText.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Màu xám mờ
            }
            else if (isOwned)
            {
                priceText.text = "TRANG BỊ";
                priceText.color = Color.white;
            }
            else
            {
                priceText.text = data.price.ToString();
                priceText.color = Color.yellow;
            }
        }

        // Gắn sự kiện khi người chơi click vào nút
        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnActionButtonClicked);
        }
    }

    private void OnActionButtonClicked()
    {
        string saveKey = "SkinOwned_" + skinData.skinName;
        bool isOwned = PlayerPrefs.GetInt(saveKey, 0) == 1;
        if (skinData.price == 0) isOwned = true;

        if (isOwned)
        {
            // NẾU ĐÃ MUA -> TRANG BỊ
            PlayerPrefs.SetString("EquippedSkin", skinData.skinName);
            PlayerPrefs.Save();
            Debug.Log($"Đã trang bị skin: {skinData.skinName}");

            // 1. Áp dụng ngay diện mạo lên Player
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.ApplySkin(skinData);
            }

            // 2. Báo cho UIStore load lại toàn bộ thẻ (Nút này sẽ biến thành ĐANG DÙNG, nút cũ nhả ra)
            if (UIStore.Instance != null) UIStore.Instance.RefreshAllCards();
        }
        else
        {
            // NẾU CHƯA MUA -> KIỂM TRA TIỀN VÀ TRỪ TIỀN
            if (DataManager.Instance != null)
            {
                if (DataManager.Instance.SpendCoins(skinData.price))
                {
                    PlayerPrefs.SetInt(saveKey, 1);
                    PlayerPrefs.Save();

                    Debug.Log($"Mua thành công {skinData.skinName}!");
                    Setup(skinData);

                    // 3. Cập nhật vàng ở góc Shop
                    if (UIStore.Instance != null) UIStore.Instance.UpdateCoinDisplay();

                    // 4. Cập nhật vàng ở Màn Hình Chính
                    if (GameManager.Instance != null && GameManager.Instance.uiMainMenu != null)
                    {
                        GameManager.Instance.uiMainMenu.UpdateSavedDataUI();
                    }
                }
                else
                {
                    Debug.Log("Không đủ vàng để mua!");
                }
            }
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

    public void Toggle3DBall(bool isVisible)
    {
        if (ballSkinObject != null && ballSkinObject.activeSelf != isVisible)
        {
            ballSkinObject.SetActive(isVisible);
        }
    }
}
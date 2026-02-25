using UnityEngine;
using UnityEngine.UI;

public class UISkillSlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay;

    private UpgradeType skillType;

    public void Setup(UpgradeCardData cardData)
    {
        skillType = cardData.upgradeType;

        if (iconImage != null)
        {
            iconImage.sprite = cardData.icon;
            iconImage.enabled = true;
        }

        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0;
        }
    }

    private void Update()
    {
        UpdateCooldownVisual();
    }

    private void UpdateCooldownVisual()
    {
        if (cooldownOverlay == null) return;

        float ratio = 0f;

        switch (skillType)
        {
            case UpgradeType.Magnet:
                if (MagnetSkill.Instance != null)
                    ratio = MagnetSkill.Instance.GetCooldownRatio();
                break;

            case UpgradeType.Shield:
                if (ShieldSkill.Instance != null)
                    ratio = ShieldSkill.Instance.GetCooldownRatio();
                break;

            case UpgradeType.DoubleGold:
                if (DoubleGoldSkill.Instance != null)
                    ratio = DoubleGoldSkill.Instance.GetCooldownRatio();
                break;

            case UpgradeType.Ghost:
                if (GhostSkill.Instance != null)
                    ratio = GhostSkill.Instance.GetCooldownRatio();
                break;

            case UpgradeType.Overdrive:
                if (OverdriveSkill.Instance != null)
                    ratio = OverdriveSkill.Instance.GetCooldownRatio();
                break;

            case UpgradeType.FastLearner:
                ratio = 0f;
                break;
            case UpgradeType.NanoTech:
                if (NanoTechSkill.Instance != null)
                    ratio = NanoTechSkill.Instance.GetCooldownRatio();
                break;
        }

        // Cập nhật hình ảnh (0 = Sáng trưng, 1 = Tối thui)
        cooldownOverlay.fillAmount = ratio;
    }
}
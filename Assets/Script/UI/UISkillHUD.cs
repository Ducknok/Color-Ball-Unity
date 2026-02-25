using System.Collections.Generic;
using UnityEngine;

public class UISkillHUD : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private UISkillSlot slotPrefab;
    [SerializeField] private Transform slotContainer;

    private List<UpgradeType> addedSkills = new List<UpgradeType>();

    public static UISkillHUD Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void AddSkillIcon(UpgradeCardData cardData)
    {
        if (cardData.upgradeType == UpgradeType.None || addedSkills.Contains(cardData.upgradeType))
        {
            return;
        }
        UISkillSlot newSlot = Instantiate(slotPrefab, slotContainer);
        newSlot.Setup(cardData);
        addedSkills.Add(cardData.upgradeType);
    }
}
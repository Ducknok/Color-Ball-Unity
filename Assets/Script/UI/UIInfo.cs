using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class UIInfo : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] public TextMeshProUGUI pointTxt;
    [SerializeField] public int point;
    [SerializeField] private float scorePunchDuration = 0.2f;

    [Header("Combo")]
    [SerializeField] public TextMeshProUGUI comboTxt;
    [SerializeField] private Color comboNormalColor = Color.white;
    [SerializeField] private Color comboFlashColor = Color.yellow;
    [SerializeField] private float punchStrength = 0.5f; // Độ to khi nảy combo
    [SerializeField] private float comboPunchDuration = 0.3f;

    [Header("Time")]
    [SerializeField] public TextMeshProUGUI minuteTxt;
    [SerializeField] public TextMeshProUGUI secondTxt;
    private float timer = 0f;

    [Header("Coin")]
    [SerializeField] public TextMeshProUGUI coinTxt;
    [SerializeField] public int coin = 0;
    [SerializeField] public int bonusCoinMultiplier = 1;
    [SerializeField] private float coinPunchDuration = 0.2f;

    [Header("Exp")]
    [SerializeField] private Image expScrollbar;
    [SerializeField] private TextMeshProUGUI levelTxt;
    public int currentLevel = 1;
    public float currentXP = 0;
    public float bonusXPMultiplier;
    public float maxXP = 100;
    [SerializeField] private float xpMultiplier = 1.2f;

    [Header("References")]
    [SerializeField] private UIUpgrade upgradeManager;

    private Vector3 comboOriginalScale;
    private Vector3 pointOriginalScale;
    private Vector3 coinOriginalScale;

    private void Awake()
    {
        this.pointTxt = this.transform.Find("Point_txt/PointNumber_txt").GetComponent<TextMeshProUGUI>();
        this.comboTxt = this.transform.Find("Combo_txt").GetComponent<TextMeshProUGUI>();
        this.minuteTxt = this.transform.Find("Minute_txt").GetComponent<TextMeshProUGUI>();
        this.secondTxt = this.transform.Find("Minute_txt/Second_txt").GetComponent<TextMeshProUGUI>();
        this.coinTxt = this.transform.Find("Coin_img/Coin_txt").GetComponent<TextMeshProUGUI>();
        this.expScrollbar = this.transform.Find("Exp_bar").GetComponent<Image>();
        this.levelTxt = this.expScrollbar.GetComponentInChildren<TextMeshProUGUI>();

        if (comboTxt != null) comboOriginalScale = comboTxt.transform.localScale;
        if (pointTxt != null) pointOriginalScale = pointTxt.transform.localScale;
        if (coinTxt != null) coinOriginalScale = coinTxt.transform.localScale;
    }

    private void Start()
    {
        // Reset sạch sẽ mọi thông số khi bắt đầu ván mới
        currentLevel = 1;
        currentXP = 0;
        maxXP = 100;
        bonusXPMultiplier = 0f;
        bonusCoinMultiplier = 1;
        point = 0;
        coin = 0;
        timer = 0f;

        if (this.comboTxt != null) this.comboTxt.gameObject.SetActive(false);
        if (this.expScrollbar != null) this.expScrollbar.fillAmount = 0;

        if (upgradeManager == null)
        {
            upgradeManager = FindObjectOfType<UIUpgrade>(true);
        }

        UpdateUI();
    }

    private void Update()
    {
        this.pointTxt.text = this.point.ToString();

        if (PlayerController.Instance != null && PlayerController.Instance.isGameActive == true)
        {
            this.timer += Time.deltaTime;
            this.TimerCount(this.timer);
        }

        this.coinTxt.text = coin.ToString();
    }

    public void AddPoint()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayScore();
        this.point += 100;

        // --- BẢO VỆ DOTWEEN: Dập tắt hiệu ứng cũ trước khi bóp phình lần nữa ---
        if (pointTxt != null)
        {
            pointTxt.transform.DOKill(true);
            pointTxt.transform.localScale = pointOriginalScale;
            pointTxt.transform.DOPunchScale(Vector3.one * 0.2f, scorePunchDuration);
        }
    }

    public void AddPointWithCombo(int index)
    {
        this.point += 100 * index;

        if (pointTxt != null)
        {
            pointTxt.transform.DOKill(true);
            pointTxt.transform.localScale = pointOriginalScale;
            pointTxt.transform.DOPunchScale(Vector3.one * 0.2f, scorePunchDuration);
        }

        ShowComboUI(index);
    }

    public void ShowComboUI(int index)
    {
        if (comboTxt != null && index > 1)
        {
            comboTxt.gameObject.SetActive(true);
            comboTxt.text = "Combo x" + index;

            float shakeStrength = Mathf.Clamp(index * 0.05f, 0.1f, 0.6f);
            if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.2f, shakeStrength);

            comboTxt.transform.DOKill(true);
            comboTxt.transform.localScale = comboOriginalScale;
            comboTxt.transform.DOPunchScale(Vector3.one * punchStrength, comboPunchDuration, 10, 1);

            comboTxt.DOKill(true);
            comboTxt.color = comboFlashColor;
            comboTxt.DOColor(comboNormalColor, 0.5f).SetEase(Ease.InQuad);
        }
    }

    public void ResetComboUI()
    {
        if (comboTxt != null) comboTxt.gameObject.SetActive(false);
    }

    public void TimerCount(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (minuteTxt != null) minuteTxt.text = string.Format("{0:00} :", minutes);
        if (secondTxt != null) secondTxt.text = string.Format("{0:00}", seconds);
    }

    public void AddCoin(int amount)
    {
        this.coin += (amount * bonusCoinMultiplier);

        if (coinTxt != null)
        {
            coinTxt.transform.DOKill(true);
            coinTxt.transform.localScale = coinOriginalScale;
            coinTxt.transform.DOPunchScale(Vector3.one * 0.3f, coinPunchDuration);
        }
    }

    public void SetCoinMultiplier(int bonusCoin)
    {
        this.bonusCoinMultiplier = bonusCoin;
    }

    public void AddExperience(float amount)
    {
        float totalXP = amount * (1f + bonusXPMultiplier);
        currentXP += totalXP;

        while (currentXP >= maxXP)
        {
            currentXP -= maxXP;
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        currentLevel++;
        maxXP = Mathf.Round(maxXP * xpMultiplier);

        if (upgradeManager != null)
        {
            upgradeManager.ShowUpgradeOptions();
        }
        UpdateUI();
    }

    public void SetBonusXP(float bonusPercent)
    {
        this.bonusXPMultiplier = bonusPercent;
    }

    private void UpdateUI()
    {
        if (expScrollbar != null)
        {
            expScrollbar.fillAmount = currentXP / maxXP;
        }

        if (levelTxt != null)
        {
            levelTxt.text = "Lv. " + currentLevel;
        }
    }

    private void OnDestroy()
    {
        if (pointTxt != null) pointTxt.transform.DOKill();
        if (coinTxt != null) coinTxt.transform.DOKill();
        if (comboTxt != null) comboTxt.transform.DOKill();
    }
}
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    public UIGameOver uiGameOver;
    public UIMainMenu uiMainMenu;
    public UIInfo uiInfo;
    public UIUpgrade uiUpgrade;
    public UISetting uiSetting;

    [Header("UI info")]
    public float comboTimer;
    public int comboIndex = 0;

    [Header("Start Game Countdown")]
    public TextMeshProUGUI countdownText;

    // --- BIẾN LƯU TRẠNG THÁI CHO SETTING ---
    private bool wasPlayingBeforeSettings = false;
    private float previousTimeScale = 1f;

    // --- BIẾN CỦA NEW INPUT SYSTEM ---
    private InputAction escapeAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


        uiGameOver ??= GetComponentInChildren<UIGameOver>(true);
        uiInfo ??= GetComponentInChildren<UIInfo>(true);
        uiMainMenu ??= GetComponentInChildren<UIMainMenu>(true);
        uiUpgrade ??= GetComponentInChildren<UIUpgrade>(true);
        uiSetting ??= GetComponentInChildren<UISetting>(true);


        escapeAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/escape");
        escapeAction.performed += context => ToggleSettings();
    }

    private void OnEnable() => escapeAction?.Enable();
    private void OnDisable() => escapeAction?.Disable();

    private void Start()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.isGameActive = false;
        if (uiMainMenu != null) uiMainMenu.gameObject.SetActive(true);
        if (uiInfo != null) uiInfo.gameObject.SetActive(false);
        if (uiGameOver != null) uiGameOver.gameObject.SetActive(false);
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        if (uiUpgrade != null) uiUpgrade.gameObject.SetActive(false);
        if (uiSetting != null) uiSetting.gameObject.SetActive(false);

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (PlayerController.Instance == null || !PlayerController.Instance.isGameActive) return;

        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0)
        {
            ResetCombo();
        }
    }

    public void ToggleSettings()
    {

        if (uiSetting == null)
        {
            Debug.LogWarning("⚠️ Chưa kéo UI Setting vào GameManager!");
            return;
        }

        bool isOpening = !uiSetting.gameObject.activeSelf;
        uiSetting.gameObject.SetActive(isOpening);

        if (isOpening)
        {
            if (PlayerController.Instance != null)
            {
                wasPlayingBeforeSettings = PlayerController.Instance.isGameActive;
            }
            previousTimeScale = Time.timeScale;
            PauseGame();
        }
        else
        {
            if (wasPlayingBeforeSettings) StartGameRoutine();
            else Time.timeScale = previousTimeScale;
        }
    }

    public void OnPlayButtonClicked()
    {
        if (uiMainMenu != null) uiMainMenu.gameObject.SetActive(false);
        if (uiInfo != null) uiInfo.gameObject.SetActive(true);

        StartGameRoutine();
    }

    public void GameOver()
    {
        PauseGame();

        if (uiInfo != null && DataManager.Instance != null)
        {
            DataManager.Instance.AddCoins(uiInfo.coin);
        }

        if (uiGameOver != null) uiGameOver.gameObject.SetActive(true);
        if (uiInfo != null) uiInfo.gameObject.SetActive(false);
    }

    public void StartGameRoutine()
    {
        StartCoroutine(StartGameRoutineCoroutine());
    }

    private IEnumerator StartGameRoutineCoroutine()
    {
        if (PlayerController.Instance != null) PlayerController.Instance.isGameActive = false;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            // 5. CLEAN CODE: Sử dụng vòng lặp thay vì Hard-code (Viết tay) 3 lần
            for (int i = 3; i > 0; i--)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSecondsRealtime(1f);
            }

            countdownText.text = "GO!";
        }
        else
        {
            Debug.LogWarning("Chưa gắn Countdown Text vào GameManager!");
        }

        Time.timeScale = 1f;
        if (PlayerController.Instance != null) PlayerController.Instance.isGameActive = true;

        yield return new WaitForSeconds(0.5f);
        if (countdownText != null) countdownText.gameObject.SetActive(false);
    }

    public void PauseGame()
    {
        
        Time.timeScale = 0f;
        if (PlayerController.Instance != null) PlayerController.Instance.isGameActive = false;
    }

    public void PlayerPoint()
    {
        if (uiInfo == null) return;

        if (comboIndex < 2)
        {
            uiInfo.AddPoint();
        }
        else
        {
            uiInfo.AddPointWithCombo(comboIndex - 1);
            uiInfo.ShowComboUI(comboIndex);
        }
    }

    public void ResetCombo()
    {
        comboIndex = 0;
        comboTimer = 0;
        if (uiInfo != null && uiInfo.comboTxt != null)
            uiInfo.comboTxt.gameObject.SetActive(false);
    }
}
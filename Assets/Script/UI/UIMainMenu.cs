using TMPro;
using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [Header("Save Load")]
    [SerializeField] private TextMeshProUGUI highestScore;
    [SerializeField] private TextMeshProUGUI coin;

    private void Awake()
    {
        if (highestScore == null)
            highestScore = transform.Find("HighestScore_img/Score_txt")?.GetComponent<TextMeshProUGUI>();

        if (coin == null)
            coin = transform.Find("CoinBG_img/Coin_txt")?.GetComponent<TextMeshProUGUI>();

        // ??NG K? NGHE S? KI?N: "Khi n¨¤o DataManager t?i xong, h?y g?i h¨¤m UpdateSavedDataUI cho tao"
        if (DataManager.Instance != null)
        {
            DataManager.Instance.OnDataLoaded += UpdateSavedDataUI;
        }
    }

    // Nh? h?y ??ng k? khi UI b? t?t ?? ch?ng r¨° r? b? nh? (Memory Leak)
    private void OnDestroy()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.OnDataLoaded -= UpdateSavedDataUI;
        }
    }

    private void Start()
    {
        // V?n g?i 1 l?n l¨²c m?i b?t l¨ºn ?? ph¨°ng DataManager ?? t?i xong t? l?u r?i
        UpdateSavedDataUI();
    }

    public void UpdateSavedDataUI()
    {
        if (DataManager.Instance != null)
        {
            if (highestScore != null)
            {
                highestScore.text = DataManager.Instance.HighScore.ToString();
            }

            if (coin != null)
            {
                coin.text = DataManager.Instance.TotalCoins.ToString();
            }
        }
    }

    public void Play()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayButtonClicked();
        }
    }

    [ContextMenu("Reset All Data")]
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
using TMPro;
using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [Header("Save Load")]
    [SerializeField] private TextMeshProUGUI highestScore;
    [SerializeField] private TextMeshProUGUI coin;

    private void Awake()
    {
        // T? ??ng t¨¬m component n?u b?n qu¨ºn k¨¦o th? trong Inspector
        if (highestScore == null)
            highestScore = this.transform.Find("HighestScore_img/Score_txt")?.GetComponent<TextMeshProUGUI>();

        if (coin == null)
            coin = this.transform.Find("CoinBG_img/Coin_txt")?.GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
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
        else
        {
            Debug.LogWarning("?? Kh?ng t¨¬m th?y GameManager trong Scene!");
        }
    }

    public void Setting()
    {
    }
    [ContextMenu("??? DEV TOOL: Reset All Data")]
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
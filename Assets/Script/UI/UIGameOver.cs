using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] private NumberScrambler numberScr;
    [SerializeField] private TextMeshProUGUI newHighestScore;
    private void Awake()
    {
        numberScr = this.gameObject.transform.GetComponent<NumberScrambler>();
        newHighestScore = this.gameObject.transform.Find("ScoreStat/NewHighestScore_txt").GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        AudioManager.Instance.PlayGameOver();
    }
    private void Start()
    {
        numberScr.StartEffect(GameManager.Instance.uiInfo.point, GameManager.Instance.uiInfo.coin);
        if (DataManager.Instance.CheckAndUpdateHighScore(GameManager.Instance.uiInfo.point)) newHighestScore.gameObject.SetActive(true);
    }
    public void Restart()
    {
        SceneManager.LoadScene("Level1");
    }
    public void MainMenu()
    {
        this.gameObject.SetActive(false);
        SceneManager.LoadScene("Level1");
    }

}

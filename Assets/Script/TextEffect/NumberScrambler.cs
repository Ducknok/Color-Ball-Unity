using System.Collections;
using UnityEngine;
using TMPro;

public class NumberScrambler : MonoBehaviour
{
    [Header("Score")]
    public TextMeshProUGUI scoreTxt;
    public int scoreLength = 1;

    [Header("Coin")]
    public TextMeshProUGUI coinTxt;
    public int coinLength;

    [Header("Effect cooldown")]
    public float duration = 2f;
    public float shakeIntensity = 2f;

    private Vector3 originalScorePosition;
    private Vector3 originalCoinPosition;
    private bool isInitialized = false;

    private void InitPositions()
    {
        if (!isInitialized)
        {
            if (scoreTxt != null) originalScorePosition = scoreTxt.transform.localPosition;
            if (coinTxt != null) originalCoinPosition = coinTxt.transform.localPosition;
            isInitialized = true;
        }
    }

    public IEnumerator ScoreScrambleRoutine(int finalValue)
    {
        float elapsed = 0;
        int finalScoreLength = finalValue.ToString().Length;
        int length = Mathf.Max(scoreLength, finalScoreLength);

        // --- BIẾN ĐIỀU KHIỂN ÂM THANH ---
        float tickTimer = 0f;
        float tickInterval = 0.08f; // Cứ 0.08s phát tiếng 'tạch' 1 lần để không bị rè loa

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Dùng unscaled để chạy được khi Pause game
            tickTimer += Time.unscaledDeltaTime; // Cộng dồn thời gian cho âm thanh

            string randomStr = "";
            for (int i = 0; i < length; i++)
            {
                randomStr += Random.Range(0, 10).ToString();
            }

            if (scoreTxt != null)
            {
                scoreTxt.text = randomStr;
                float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
                float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
                scoreTxt.transform.localPosition = originalScorePosition + new Vector3(offsetX, offsetY, 0);
            }

            // --- PHÁT TIẾNG TẠCH TẠCH ĐẾM ĐIỂM ---
            if (tickTimer >= tickInterval)
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlayScoreTick();
                tickTimer = 0f; // Reset lại đồng hồ
            }

            yield return null;
        }

        if (scoreTxt != null)
        {
            scoreTxt.text = finalValue.ToString().PadLeft(length, '0');
            scoreTxt.transform.localPosition = originalScorePosition;
        }

        // --- PHÁT TIẾNG "TING!" CHỐT SỔ (Score.mp3) KHI DỪNG LẠI ---
        if (AudioManager.Instance != null) AudioManager.Instance.PlayScoreFinish();
    }

    public IEnumerator CoinScrambleRoutine(int finalValue)
    {
        float elapsed = 0;
        int finalScoreLength = finalValue.ToString().Length;
        int length = Mathf.Max(coinLength, finalScoreLength);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            string randomStr = "";
            for (int i = 0; i < length; i++)
            {
                randomStr += Random.Range(0, 10).ToString();
            }

            if (coinTxt != null)
            {
                coinTxt.text = randomStr;
                float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
                float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
                coinTxt.transform.localPosition = originalCoinPosition + new Vector3(offsetX, offsetY, 0);
            }

            yield return null;
        }

        if (coinTxt != null)
        {
            coinTxt.text = finalValue.ToString().PadLeft(length, '0');
            coinTxt.transform.localPosition = originalCoinPosition;
        }
    }

    // Hàm này sẽ được UIGameOver gọi
    public void StartEffect(int score, int coin)
    {
        InitPositions(); 
        StopAllCoroutines();
        StartCoroutine(ScoreScrambleRoutine(score));
        StartCoroutine(CoinScrambleRoutine(coin));
    }
}
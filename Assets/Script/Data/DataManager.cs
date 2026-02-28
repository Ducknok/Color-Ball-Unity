using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public event Action OnDataLoaded;

    [Header("Player Data")]
    public int TotalCoins { get; private set; }
    public int HighScore { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadData()
    {
        // Cú pháp: PlayerPrefs.GetInt("Tên_Biến", Giá_Trị_Mặc_Định_Nếu_Chưa_Có)
        TotalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        HighScore = PlayerPrefs.GetInt("HighScore", 0);

        Debug.Log($"[DataManager] Đã tải dữ liệu - Vàng: {TotalCoins}, Kỷ lục: {HighScore}");
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("TotalCoins", TotalCoins);
        PlayerPrefs.SetInt("HighScore", HighScore);

        PlayerPrefs.Save();
        Debug.Log("[DataManager] Đã lưu dữ liệu thành công!");
    }

    // --- CÁC HÀM THAO TÁC VỚI DỮ LIỆU ---

    // Gọi hàm này khi kết thúc ván chơi để cộng dồn vàng
    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        SaveData();
    }

    // Gọi hàm này khi mua đồ trong Shop
    public bool SpendCoins(int amount)
    {
        if (TotalCoins >= amount)
        {
            TotalCoins -= amount;
            SaveData();
            return true; // Mua thành công
        }
        return false; // Không đủ tiền
    }

    // Gọi hàm này khi kết thúc ván chơi để kiểm tra xem có phá kỷ lục không
    public bool CheckAndUpdateHighScore(int currentScore)
    {
        if (currentScore > HighScore)
        {
            HighScore = currentScore;
            SaveData();
            return true; // Chúc mừng, phá kỷ lục mới!
        }
        return false;
    }

    // (Dành cho Dev) Nút xóa dữ liệu để test
    [ContextMenu("Reset All Data")]
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        LoadData();
        Debug.Log("Đã xóa toàn bộ dữ liệu lưu trữ!");
    }
}
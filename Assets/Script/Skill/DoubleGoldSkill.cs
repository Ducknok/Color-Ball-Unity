using UnityEngine;

public class DoubleGoldSkill : MonoBehaviour
{
    [Header("Skill Stats")]
    public float activeDuration = 10f;   // Thời gian tác dụng (Cập nhật từ thẻ bài)
    public float cooldownDuration = 30f; // Thời gian hồi chiêu (Mặc định 30s)

    private SkillState currentState = SkillState.Locked;
    private float currentTimer = 0f;

    // Singleton để UIUpgrade gọi
    public static DoubleGoldSkill Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        switch (currentState)
        {
            case SkillState.Locked:
                break;

            case SkillState.Waiting:

                StartActivation();
                break;

            case SkillState.Active:
                currentTimer -= Time.deltaTime;

                if (currentTimer <= 0)
                {
                    DeactivateSkill();
                }
                break;

            case SkillState.Cooldown:
                currentTimer -= Time.deltaTime;
                if (currentTimer <= 0)
                {
                    StartActivation();
                }
                break;
        }
    }

    private void StartActivation()
    {
        currentState = SkillState.Active;
        currentTimer = activeDuration;

        if (GameManager.Instance != null && GameManager.Instance.uiInfo != null)
        {
            GameManager.Instance.uiInfo.SetCoinMultiplier(2);
        }
    }

    private void DeactivateSkill()
    {
        currentState = SkillState.Cooldown;
        currentTimer = cooldownDuration; // Bắt đầu đếm 30s

        // Trả về nhân 1 (bình thường)
        if (GameManager.Instance != null && GameManager.Instance.uiInfo != null)
        {
            GameManager.Instance.uiInfo.SetCoinMultiplier(1);
        }
    }

    public void ActivateSkill(float duration)
    {
        activeDuration = duration;

        currentState = SkillState.Waiting;

    }
    // --- HÀM MỚI CHO UI ---
    public float GetCooldownRatio()
    {
        if (currentState == SkillState.Locked) return 1f; // Chưa học thì tối thui
        if (currentState == SkillState.Cooldown)
        {
            // Trả về tỉ lệ thời gian còn lại (1 -> 0)
            return Mathf.Clamp01(currentTimer / cooldownDuration);
        }
        return 0f; // Đang Active thì sáng trưng
    }

}
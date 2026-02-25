using UnityEngine;

public class NanoTechSkill : MonoBehaviour
{
    [Header("Skill Stats")]
    public float activeDuration = 5f;    // Thời gian làm chậm
    public float cooldownDuration = 30f; // Hồi chiêu 30s
    [Range(0, 100)]
    public float slowPercentage = 30f;   // Giảm 30% tốc độ

    [Header("Smoothness")]
    public float smoothReturnDuration = 1.5f; // Thời gian để tăng tốc lại bình thường (tránh bị khựng)

    private SkillState currentState = SkillState.Locked;
    private float currentTimer = 0f;
    private float recoverTimer = 0f; // Timer riêng cho việc hồi phục tốc độ

    private float originalSpeed;
    private float targetSlowSpeed; // Biến mới: Lưu tốc độ đã giảm để ép cố định

    // BIẾN QUAN TRỌNG: Đánh dấu xem có đang bị làm chậm không để tránh lỗi cộng dồn
    private bool isSlowed = false;

    // Singleton
    public static NanoTechSkill Instance;

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
                PlayerController.Instance.forwardSpeed = targetSlowSpeed;

                if (currentTimer <= 0)
                {
                    StartRecovery();
                }
                break;

            case SkillState.Recovering:
                // --- TRẢ LẠI TỐC ĐỘ MƯỢT MÀ ---
                RecoverSpeed();
                break;

            case SkillState.Cooldown:
                currentTimer -= Time.deltaTime;

                // Hết hồi chiêu -> Tự bật lại
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

        if (!isSlowed)
        {
            originalSpeed = PlayerController.Instance.forwardSpeed;
        }

        float factor = 1f - (slowPercentage / 100f);
        targetSlowSpeed = originalSpeed * factor; 

        PlayerController.Instance.forwardSpeed = targetSlowSpeed;

        isSlowed = true;

    }

    private void StartRecovery()
    {
        currentState = SkillState.Recovering;
        recoverTimer = 0f;
    }

    private void RecoverSpeed()
    {

        recoverTimer += Time.deltaTime;

        float progress = recoverTimer / smoothReturnDuration;

        PlayerController.Instance.forwardSpeed = Mathf.Lerp(targetSlowSpeed, originalSpeed, progress);

        if (progress >= 1f)
        {
            PlayerController.Instance.forwardSpeed = originalSpeed;
            DeactivateSkill(); 
        }
    }

    private void DeactivateSkill()
    {
        currentState = SkillState.Cooldown;
        currentTimer = cooldownDuration;

        if (isSlowed)
        {
            PlayerController.Instance.forwardSpeed = originalSpeed;
            isSlowed = false; 
        }

        Debug.Log($"⏱️ Slow Motion kết thúc. Hồi chiêu 30s...");
    }
    public float GetCooldownRatio()
    {
        if (currentState == SkillState.Locked) return 1f;
        if (currentState == SkillState.Cooldown)
        {
            return Mathf.Clamp01(currentTimer / cooldownDuration);
        }
        return 0f;
    }
    public void ActivateSkill(float duration)
    {
        activeDuration = duration;
        currentState = SkillState.Waiting;
    }
}
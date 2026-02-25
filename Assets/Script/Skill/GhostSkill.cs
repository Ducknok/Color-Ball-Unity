using UnityEngine;

public class GhostSkill : MonoBehaviour
{
    [Header("Skill Stats")]
    public float activeDuration = 5f;   // Thời gian hiệu lực
    public float cooldownDuration = 30f; // Thời gian hồi chiêu

    [Header("Visuals")]
    [SerializeField] private Renderer playerRenderer; // Kéo MeshRenderer của Player vào đây

    [Header("Blink Warning")]
    [SerializeField] private float blinkThreshold = 2f; // Còn 2s thì bắt đầu nháy
    [SerializeField] private float blinkSpeed = 10f;    // Tốc độ nháy

    private SkillState currentState = SkillState.Locked;
    private float currentTimer = 0f;

    private Color savedColor;

    public static GhostSkill Instance;

    private void Awake()
    {
        Instance = this;
        if (playerRenderer == null) playerRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        if (currentState == SkillState.Active && playerRenderer != null)
        {
            Color currentColor = playerRenderer.material.color;
            if (currentColor != Color.white && currentColor != savedColor)
            {
                savedColor = currentColor;
            }

            if (currentTimer <= blinkThreshold)
            {
                bool showGhostColor = (int)(currentTimer * blinkSpeed) % 2 == 0;
                playerRenderer.material.color = showGhostColor ? Color.white : savedColor;
            }
            else
            {
                playerRenderer.material.color = Color.white;
            }
        }

        // --- STATE MACHINE ---
        switch (currentState)
        {
            case SkillState.Locked:
                break;

            case SkillState.Waiting:
                StartActivation();
                break;

            case SkillState.Active:
                currentTimer -= Time.deltaTime;

                // Hết giờ -> Tắt Ghost -> Hồi chiêu
                if (currentTimer <= 0)
                {
                    DeactivateGhost();
                }
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

        if (playerRenderer != null)
        {
            // 1. Lưu màu hiện tại trước khi biến hình
            savedColor = playerRenderer.material.color;

            // 2. Đổi sang màu trắng (tượng trưng cho Ghost)
            playerRenderer.material.color = Color.white;
        }

    }

    private void DeactivateGhost()
    {
        currentState = SkillState.Cooldown;
        currentTimer = cooldownDuration;

        // Trả lại màu gốc (hoặc màu mới nhất đã cập nhật trong lúc chạy)
        if (playerRenderer != null)
        {
            playerRenderer.material.color = savedColor;
        }

    }

    public bool IsGhostActive()
    {
        return currentState == SkillState.Active;
    }

    public void ActivateSkill(float duration)
    {
        activeDuration = duration;
        currentState = SkillState.Waiting;
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

}
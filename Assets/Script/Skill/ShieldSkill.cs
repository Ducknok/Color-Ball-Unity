using UnityEngine;

public class ShieldSkill : MonoBehaviour
{
    [Header("Skill Stats")]
    public float activeDuration = 5f;   // Thời gian tồn tại của khiên (Tăng theo cấp độ thẻ)
    public float cooldownDuration = 30f; // Thời gian hồi chiêu cố định (30s)

    [Header("Visuals")]
    [SerializeField] private GameObject shieldVisual; // Kéo object hình cầu khiên vào đây

    [Header("Blink Warning")]
    [SerializeField] private float blinkThreshold = 2f; // Còn 2s thì bắt đầu nháy
    [SerializeField] private float blinkSpeed = 15f;    // Tốc độ nháy (số càng to nháy càng nhanh)

    private SkillState currentState = SkillState.Locked;
    private float currentTimer = 0f;

    public static ShieldSkill Instance;

    private void Awake()
    {
        Instance = this;
        if (shieldVisual != null) shieldVisual.SetActive(false);
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
                if (currentTimer <= blinkThreshold)
                {
                    bool isVisible = (int)(currentTimer * blinkSpeed) % 2 == 0;

                    if (shieldVisual != null) shieldVisual.SetActive(isVisible);
                }
                else
                {
                    if (shieldVisual != null && !shieldVisual.activeSelf)
                        shieldVisual.SetActive(true);
                }

                if (currentTimer <= 0)
                {
                    DeactivateShield(false);
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

        if (shieldVisual != null) shieldVisual.SetActive(true);

    }


    private void DeactivateShield(bool broken)
    {
        currentState = SkillState.Cooldown;
        currentTimer = cooldownDuration;


        if (shieldVisual != null) shieldVisual.SetActive(false);
    }

    public bool TryBlockHit()
    {
        if (currentState == SkillState.Active)
        {
            DeactivateShield(true); 
            return true;
        }
        return false; 
    }


    public void UnlockShield(float duration)
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
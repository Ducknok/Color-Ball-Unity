using UnityEngine;

public class MagnetSkill : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float magnetRange = 10f; 
    [SerializeField] private LayerMask coinLayer;

    [Header("Skill Stats")]
    public float activeDuration = 5f; 
    public float cooldownDuration = 30f;

    private SkillState currentState = SkillState.Locked;
    private float currentTimer = 0f;

    // Singleton
    public static MagnetSkill Instance;

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
                ScanAndAttractCoins();
                if (currentTimer <= 0)
                {
                    currentState = SkillState.Cooldown;
                    currentTimer = cooldownDuration;
                    //Debug.Log($"Nam châm hết hiệu lực. Hồi chiêu trong {cooldownDuration}s...");
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
    }

    private void ScanAndAttractCoins()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.parent.position, magnetRange, coinLayer);
        foreach (var hit in hitColliders)
        {
            CoinController coin = hit.GetComponent<CoinController>();
            if (coin != null)
            {
                coin.StartMagnetized(this.transform.parent);
            }
        }
    }

    public void ActivateSkill(float duration)
    {
        activeDuration = duration;
        currentState = SkillState.Waiting;

        //Debug.Log($"Đã nâng cấp Nam châm! Thời gian hút mới: {activeDuration}s. Chờ kích hoạt...");
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

    private void OnDrawGizmosSelected()
    {
        if (currentState == SkillState.Active) Gizmos.color = Color.green;
        else if (currentState == SkillState.Cooldown || currentState == SkillState.Waiting) Gizmos.color = Color.yellow;
        else Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, magnetRange);
    }
}
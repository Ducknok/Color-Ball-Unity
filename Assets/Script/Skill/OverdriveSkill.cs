using UnityEngine;

public class OverdriveSkill : MonoBehaviour
{
    [Header("Skill Stats")]
    public float activeDuration = 6f;
    public float cooldownDuration = 30f;
    public float speedMultiplier = 2f;

    [Header("Visuals")]
    [SerializeField] private GameObject dashEffectObj;

    [Header("Blink Warning")]
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private float blinkThreshold = 2f;
    [SerializeField] private float blinkSpeed = 15f;


    private SkillState currentState = SkillState.Locked;
    private float currentTimer = 0f;

    private float originalSpeed;
    private bool isSpeedModified = false;

    // --- BIẾN MỚI ---
    private ParticleSystem[] cachedParticles;
    private Color lastKnownColor;

    public static OverdriveSkill Instance;

    private void Awake()
    {
        Instance = this;


        // Tự tìm Renderer nếu chưa kéo (Tìm cả ở con vì model thường nằm ở object con)
        if (playerRenderer == null) playerRenderer = GetComponentInChildren<Renderer>();

        if (dashEffectObj != null)
        {
            dashEffectObj.SetActive(false);
            cachedParticles = dashEffectObj.GetComponentsInChildren<ParticleSystem>(true);
        }
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
                PlayerController.Instance.forwardSpeed = originalSpeed * speedMultiplier;

                if (playerRenderer != null)
                {
                    Color currentColor = playerRenderer.material.color;

                    if (!IsColorSimilar(currentColor, lastKnownColor))
                    {
                        UpdateEffectColor(currentColor);
                    }
                }

                if (currentTimer <= blinkThreshold && playerRenderer != null)
                {
                    bool isVisible = (int)(currentTimer * blinkSpeed) % 2 == 0;
                    playerRenderer.enabled = isVisible;
                }
                else if (playerRenderer != null)
                {
                    playerRenderer.enabled = true;
                }

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

        if (!isSpeedModified)
        {
            originalSpeed = PlayerController.Instance.forwardSpeed;
            isSpeedModified = true;
        }

        if (dashEffectObj != null)
        {
            dashEffectObj.SetActive(true);

            if (playerRenderer != null)
            {
                UpdateEffectColor(playerRenderer.material.color);
            }
        }

    }
    private void UpdateEffectColor(Color color)
    {
        lastKnownColor = color;

        if (cachedParticles != null)
        {
            foreach (var ps in cachedParticles)
            {
                var main = ps.main;
                main.startColor = color;

                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
                int count = ps.GetParticles(particles);
                for (int i = 0; i < count; i++)
                {

                    float currentAlpha = particles[i].startColor.a;
                    particles[i].startColor = new Color(color.r, color.g, color.b, currentAlpha);
                }
                ps.SetParticles(particles, count);
            }
        }
    }
    private bool IsColorSimilar(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < 0.01f &&
               Mathf.Abs(a.g - b.g) < 0.01f &&
               Mathf.Abs(a.b - b.b) < 0.01f;
    }
    private void DeactivateSkill()
    {

        currentState = SkillState.Cooldown;
        currentTimer = cooldownDuration;

        if (PlayerController.Instance != null && isSpeedModified)
        {
            PlayerController.Instance.forwardSpeed = originalSpeed;
            isSpeedModified = false;
        }

        if (dashEffectObj != null) dashEffectObj.SetActive(false);
        if (playerRenderer != null) playerRenderer.enabled = true;
    }
    public void ForceStop()
    {
        if (currentState == SkillState.Active)
        {
            DeactivateSkill();
        }
    }
    public bool IsDashing()
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
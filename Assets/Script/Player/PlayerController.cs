using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDeath
{
    public static PlayerController Instance { get; private set; }
    private Camera cam;
    private Rigidbody rb;
    private PlayerInput inputActions;
    private TrailRenderer trail;
    public bool isGameActive = false;
    public bool isDead = false;

    [Header("Speed")]
    [SerializeField] public float forwardSpeed = 30f;
    [SerializeField] float speedIncreaseRate = 0.5f;
    [SerializeField] float maxSpeed = 70f;

    [SerializeField] float smoothTime = 0.02f;
    [SerializeField] float inputSensitivity = 1.5f;

    [Header("Hiệu ứng Visual")]
    [SerializeField] float tiltAngle = 30f;
    [SerializeField] float tiltSpeed = 15f;

    [Header("Endless run")]
    [SerializeField] float minX = -3f;
    [SerializeField] float maxX = 3f;

    [Header("Particle")]
    [SerializeField] Transform deathParticle;

    private Vector2 pointerScreenPos;
    private float currentVelocityX;
    private float currentRollAngle = 0f; // Biến lưu góc lăn

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        this.cam = Camera.main;
        this.rb = GetComponent<Rigidbody>();
        this.inputActions = new PlayerInput();
        this.trail = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        UpdateSensitivity();

        this.isGameActive = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        Color tmpColor = RandomColor();
        this.ChangePlayerColor(tmpColor);
        this.ChangeTrailColor(tmpColor);
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Mouse.performed += OnPointerMove;
    }

    private void OnDisable()
    {
        inputActions.Player.Mouse.performed -= OnPointerMove;
        inputActions.Player.Disable();
    }

    private void OnPointerMove(InputAction.CallbackContext ctx)
    {
        pointerScreenPos = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (!isGameActive) return;
        if (this.forwardSpeed < maxSpeed) this.forwardSpeed += speedIncreaseRate * Time.deltaTime;
        HandleMovement();
    }

    private void HandleMovement()
    {
        float screenRatio = pointerScreenPos.x / Screen.width;
        screenRatio = (screenRatio - 0.5f) * inputSensitivity + 0.5f;
        screenRatio = Mathf.Clamp01(screenRatio);

        float wantedX = Mathf.Lerp(minX, maxX, screenRatio);
        Vector3 currentPos = transform.position;

        float newX = Mathf.SmoothDamp(currentPos.x, wantedX, ref currentVelocityX, smoothTime);

        float newZ = currentPos.z + forwardSpeed * Time.deltaTime;

        transform.position = new Vector3(newX, currentPos.y, newZ);
        float deltaX = newX - currentPos.x;
        float targetRotZ = -deltaX * tiltAngle * 20f;
        targetRotZ = Mathf.Clamp(targetRotZ, -tiltAngle, tiltAngle);

        float distance = forwardSpeed * Time.deltaTime;
        currentRollAngle += (distance / Mathf.PI) * 360f; 
        currentRollAngle %= 360f;

        float currentZ = transform.rotation.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;
        float smoothedZ = Mathf.Lerp(currentZ, targetRotZ, tiltSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentRollAngle, 0, smoothedZ);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    public void OnPlayerDeath()
    {
        this.isGameActive = false;
        this.isDead = true;
        CameraShake.Instance.Shake(0.5f, 10f);
        this.SpawnDeathParticle();
        this.gameObject.SetActive(false);
        GameManager.Instance.GameOver();
    }

    private void SpawnDeathParticle()
    {
        Transform deathEffect = Instantiate(this.deathParticle, this.transform.position, Quaternion.identity);
        ParticleSystem ps = deathEffect.GetComponent<ParticleSystem>();

        if (ps != null)
        {
            Color pColor = Color.white;
            MeshRenderer playerMess = GetComponent<MeshRenderer>();
            if (playerMess != null)
            {
                pColor = playerMess.material.GetColor("_BaseColor");
            }

            var main = ps.main;
            main.startColor = pColor * 2f;

            Destroy(deathEffect.gameObject, main.duration + main.startLifetime.constantMax);
        }
    }

    public void OnObstacleDeath()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateSensitivity()
    {
        inputSensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.5f);
    }

    public void ApplySkin(SkinData skinData)
    {
        if (skinData == null || skinData.material == null) return;

        MeshRenderer playerMess = this.GetComponent<MeshRenderer>();
        if (playerMess != null)
        {

            playerMess.material = skinData.material;

            if (skinData.material.HasProperty("_BaseColor"))
            {
                Color skinColor = skinData.material.GetColor("_BaseColor");
                this.ChangeTrailColor(skinColor);
            }
        }
    }

    public void ChangePlayerColor(Color tmpColor)
    {
        MeshRenderer playerMess = this.GetComponent<MeshRenderer>();
        if (playerMess != null)
        {
            playerMess.material.SetColor("_BaseColor", tmpColor);
            playerMess.material.SetColor("_EmissionColor", tmpColor * 2f);
            playerMess.material.EnableKeyword("_EMISSION");
        }

        this.ChangeTrailColor(tmpColor);
    }

    private void ChangeTrailColor(Color tmpColor)
    {
        if (trail != null)
        {
            Gradient gradient = new Gradient();

            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(tmpColor, 0.0f);
            colorKeys[1] = new GradientColorKey(tmpColor, 1.0f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
            alphaKeys[1] = new GradientAlphaKey(0.0f, 1.0f);

            gradient.SetKeys(colorKeys, alphaKeys);
            trail.colorGradient = gradient;
        }
    }

    private string RandomColorName()
    {
        int colorIndex = Random.Range(1, 4);
        switch (colorIndex)
        {
            case 1:
                return nameof(ColorType.blue);
            case 2:
                return nameof(ColorType.red);
            case 3:
                return nameof(ColorType.green);
            default:
                break;
        }
        return "";
    }

    private Color RandomColor()
    {
        if (RandomColorName() == ColorType.blue.ToString()) return Color.blue;
        else if (RandomColorName() == ColorType.red.ToString()) return Color.red;
        else return Color.green;
    }
}
using UnityEngine;

public enum ColorType
{
    red,
    green,
    blue,
}

public class Obstacle : MonoBehaviour, IDeath
{
    private PlayerController player;
    [SerializeField] private float destroyOffset = 50f; // Giảm xuống 50 để đỡ tốn tài nguyên (1000 quá xa)
    [Header("Particle")]
    [SerializeField] private Transform successHit;

    private void Awake()
    {
        this.player = GameObject.FindAnyObjectByType<PlayerController>();
    }

    private void OnEnable()
    {
        Color tmpColor = RandomColor();
        MeshRenderer obstacleMess = GetComponent<MeshRenderer>();
        if (obstacleMess != null)
        {
            obstacleMess.material.SetColor("_BaseColor", tmpColor);
            obstacleMess.material.SetColor("_EmissionColor", tmpColor * 0.5f);
            obstacleMess.material.EnableKeyword("_EMISSION");
        }
    }

    private void Update()
    {
        if (this.player == null) return;

        float distanceZ = this.player.transform.position.z - transform.position.z;

        if (distanceZ > destroyOffset)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player == null) return;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) return;
        Color obstacleColor = meshRenderer.material.color;

        Color playerColor = player.GetComponent<MeshRenderer>().material.color;
        Color tmpColor = RandomColor();
        GameManager gameManager = GameManager.Instance;

        if (other.gameObject.name == "Player" || other.CompareTag("Player"))
        {

            bool isGhostActive = GhostSkill.Instance != null && GhostSkill.Instance.IsGhostActive();


            bool isDashing = OverdriveSkill.Instance != null && OverdriveSkill.Instance.IsDashing();

            bool isColorMatch = IsColorSimilar(obstacleColor, playerColor);

            if (isColorMatch || isGhostActive || isDashing)
            {
                this.OnObstacleDeath();

                if (gameManager != null)
                {
                    gameManager.comboTimer = 1.5f;
                    gameManager.comboIndex++;
                    this.player.ChangePlayerColor(tmpColor);
                    gameManager.PlayerPoint();
                    if (gameManager.uiInfo != null) gameManager.uiInfo.AddExperience(10f);
                }
            }
            else
            {
                if (ShieldSkill.Instance != null && ShieldSkill.Instance.TryBlockHit())
                {
                    this.OnObstacleDeath();
                    return;
                }
                player.OnPlayerDeath();
            }
        }
    }

    private bool IsColorSimilar(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < 0.01f &&
               Mathf.Abs(a.g - b.g) < 0.01f &&
               Mathf.Abs(a.b - b.b) < 0.01f;
    }

    private string RandomColorName()
    {
        int colorIndex = Random.Range(1, 4);
        switch (colorIndex)
        {
            case 1: return nameof(ColorType.blue);
            case 2: return nameof(ColorType.red);
            case 3: return nameof(ColorType.green);
            default: return "";
        }
    }

    private Color RandomColor()
    {
        string colorName = RandomColorName();
        if (colorName == ColorType.blue.ToString()) return Color.blue;
        else if (colorName == ColorType.red.ToString()) return Color.red;
        else return Color.green;
    }

    public void OnObstacleDeath()
    {
        AudioManager.Instance.PlayHit();
        if (this.successHit != null)
        {
            Transform successEffect = Instantiate(this.successHit, this.transform.position, Quaternion.LookRotation(transform.forward));
            ParticleSystem ps = successEffect.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                Renderer obstacleRend = GetComponent<Renderer>();
                if (obstacleRend != null)
                {
                    Color pColor = obstacleRend.material.GetColor("_BaseColor");
                    var main = ps.main;
                    main.startColor = pColor * 2f;
                }
                Destroy(successEffect.gameObject, 1.5f); // Hủy VFX sau 1.5s
            }
        }

        gameObject.SetActive(false);
    }

    public void OnPlayerDeath()
    {
        throw new System.NotImplementedException();
    }
}
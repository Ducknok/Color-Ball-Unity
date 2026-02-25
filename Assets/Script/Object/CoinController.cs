using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour
{
    [Header("Movement & Magnet")]
    [SerializeField] private float flySpeed = 40f;
    [SerializeField] private int coinValue = 1;

    [Header("Visuals")]
    [SerializeField] private GameObject pickupVFX;
    [SerializeField] private float flyHeight = 2.5f;
    [SerializeField] private float flyDuration = 0.4f;

    // Thêm khoảng cách tự hủy để trả về Pool nếu người chơi bỏ qua
    [SerializeField] private float destroyOffset = 50f;

    private Transform targetPlayer;
    private bool isMagnetized = false;
    private bool isCollected = false;
    private Transform playerTransform; // Để tính khoảng cách
    private Vector3 originalScale; // Lưu kích thước gốc để reset

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        // Lưu lại kích thước ban đầu (1,1,1)
        originalScale = transform.localScale;
    }

    // Hàm này chạy mỗi khi được lấy ra từ Pool
    private void OnEnable()
    {
        isMagnetized = false;
        isCollected = false;
        targetPlayer = null;

        // --- QUAN TRỌNG: Reset lại trạng thái ---
        transform.localScale = originalScale; // Trả lại kích thước to (vì lúc ăn bị thu nhỏ)

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true; // Bật lại Collider để ăn được tiếp

        // Reset vật lý nếu có
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public void StartMagnetized(Transform playerTransform)
    {
        if (isMagnetized || isCollected) return;

        isMagnetized = true;
        targetPlayer = playerTransform;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void Update()
    {
        if (isCollected) return;


        // --- QUAN TRỌNG: Tự trả về Pool nếu bị bỏ lại quá xa ---
        if (playerTransform != null && !isMagnetized)
        {
            if (playerTransform.position.z - transform.position.z > destroyOffset)
            {
                gameObject.SetActive(false); // Trả về Pool
                return;
            }
        }

        if (isMagnetized && targetPlayer != null)
        {
            float step = flySpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, step);

            if (Vector3.Distance(transform.position, targetPlayer.position) < 0.5f)
            {
                CollectCoin(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player") || other.name == "Player")
        {
            AudioManager.Instance.PlayCoin();
            CollectCoin(false);
        }
    }

    private void CollectCoin(bool instantCollect)
    {
        isCollected = true;

        if (GameManager.Instance != null && GameManager.Instance.uiInfo != null)
        {
            GameManager.Instance.uiInfo.AddCoin(coinValue);
        }

        if (pickupVFX != null)
        {
            Instantiate(pickupVFX, transform.position, Quaternion.identity);
        }

        if (instantCollect)
        {
            // SỬA: Dùng SetActive(false) thay vì Destroy
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(CollectRoutine());
        }
    }

    private IEnumerator CollectRoutine()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * flyHeight;
        Vector3 startScale = transform.localScale;

        float elapsed = 0f;
        while (elapsed < flyDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / flyDuration;

            transform.position = Vector3.Lerp(startPos, targetPos, percent);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, percent);

            yield return null;
        }
        gameObject.SetActive(false);
    }
}
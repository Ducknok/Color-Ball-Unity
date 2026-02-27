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

    [SerializeField] private float destroyOffset = 50f;

    private Transform targetPlayer;
    private bool isMagnetized = false;
    private bool isCollected = false;
    private Transform playerTransform; 
    private Vector3 originalScale; 

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;


        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        this.ResetCoin();
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


        if (playerTransform != null && !isMagnetized)
        {
            if (playerTransform.position.z - transform.position.z > destroyOffset)
            {
                ObjectPooler.Instance.ReturnToPool(gameObject);
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
            ObjectPooler.Instance.ReturnToPool(gameObject);
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
        ObjectPooler.Instance.ReturnToPool(gameObject);
    }

    public void ResetCoin()
    {
        isMagnetized = false;
        isCollected = false;
        targetPlayer = null;

        transform.localScale = originalScale;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = true;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = true;
        }
    }
}
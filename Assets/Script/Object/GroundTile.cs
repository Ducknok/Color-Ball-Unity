using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundTile : MonoBehaviour
{
    private GroundSpawner groundSpawner;

    [Header("Spawn Obstacle")]
    [SerializeField] private string obstacle;
    [SerializeField] private GameObject[] obstacleSpawnPoints;

    [Header("Spawn Coin")]
    [SerializeField] private string coin;
    [SerializeField] private GameObject[] coinSpawnPoints;

    [Range(0f, 1f)]
    [SerializeField] private float coinSpawnChance = 0.3f;

    // Lưu lại object đã spawn để clear khi tile reuse
    private List<GameObject> spawnedObjects = new List<GameObject>();

    // Lưu vị trí obstacle để tránh coin spawn trùng
    private Transform lastObstaclePoint;

    private void Awake()
    {
        groundSpawner = GameObject.FindFirstObjectByType<GroundSpawner>();
    }

    public void ResetGround()
    {
        ClearTile(); // 👈 Quan trọng: dọn tile trước khi spawn lại
        StartCoroutine(WaitAndSpawn());
    }

    private IEnumerator WaitAndSpawn()
    {
        yield return null;

        SpawnObstacles();
        SpawnCoins();
    }

    private void OnTriggerExit(Collider other)
    {
        ObjectPooler.Instance.ReturnToPool(gameObject);

        if (groundSpawner != null)
            groundSpawner.SpawnTile();
    }


    private void ClearTile()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
                ObjectPooler.Instance.ReturnToPool(obj);
        }

        spawnedObjects.Clear();
    }

    private void SpawnObstacles()
    {
        lastObstaclePoint = ChooseObstaclePoint();

        if (lastObstaclePoint != null)
            SpawnObstacle(lastObstaclePoint);
    }

    private Transform ChooseObstaclePoint()
    {
        if (obstacleSpawnPoints == null || obstacleSpawnPoints.Length == 0)
            return null;

        int index = Random.Range(0, obstacleSpawnPoints.Length);
        return obstacleSpawnPoints[index].transform;
    }

    private void SpawnObstacle(Transform point)
    {
        if (ObjectPooler.Instance == null) return;

        GameObject obj = ObjectPooler.Instance.SpawnFromPool(obstacle, point.position, point.rotation);

        if (obj != null)
            spawnedObjects.Add(obj);
    }


    private void SpawnCoins()
    {
        if (Random.value >= coinSpawnChance)
            return;

        Transform point = ChooseCoinPoint();
        if (point == null) return;

        // Không cho spawn trùng vị trí obstacle
        if (lastObstaclePoint != null &&
            Vector3.Distance(point.position, lastObstaclePoint.position) < 0.1f)
        {
            return;
        }

        SpawnCoin(point);
    }

    private Transform ChooseCoinPoint()
    {
        if (coinSpawnPoints == null || coinSpawnPoints.Length == 0)
            return null;

        int index = Random.Range(0, coinSpawnPoints.Length);
        return coinSpawnPoints[index].transform;
    }

    private void SpawnCoin(Transform point)
    {
        if (ObjectPooler.Instance == null) return;

        GameObject obj = ObjectPooler.Instance.SpawnFromPool(coin, point.position, point.rotation);

        if (obj != null)
            spawnedObjects.Add(obj);
    }
}
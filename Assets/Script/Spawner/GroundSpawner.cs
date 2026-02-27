using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    [Header("Settings")]
    public string groundTile = "Ground";


    private Vector3 nextSpawnPoint;

    private void Start()
    {
        int spawnCount = 10;

        if (ObjectPooler.Instance != null)
        {
            ObjectPooler.Pool groundPool =
                ObjectPooler.Instance.pools.Find(p => p.tag == groundTile);

            if (groundPool != null)
                spawnCount = groundPool.size;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnTile(false);
        }
    }


    public void SpawnTile(bool spawnContent = true)
    {
        if (ObjectPooler.Instance == null) return;

        GameObject newPrefab = ObjectPooler.Instance.SpawnFromPool(groundTile, nextSpawnPoint, Quaternion.identity);

        if (newPrefab != null)
        {
            if (spawnContent)
                newPrefab.GetComponent<GroundTile>().ResetGround();

            Transform endPoint = newPrefab.transform.Find("NextSpawnPoint");

            if (endPoint != null)
                nextSpawnPoint = endPoint.position;
            else
                nextSpawnPoint.z += 100f;
        }
    }
}
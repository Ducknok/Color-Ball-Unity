using UnityEngine;


public class GroundTile : MonoBehaviour
{
    GroundSpawner groundSpawner;
    

    [Header("Spawn Obstacle")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject[] obstacleSpawnPoints;

    [Header("Spawn Coint")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject[] coinSpawnPoints;
    // Tỷ lệ xuất hiện vàng (0.0 = 0%, 1.0 = 100%). Mặc định để 0.3 (30%)
    [Range(0f, 1f)]
    [SerializeField] private float coinSpawnChance = 0.3f;


    private void Start()
    {
        this.groundSpawner = GameObject.FindFirstObjectByType<GroundSpawner>();
        this.SpawnObstacles();
        this.SpawnCoins();
    }

    private void OnTriggerExit(Collider other)
    {
        this.groundSpawner.SpawnTile();
        Destroy(gameObject, 2);
    }

    /*-------------------SPAWN OBSTACLES------------------*/
    private void SpawnObstacles()
    {
        Transform point = ChoosePoint();
        this.SpawnObstacle(point);
    }
    private Transform ChoosePoint()
    {
        //Choose a random point to spawn the obstacle
        int obstacleSpawnIndex = Random.Range(0, this.obstacleSpawnPoints.Length);
        Transform point = this.obstacleSpawnPoints[obstacleSpawnIndex].transform;
        return point;
    }
    private void SpawnObstacle(Transform point)
    {
        //Spawn the obstacle at the position
        Instantiate(this.obstaclePrefab, point.position, Quaternion.identity, transform);

    }
    /*-------------------SPAWN COIN------------------*/
    private void SpawnCoins()
    {

        if (Random.value < coinSpawnChance)
        {
            Transform point = ChooseCoinPoint();
            this.SpawnCoin(point);
        }
    }
    private Transform ChooseCoinPoint()
    {
        //Choose a random point to spawn the obstacle
        int coinSpawnIndex = Random.Range(0, this.coinSpawnPoints.Length);
        Transform point = this.coinSpawnPoints[coinSpawnIndex].transform;
        return point;

    }
    private void SpawnCoin(Transform point)
    {
        //Spawn the obstacle at the position
        Instantiate(this.coinPrefab, point.position, Quaternion.identity, transform);

    }

}

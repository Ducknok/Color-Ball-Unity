using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public GameObject groundTile;
    private Vector3 nextSpawnPoint;

    public void SpawnTile()
    {
        GameObject newPrefab = Instantiate(this.groundTile, nextSpawnPoint,Quaternion.identity, this.transform);
        this.nextSpawnPoint = newPrefab.transform.GetChild(1).transform.position;
    }
    private void Start()
    {
        for(int i = 0; i < 15; i++)
        {
            SpawnTile();

        }
    }
}

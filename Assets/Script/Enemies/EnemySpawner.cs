using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Tile spawnTile;
    [SerializeField] GameObject enemyTarget;
    [SerializeField] private Enemy enemy;

    private void OnEnable()
    {
        GridManager.instance.OnStartingTileDecided += SetSpawnTile;
        GridManager.instance.OnPathGenerated += StartSpawn;
    }

    private void OnDisable()
    {
        GridManager.instance.OnStartingTileDecided -= SetSpawnTile;
        GridManager.instance.OnPathGenerated -= StartSpawn;
    }

    private void SetSpawnTile(Tile tile)
    {
        spawnTile = tile;
        transform.position = spawnTile.transform.position;
    }

    private void StartSpawn()
    {
        Instantiate(enemy, SetEnemySpawnPosition(), Quaternion.identity);
    }

    private Vector3 SetEnemySpawnPosition()
    {
        float enemyHalfHeight = enemy.GetComponent<MeshRenderer>().bounds.extents.y;
        Vector3 spawnPosition = new Vector3(transform.position.x, 0.6f, transform.position.z);

        return spawnPosition;
    }
}

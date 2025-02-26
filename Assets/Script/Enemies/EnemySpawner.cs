using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Tile spawnTile;
    [SerializeField] private float delayBetweenEnemies;
    private float timeSinceLastEnemy;
    [Space]
    [SerializeField] GameObject enemyTarget;
    [Space]
    [SerializeField] private List<Enemy> spawnableEnemies;
    private List<Enemy> enemiesToSpawn = new();
    [SerializeField] private Enemy enemy;

    private int currentWave = 0;

    private bool canSpawnEnemies = false;

    private void OnEnable()
    {
        GameManager.OnGameStateChange += GetEnemiesToSpawn;
        GameManager.OnGameStateChange += UpdateCanSpawnEnemies;

        GridManager.instance.OnStartingTileDecided += SetSpawnTile;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= GetEnemiesToSpawn;
        GameManager.OnGameStateChange -= UpdateCanSpawnEnemies;

        GridManager.instance.OnStartingTileDecided -= SetSpawnTile;
    }

    private void Update()
    {
        if (canSpawnEnemies)
            StartSpawn();
    }

    private void UpdateCanSpawnEnemies(GameState gameState)
    {
        canSpawnEnemies = gameState switch
        {
            GameState.Game => true,
            _ => false
        };
    }

    #region GetEnemiesToSpawn

    /// <summary>
    /// Get the credit the wave have for get the enemies it will spawn
    /// </summary>
    private int WaveCredit()
    {
        currentWave++;
        int waveCredit = (int)(currentWave * Mathf.Pow(2, Mathf.Log(2)));
        return waveCredit;
    }

    /// <summary>
    /// Get a list of all the enemies the wave will spawn
    /// </summary>
    private void GetEnemiesToSpawn(GameState gameState)
    {
        if (gameState != GameState.Shop) 
            return;

        int WaveCost = WaveCredit();

        List<Enemy> enemies = new(spawnableEnemies);

        if (enemies.Count == 0)
            Debug.LogError("No enemies in spawnableEnemies", this);


        //Get the enemies that will be spawned
        while (WaveCost > 0)
        {
            int randomEnemyId = Random.Range(0, enemies.Count);

            Enemy enemyToAdd = enemies[randomEnemyId];

            if (enemyToAdd == null)
            {
                Debug.LogError("enemy to add is null", this);
                break;
            }

            int enemyCost = enemyToAdd.enemySo.waveCost;

            if (WaveCost - enemyCost > 0)
                enemiesToSpawn.Add(enemyToAdd);
            else if (WaveCost - enemyCost == 0)
            {
                enemiesToSpawn.Add(enemyToAdd);
                break;
            }
            else
            {
                enemies.RemoveAt(randomEnemyId);
            }

            if (enemies.Count == 0)
                break;
        }
    }
    #endregion

    #region Spawner position
    private void SetSpawnTile(Tile tile)
    {
        spawnTile = tile;
        transform.position = spawnTile.transform.position;
    }

    private Vector3 SetEnemySpawnPosition()
    {
        float enemyHalfHeight = enemy.GetComponent<MeshRenderer>().bounds.extents.y;
        Vector3 spawnPosition = new Vector3(transform.position.x, 0.6f, transform.position.z);

        return spawnPosition;
    }
    #endregion

    #region SpawnEnemies
    private void StartSpawn()
    {
        //Get the list of enemies to spawn
        //spawn the enemies at a constant interval
        //once there is no more enemies to spawn end the wave

        if (timeSinceLastEnemy >= delayBetweenEnemies)
        {
            Vector3 spawnPosition = SetEnemySpawnPosition();

            Enemy enemy = enemiesToSpawn[0];

            Enemy enemyToSpawn = ObjectPool.GetInactive<Enemy>(enemy, spawnPosition, Quaternion.identity);

            enemiesToSpawn.Remove(enemy);

            timeSinceLastEnemy = 0;

            if (enemiesToSpawn.Count == 0)
                GameManager.UpdateGameState(GameState.Shop);
        }
        else
        {
            timeSinceLastEnemy += Time.deltaTime;
        }
    }
    #endregion
}

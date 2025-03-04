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

    private List<Enemy> enemiesInWave = new();

    private int currentWave = 0;

    private bool canSpawnEnemies = false;

    private void OnEnable()
    {
        GameManager.OnGameStateChange += GetEnemiesToSpawn;
        GameManager.OnGameStateChange += UpdateCanSpawnEnemies;

        GridManager.instance.OnStartingTileDecided += SetSpawnTile;

        EnemyEvent.OnDeath += RemoveEnemyId;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= GetEnemiesToSpawn;
        GameManager.OnGameStateChange -= UpdateCanSpawnEnemies;

        GridManager.instance.OnStartingTileDecided -= SetSpawnTile;

        EnemyEvent.OnDeath -= RemoveEnemyId;
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

    private void RemoveEnemyId(Enemy killedEnemy)
    {
        if (enemiesInWave.Contains(killedEnemy))
            enemiesInWave.Remove(killedEnemy);

        if (enemiesInWave.Count == 0 && enemiesToSpawn.Count == 0)
            GameManager.UpdateGameState(GameState.Shop);
    }

    #region GetEnemiesToSpawn

    /// <summary>
    /// Get the credit the wave have for get the enemies it will spawn
    /// </summary>
    private int WaveCredit()
    {
        currentWave++;
        int waveCredit = (int)(currentWave * Mathf.Pow(1.5f, Mathf.Log(2)));
        return waveCredit;
    }

    /// <summary>
    /// Get a list of all the enemies the wave will spawn
    /// </summary>
    private void GetEnemiesToSpawn(GameState gameState)
    {
        if (gameState != GameState.Shop) 
            return;

        enemiesToSpawn.Clear();
        List<Enemy> enemies = new(spawnableEnemies);

        timeSinceLastEnemy = delayBetweenEnemies;

        int WaveCredit = this.WaveCredit();

        if (enemies.Count == 0)
            Debug.LogError("No enemies in spawnableEnemies", this);

        //Raise out of memory exception

        //Get the enemies that will be spawned
        while (WaveCredit > 0)
        {
            int randomEnemyId = Random.Range(0, enemies.Count);

            Enemy enemyToAdd = enemies[randomEnemyId];

            int enemyCost = enemyToAdd.enemySo.waveCost;

            if (WaveCredit - enemyCost >= 0)
            {
                enemiesToSpawn.Add(enemyToAdd);
                WaveCredit -= enemyCost;
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
    #endregion

    #region SpawnEnemies
    private void StartSpawn()
    {
        //Get the list of enemies to spawn
        //spawn the enemies at a constant interval
        //once there is no more enemies to spawn end the wave

        if (timeSinceLastEnemy >= delayBetweenEnemies)
        {
            if (enemiesToSpawn.Count == 0)
                return;

            Enemy enemy = enemiesToSpawn[0];
            Vector3 spawnPosition = transform.position;
            spawnPosition.y = 0.6f;

            Enemy enemyToSpawn = ObjectPool.GetObject(enemy, spawnPosition, Quaternion.identity);

            enemiesInWave.Add(enemyToSpawn);

            enemiesToSpawn.Remove(enemy);

            timeSinceLastEnemy = 0;
        }
        else
        {
            timeSinceLastEnemy += Time.deltaTime;
        }
    }
    #endregion
}

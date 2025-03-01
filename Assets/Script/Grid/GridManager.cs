using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance { get; private set; }

    [Header("Grid")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private int gridLenght;
    [SerializeField] private int gridWidth;

    [Header("Tile")]
    [SerializeField] private Tile baseTile;
    private Tile startTile;

    [Header("Grid Color")]
    [SerializeField] private Material startMaterial;
    [SerializeField] private Material gridMaterial;
    [SerializeField] private Material pathMaterial;

    [Header("Target")]
    [SerializeField] private GameObject enemyTarget;

    private List<Tile> startingTiles = new();
    private List<Tile> endingTiles = new();
    public List<Tile> tiles { get; private set; } = new();

    public List<Tile> path { get; private set; } = new();

    private Bounds meshBounds;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshBounds = baseTile.GetComponent<MeshRenderer>().bounds;
        gridParent.transform.position = Vector3.zero;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChange += SpawnGrid;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= SpawnGrid;
    }

    #region Grid spawn
    private void SpawnGrid(GameState gameState)
    {
        if (gameState != GameState.GeneratingPath)
            return;

        ResetGrid();

        Vector3 displacement = new Vector3(gridLenght, 0, gridWidth);

        Vector3 basePosition = transform.position - Vector3.Scale(meshBounds.extents, displacement);
        for (int column = 0; column < gridLenght; column++)
        {
            for (int row = 0; row < gridWidth; row++)
            {
                Vector3 offset = Vector3.Scale(meshBounds.size, new Vector3(column, 0, row));

                Vector3 position = basePosition + offset;

                Tile tileToSpawn = ObjectPool.GetObject<Tile>(baseTile, position, Quaternion.identity);

                if (row == 0)
                {
                    startingTiles.Add(tileToSpawn);
                    tileToSpawn.isAvailable = false;
                }
                if (row == gridLenght - 1)
                    endingTiles.Add(tileToSpawn);

                tileToSpawn.SetId(row, column);
                tileToSpawn.GetComponent<MeshRenderer>().material = gridMaterial;
                tileToSpawn.name = $"{row}, {column}";
                tiles.Add(tileToSpawn);
                tileToSpawn.transform.SetParent(gridParent, true);
            }
        }

        GetStartPosition();
    }

    public event Action<Tile> OnStartingTileDecided;
    private void GetStartPosition()
    {
        int randomId = UnityEngine.Random.Range(0, startingTiles.Count);

        startTile = startingTiles[randomId];
        OnStartingTileDecided?.Invoke(startTile);
        FindPath();
    }

    private void ResetGrid()
    {
        tiles.Clear();
        startingTiles.Clear();
        endingTiles.Clear();

        ObjectPool.SetObjectInactive<Tile>(baseTile);
    }
    #endregion

    #region next tile
    /// <summary>
    /// Get the available tiles adjacent of the current one
    /// </summary>
    /// <param name="currentTile">The last tile of the path</param>
    /// <returns>A list of the available tiles</returns>
    private List<Tile> GetAvailableAdjacentTiles(Tile currentTile)
    {
        //Get the adjacent tiles of the current tile excluding those that aren't available
        List<Tile> adjacentTiles = tiles.FindAll(tile =>
        {
            //check if the tile yId is above or below the current tile by 1 on an axis and if the other id the same for the other axis
            bool isAdjacentOnY = (tile.yId == currentTile.yId + 1 || tile.yId == currentTile.yId - 1) && tile.xId == currentTile.xId;

            bool isAbjacentOnX = (tile.xId == currentTile.xId + 1 || tile.xId == currentTile.xId - 1) && tile.yId == currentTile.yId;

            return (isAdjacentOnY || isAbjacentOnX) && tile.isAvailable;
        });

        return adjacentTiles;
    }

    /// <summary>
    /// Randomly pick the next tile of the path finder in the available adjacent tiles
    /// </summary>
    /// <param name="adjacentTiles">list od the adjacent tiles</param>
    /// <returns>the next tile of the path</returns>
    private Tile GetNextTile(List<Tile> adjacentTiles)
    {
        int nextTileRandomId = UnityEngine.Random.Range(0, adjacentTiles.Count);

        Tile nextTile = adjacentTiles[nextTileRandomId];

        nextTile.isAvailable = false;

        path.Add(nextTile);

        return nextTile;
    }
    #endregion

    #region Tile availability
    /// <summary>
    /// Control the availability of the adjacent tiles of the current tile
    /// </summary>
    /// <param name="currentTile">The last tile of the path</param>
    private void SetAdjacentTileAvailability(Tile currentTile)
    {
        List<Tile> adjacentTiles = GetAvailableAdjacentTiles(currentTile);
        //For each of the adjacentTile check if they will border more than 2 tiles
        //if they are set their availability to true, set it to false otherwise
        foreach (Tile tile in adjacentTiles)
        {
            //Get all the adjacent tiles from the current tile
            //check their adjacent tile for any tiles marked as road
            int roadTileCount = tiles.FindAll(adjacentRoad =>
            {
                //Check the border of the current Tile on y.
                bool borderOnY = tile.yId == adjacentRoad.yId + 1 || tile.yId == adjacentRoad.yId - 1;
                bool hasSameX = tile.xId == adjacentRoad.xId;
                bool isAdjacentOnY = borderOnY && hasSameX;

                //Check the border of the current Tile on x.
                bool borderOnX = tile.xId == adjacentRoad.xId + 1 || tile.xId == adjacentRoad.xId - 1;
                bool hasSameY = tile.yId == adjacentRoad.yId;
                bool isAbjacentOnX = borderOnX && hasSameY;

                
                return (isAbjacentOnX || isAdjacentOnY) && adjacentRoad.isRoad;
            }).Count;

            if (roadTileCount > 1)
            {
                tile.isAvailable = false;
            }
            else
                tile.isAvailable = true;
        }
    }

    /// <summary>
    /// Go back to the previous tile in path if the pathFinder is stuck
    /// </summary>
    /// <param name="currentTile">The last tile of the path, the one we'er stuck in</param>
    /// <returns>The previous tile of the path</returns>
    private Tile RollBackToLastTile(Tile currentTile)
    {
        currentTile.isRoad = false;
        currentTile.isAvailable = false;
        currentTile.SetRendererMaterial(gridMaterial);

        int newCurrentTileId = path.Count - 2;
        currentTile = path[newCurrentTileId];

        path.RemoveAt(path.Count - 1);

        return currentTile;
    }
    #endregion

    #region path finder
    /// <summary>
    /// Create the path the enemy will go through
    /// </summary>
    private void FindPath()
    {
        path.Clear();

        StartCoroutine(PathGenerationVisual());
    }

    private IEnumerator PathGenerationVisual()
    {
        Tile currentTile = startTile;
        currentTile.SetRendererMaterial(startMaterial);
        currentTile.isRoad = true;

        path.Add(currentTile);

        while (true)
        {
            SetAdjacentTileAvailability(currentTile);
            List<Tile> adjacentTiles = GetAvailableAdjacentTiles(currentTile);

            if (adjacentTiles.Count == 0)
            {
                currentTile = RollBackToLastTile(currentTile);

                adjacentTiles = GetAvailableAdjacentTiles(currentTile);

                yield return new WaitForSeconds(0.1f);

                if (adjacentTiles.Count == 0)
                    continue;
            }

            currentTile = GetNextTile(adjacentTiles);
            currentTile.isRoad = true;

            currentTile.SetRendererMaterial(pathMaterial);

            yield return new WaitForSeconds(0.1f);

            if (endingTiles.Contains(currentTile))
            {
                enemyTarget.transform.position = currentTile.transform.position;
                break;
            }
        }

        GameManager.UpdateGameState(GameState.Shop);
    }
    #endregion

    /// <summary>
    /// Get the tiles close to the selected tile and check if there is any that are road
    /// </summary>
    /// <param name="tileClicked">the tile that was click on</param>
    /// <returns>true if the tile is near a road</returns>
    public bool CanTileHoldTurret(Tile tileClicked)
    {
        List<Tile> turretAdjacentTiles = GetAllAdjacentTiles(tileClicked);

        bool? isAdjacentToRoad = null;

        foreach(Tile tile in turretAdjacentTiles)
        {
            if (tile.isRoad)
                isAdjacentToRoad = true;
        }

        return isAdjacentToRoad.HasValue ? isAdjacentToRoad.Value : false;
    }

    /// <summary>
    /// Get the adjacent tiles
    /// </summary>
    /// <param name="tileToCheck">the tile to get the adjacent ones</param>
    /// <returns>a list of the adjacent tiles</returns>
    private List<Tile> GetAllAdjacentTiles(Tile tileToCheck)
    {
        List<Tile> allAdjacentTiles = path.FindAll(tile =>
        {
            //check if the tile Id is above or below the current tile by 1 on an axis and if the other id the same for the other axis
            bool isAdjacentOnY = (tile.yId == tileToCheck.yId + 1 || tile.yId == tileToCheck.yId - 1) && tile.xId == tileToCheck.xId;

            bool isAdjacentOnX = (tile.xId == tileToCheck.xId + 1 || tile.xId == tileToCheck.xId - 1) && tile.yId == tileToCheck.yId;

            return isAdjacentOnX || isAdjacentOnY;
        });

        return allAdjacentTiles;
    }
}

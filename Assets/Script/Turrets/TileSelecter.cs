using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class TileSelecter : MonoBehaviour
{
    private InputSystem_Actions inputs;

    private new Camera camera;
    [SerializeField] BaseTurrets selectedTurret;

    private bool canPlaceTurret;

    private void Awake()
    {
        inputs = new();
    }

    private void Start()
    {
        camera = Camera.main;
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.LeftMouseClick.performed += GetSelectedTilePosition;

        GameManager.OnGameStateChange += UpdateCanPlaceTurret;
        BetweenWaveEvent.OnTurretSelection += UpdateCanPlaceTurret;

        SelectorEvent.OnTurretToPlaceChanged += UpdateSelectedTurret;
    }

    private void OnDisable()
    {
        inputs.Disable();
        inputs.Player.LeftMouseClick.performed -= GetSelectedTilePosition;

        GameManager.OnGameStateChange -= UpdateCanPlaceTurret;
        BetweenWaveEvent.OnTurretSelection -= UpdateCanPlaceTurret;

        SelectorEvent.OnTurretToPlaceChanged -= UpdateSelectedTurret;
    }

    private void GetSelectedTilePosition(InputAction.CallbackContext context)
    {
        if (!canPlaceTurret)
            return;

        if (!IsMouseOverTile())
            return;

        Tile closestTile = SelectedTile();
        
        if (closestTile.isRoad)
            return;

        if (!GridManager.instance.CanTileHoldTurret(closestTile))
            return;

        if (!closestTile.isTurretPoint)
            PlaceNewTurret(closestTile);
        else UpgradeTurret(closestTile);
    }

    #region tiles selection
    private Tile SelectedTile()
    {
        Tile closestTile = GridManager.instance.tiles.OrderBy(tile =>
        {
            Vector3 tilePositionOnScreen = camera.WorldToScreenPoint(tile.transform.position);

            Vector3 mousePositionOnScreen = Mouse.current.position.ReadValue();

            return Vector3.Distance(tilePositionOnScreen, mousePositionOnScreen);
        }).FirstOrDefault();

        return closestTile;
    }

    private bool IsMouseOverTile()
    {
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        return Physics.Raycast(ray, out RaycastHit hit);
    }
    #endregion

    #region turret
    private void UpdateSelectedTurret(BaseTurrets newSelected)
    {
        selectedTurret = newSelected;
    }

    private void PlaceNewTurret(Tile tile)
    {
        float turretHeight = selectedTurret.GetComponent<MeshRenderer>().bounds.extents.y;

        Vector3 position = tile.transform.position;
        position.y = turretHeight;

        BaseTurrets turretToPlace = ObjectPool.GetObject(selectedTurret, position, Quaternion.identity);

        GameManager.UseGold(turretToPlace.turretsStats[0].cost);

        tile.isTurretPoint = true;
        tile.SetTileTurret(turretToPlace);
    }

    private void UpgradeTurret(Tile tile)
    {
        SelectorEvent.LevelUpTurret(tile.turretOnTile);
    }
    #endregion

    /// <summary>
    /// update canPlaceTurret depending on the state
    /// </summary>
    private void UpdateCanPlaceTurret(GameState gameState)
    {
        canPlaceTurret = gameState switch
        {
            GameState.Shop => true,
            _ => false
        };
    }

    /// <summary>
    /// Update canPlaceTurret depending on wether or not the turret selection menu is open
    /// </summary>
    private void UpdateCanPlaceTurret(bool isAble)
    {
        canPlaceTurret = isAble;
    }
}

public static class SelectorEvent
{
    public static event Action<BaseTurrets> OnTurretToPlaceChanged;
    public static void TurretToPlaceChanged(BaseTurrets turrets) => OnTurretToPlaceChanged?.Invoke(turrets);

    public static event Action<BaseTurrets> OnLevelUp;
    public static void LevelUpTurret(BaseTurrets TurretToUpgrade) => OnLevelUp?.Invoke(TurretToUpgrade);
}
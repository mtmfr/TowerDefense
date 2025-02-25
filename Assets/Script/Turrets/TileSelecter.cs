using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class TileSelecter : MonoBehaviour
{
    private InputSystem_Actions inputs;

    private new Camera camera;
    [SerializeField] BaseTurrets turret;
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
    }

    private void OnDisable()
    {
        inputs.Disable();
        inputs.Player.LeftMouseClick.performed -= GetSelectedTilePosition;
    }

    private void GetSelectedTilePosition(InputAction.CallbackContext context)
    {
        Tile closestTile = GridManager.instance.tiles.OrderBy(tile =>
        {
            Vector3 tilePositionOnScreen = camera.WorldToScreenPoint(tile.transform.position);

            Vector3 mousePositionOnScreen = Mouse.current.position.ReadValue();

            return Vector3.Distance(tilePositionOnScreen, mousePositionOnScreen);
        }).FirstOrDefault();

        if (!closestTile.isTurretPoint && !closestTile.isRoad)
            PlaceNewTurret(closestTile);
    }

    private void PlaceNewTurret(Tile tile)
    {
        float turretHeight = turret.GetComponent<MeshRenderer>().bounds.extents.y;

        Vector3 position = tile.transform.position;
        position.y = turretHeight;

        tile.isTurretPoint = true;
        Instantiate(turret, position, Quaternion.identity);
    }
}

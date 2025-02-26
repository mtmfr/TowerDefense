using UnityEngine;

public class Tile : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    public bool isAvailable = true;
    public bool isRoad = false;
    public bool isTurretPoint = false;

    public int xId { get; private set; }
    public int yId { get; private set; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnDisable()
    {
        isAvailable = true;
        isRoad = false;
        isTurretPoint = false;
    }

    public void SetId(int newXId, int newYId)
    {
        xId = newXId;
        yId = newYId;
    }

    public void SetRendererMaterial(Material newMaterial)
    {
        meshRenderer.material = newMaterial;
    }
}

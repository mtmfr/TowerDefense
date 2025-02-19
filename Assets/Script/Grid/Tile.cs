using UnityEngine;

public class Tile : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    public bool isAvailable = true;
    public bool isRoad = false;

    public int xId { get; private set; }
    public int yId { get; private set; }

    public void SetId(int newXId, int newYId)
    {
        xId = newXId;
        yId = newYId;
    }

    public void SetRendererMaterial(Material newMaterial)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = newMaterial;
    }
}

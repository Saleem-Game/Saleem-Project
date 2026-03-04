using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    public Transform player;
    public RectTransform mapRect;
    public RectTransform pointer;

    [Header("World Boundaries")]
    public float worldMinX;
    public float worldMaxX;
    public float worldMinZ;
    public float worldMaxZ;

    [Header("Map UI Padding (0 to 1)")]
    [Range(0, 1)] public float paddingLeft = 0.1f;
    [Range(0, 1)] public float paddingRight = 0.1f;
    [Range(0, 1)] public float paddingTop = 0.2f; 
    [Range(0, 1)] public float paddingBottom = 0.1f;

    void Update()
    {
        float percX = Mathf.InverseLerp(worldMinX, worldMaxX, player.position.x);
        float percZ = Mathf.InverseLerp(worldMinZ, worldMaxZ, player.position.z);

        float finalX = Mathf.Lerp(paddingLeft, 1 - paddingRight, percX);
        float finalZ = Mathf.Lerp(paddingBottom, 1 - paddingTop, percZ);

        float mapWidth = mapRect.rect.width;
        float mapHeight = mapRect.rect.height;

        pointer.anchoredPosition = new Vector2(finalX * mapWidth, finalZ * mapHeight);
    }
}
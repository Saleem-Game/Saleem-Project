using UnityEngine;

public class circleminimap : MonoBehaviour
{
    public Transform player;
    public RectTransform mapBackground; // The "background" in your image

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

    void Reset()
    {
      player=GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // 1. Get the percentage position of Saleem in the world
        float percX = Mathf.InverseLerp(worldMinX, worldMaxX, player.position.x);
        float percZ = Mathf.InverseLerp(worldMinZ, worldMaxZ, player.position.z);

        // 2. Apply your custom padding logic to find the "center" point on the texture
        float finalX = Mathf.Lerp(paddingLeft, 1 - paddingRight, percX);
        float finalZ = Mathf.Lerp(paddingBottom, 1 - paddingTop, percZ);

        float mapWidth = mapBackground.rect.width;
        float mapHeight = mapBackground.rect.height;

        // 3. Move the background. 
        // We subtract 0.5f because UI anchored positions are relative to the center.
        // If finalX is 0.5 (dead center), the offset is 0.
        float offsetX = (finalX - 0.5f) * mapWidth;
        float offsetZ = (finalZ - 0.5f) * mapHeight;

        // Apply negative offset to "slide" the map under the stationary pointer
        mapBackground.anchoredPosition = new Vector2(-offsetX, -offsetZ);
    }
}
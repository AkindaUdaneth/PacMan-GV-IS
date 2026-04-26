using UnityEngine;

[ExecuteAlways]
public class MapGenerator : MonoBehaviour
{
    [Header("Drag Wall_Prototype prefab here")]
    public GameObject wallPrefab;
    public float tileSize = 1f;

    private static readonly int[,] PacManMap = new int[,]
    {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,0,1,1,1,0,1,1,1,0,1,1,0,1,1,1,0,1,1,1,0,1,1,0,1},
        {1,0,1,1,0,1,1,1,0,1,1,1,0,1,1,0,1,1,1,0,1,1,1,0,1,1,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,0,1,0,1,1,1,1,0,1,1,1,1,0,1,1,1,1,0,1,0,1,1,0,1},
        {1,0,0,0,0,1,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,1,0,0,0,0,1},
        {1,1,1,1,0,1,1,1,0,0,0,0,1,1,1,1,0,0,0,0,1,1,1,0,1,1,1,1},
        {1,1,1,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,1,1,1},
        {0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0},
        {1,1,1,1,0,1,0,1,1,0,1,1,0,0,0,0,1,1,0,1,1,0,1,0,1,1,1,1},
        {1,1,1,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,1,1,1},
        {1,1,1,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,0,1,0,1,1,1,1,0,1,0,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,0,1,1,1,0,1,1,0,1,1,1,1,0,1,1,0,1,1,1,0,1,1,0,1},
        {1,0,0,1,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,1,0,0,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    };

    [ContextMenu("Generate Maze")]
    public void GenerateMaze()
    {
        // Clear existing walls first
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        int rows = PacManMap.GetLength(0);
        int cols = PacManMap.GetLength(1);
        float offsetX = -(cols * tileSize) / 2f + tileSize / 2f;
        float offsetZ = -(rows * tileSize) / 2f + tileSize / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (PacManMap[row, col] == 1)
                {
                    Vector3 pos = new Vector3(
                        offsetX + col * tileSize,
                        1f,
                        offsetZ + row * tileSize
                    );
                    GameObject wall = Instantiate(wallPrefab, pos, 
                        Quaternion.identity, transform);
                    wall.name = $"Wall_{row}_{col}";
                }
            }
        }
        Debug.Log($"Maze generated with {rows}x{cols} grid");
    }

    void Start()
    {
        if (Application.isPlaying) GenerateMaze();
    }
}
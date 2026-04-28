using UnityEngine;
using Unity.AI.Navigation;

[ExecuteAlways]
public class MapGenerator : MonoBehaviour
{
    [Header("Level Settings")]
    [Range(1, 3)]
    public int levelNumber = 1;

    [Header("Drag Wall_Prototype prefab here")]
    public GameObject wallPrefab;
    public float tileSize = 1f;

    [Header("Level Materials")]
    public Material wallMaterialL1;
    public Material wallMaterialL2;
    public Material wallMaterialL3;

    private static readonly int[,] Level1Map = new int[,]
    {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,0,1},
        {1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {0,0,0,0,0,1,0,0,1,1,1,1,0,0,1,0,0,0,0,0},
        {1,0,0,0,0,1,0,0,1,1,1,1,0,0,1,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1},
        {1,0,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    };

    private static readonly int[,] Level2Map = new int[,]
    {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,0,1,1,0,0,1,0,1,1,0,1,0,0,1,1,0,1,1,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,0,1,0,1,1,1,1,0,0,1,1,1,1,0,1,0,1,1,0,1},
        {1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1},
        {0,0,0,0,0,0,0,1,0,1,1,1,1,1,1,0,1,0,0,0,0,0,0,0},
        {1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1},
        {1,0,1,1,0,1,0,1,1,1,1,0,0,1,1,1,1,0,1,0,1,1,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,0,1,1,0,0,1,0,1,1,0,1,0,0,1,1,0,1,1,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    };

    private static readonly int[,] Level3Map = new int[,]
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

    public int[,] GetCurrentMap()
    {
        return levelNumber switch
        {
            1 => Level1Map,
            2 => Level2Map,
            _ => Level3Map,
        };
    }

    [ContextMenu("Generate Maze")]
    public void GenerateMaze()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        int[,] map = GetCurrentMap();
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        float offsetX = -(cols * tileSize) / 2f + tileSize / 2f;
        float offsetZ = -(rows * tileSize) / 2f + tileSize / 2f;

        // Pick material for this level
        Material levelMat = levelNumber == 1 ? wallMaterialL1 :
                            levelNumber == 2 ? wallMaterialL2 :
                            wallMaterialL3;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (map[row, col] == 1)
                {
                    Vector3 pos = new Vector3(
                        offsetX + col * tileSize,
                        1f,
                        offsetZ + row * tileSize
                    );
                    GameObject wall = Instantiate(
                        wallPrefab, pos, Quaternion.identity, transform);
                    wall.name = $"Wall_{row}_{col}";

                    // Apply level-specific material if assigned
                    if (levelMat != null)
                        wall.GetComponent<Renderer>().material = levelMat;
                }
            }
        }

        NavMeshSurface surface = FindFirstObjectByType<NavMeshSurface>();
        if (surface != null) surface.BuildNavMesh();

        Debug.Log($"Level {levelNumber} generated: {rows} rows x {cols} cols");
    }

    void Start()
    {
        if (Application.isPlaying) GenerateMaze();
    }
}
using UnityEngine;

public class MazeFloor : MonoBehaviour
{
    public void Resize(int cols, int rows)
    {
        transform.localScale = new Vector3(cols, 1, rows);
    }
}
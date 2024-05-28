using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public int x;
    public int y;
    public bool isObstacle;
    public TileInfo previous;
    public TileInfo() { }
    public TileInfo(int x , int y , bool isObstacle)
        {
            this.x = x;
            this.y = y; 
            this.isObstacle = isObstacle;
        }
    void Start()
    {
        if (gameObject.CompareTag("Obstacle"))
        {
            isObstacle = true;
        }
    }
}

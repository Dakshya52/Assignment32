using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private List<TileInfo> path;
    private int pathIndex;
    private bool isMoving;
    public float moveSpeed = 2f;

    void Update()
    {
        if (isMoving)
        {
            MoveAlongPath();
        }
    }

    public void SetPath(List<TileInfo> newPath)
    {
        path = newPath;
        pathIndex = 0;
        isMoving = path.Count > 0;
    }

    void MoveAlongPath()
    {
        if (pathIndex < path.Count)
        {
            TileInfo targetTile = path[pathIndex];
            Vector3 targetPosition = new Vector3(targetTile.x, transform.position.y, targetTile.y);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                pathIndex++;
                if (pathIndex >= path.Count)
                {
                    isMoving = false;
                }
            }
        }
        else
        {
            isMoving = false;
        }
    }
}

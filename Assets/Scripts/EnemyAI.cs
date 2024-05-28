using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private GridManager gridManager;
    private Transform player;
    public float moveSpeed = 2f;
    private List<Vector3Int> path;
    private int pathIndex;
    private bool isMoving;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager component not found in the scene.");
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Make sure the player GameObject is tagged as 'Player'.");
            return;
        }

        StartCoroutine(MoveTowardsPlayer());
    }

    IEnumerator MoveTowardsPlayer()
    {
        while (true)
        {
            if (!isMoving)
            {
                Vector3Int playerPosition = new Vector3Int(Mathf.RoundToInt(player.position.x), Mathf.RoundToInt(player.position.z), 0);
                Vector3Int enemyPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), 0);

                if (Vector3Int.Distance(playerPosition, enemyPosition) > 1)
                {
                    path = FindPath(enemyPosition, playerPosition);
                    if (path != null && path.Count > 1)
                    {
                        path.RemoveAt(path.Count - 1); // Remove the last step to stop near the player
                        pathIndex = 0;
                        isMoving = true;
                    }
                }
            }
            yield return new WaitForSeconds(0.5f); // Adjust delay between pathfinding updates as needed
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveAlongPath();
        }
    }

    void MoveAlongPath()
    {
        if (path != null && pathIndex < path.Count)
        {
            Vector3Int targetTile = path[pathIndex];
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

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int end)
    {
        if (gridManager.grid2 == null)
        {
            
            return null;
        }

        List<Vector3Int> unvisited = new List<Vector3Int>();
        Dictionary<Vector3Int, float> distances = new Dictionary<Vector3Int, float>();
        Dictionary<Vector3Int, Vector3Int> previous = new Dictionary<Vector3Int, Vector3Int>();

        for (int x = 0; x < gridManager.gridSize; x++)
        {
            for (int y = 0; y < gridManager.gridSize; y++)
            {
                if (!gridManager.grid2[x, y])
                {
                    Vector3Int tile = new Vector3Int(x, y, 0);
                    unvisited.Add(tile);
                    distances[tile] = Mathf.Infinity;
                }
            }
        }

        distances[start] = 0;

        while (unvisited.Count > 0)
        {
            Vector3Int currentTile = default;
            float minDistance = Mathf.Infinity;
            foreach (Vector3Int tile in unvisited)
            {
                if (distances[tile] < minDistance)
                {
                    minDistance = distances[tile];
                    currentTile = tile;
                }
            }

            if (currentTile == end)
            {
                return ReconstructPath(previous, end);
            }

            unvisited.Remove(currentTile);

            foreach (Vector3Int neighbor in GetNeighbors(currentTile))
            {
                if (!unvisited.Contains(neighbor)) continue;

                float tentativeDist = distances[currentTile] + 1; // Assuming each step has equal weight
                if (tentativeDist < distances[neighbor])
                {
                    distances[neighbor] = tentativeDist;
                    previous[neighbor] = currentTile;
                }
            }
        }

        return new List<Vector3Int>(); // Return empty if no path found
    }

    List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> previous, Vector3Int end)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int currentTile = end;

        while (previous.ContainsKey(currentTile))
        {
            path.Add(currentTile);
            currentTile = previous[currentTile];
        }
        path.Reverse();
        return path;
    }

    List<Vector3Int> GetNeighbors(Vector3Int tile)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        int[,] directions = new int[,]
        {
            { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } // Only up, right, down, and left
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newX = tile.x + directions[i, 0];
            int newY = tile.y + directions[i, 1];

            if (newX >= 0 && newY >= 0 && newX < gridManager.gridSize && newY < gridManager.gridSize)
            {
                if (!gridManager.grid2[newX, newY])
                {
                    neighbors.Add(new Vector3Int(newX, newY, 0));
                }
            }
        }

        return neighbors;
    }
}

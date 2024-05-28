using System.Collections.Generic;
using UnityEngine;

public class DijkstraPathfinding : MonoBehaviour
{
    GridManager gridManager;
    public Transform player;
    TileHover tileHover;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager component not found in the scene.");
            return;
        }

        tileHover = FindObjectOfType<TileHover>();
        if (tileHover == null)
        {
            Debug.LogError("TileHover component not found in the scene.");
            return;
        }
    }

    public List<TileInfo> FindPath(TileInfo startTileInfo, TileInfo endTileInfo)
    {
        List<TileInfo> unvisited = new List<TileInfo>();
        Dictionary<TileInfo, float> distances = new Dictionary<TileInfo, float>();

        foreach (TileInfo tileInfo in gridManager.grid)
        {
            if (!tileInfo.isObstacle)
            {
                unvisited.Add(tileInfo);
                distances[tileInfo] = Mathf.Infinity;
                tileInfo.previous = null;  // Initialize previous to null
            }
        }

        distances[startTileInfo] = 0;

        while (unvisited.Count > 0)
        {
            TileInfo currentTileInfo = null;
            foreach (TileInfo tileInfo in unvisited)
            {
                if (currentTileInfo == null || distances[tileInfo] < distances[currentTileInfo])
                {
                    currentTileInfo = tileInfo;
                }
            }

            if (currentTileInfo == endTileInfo)
            {
                return ReconstructPath(endTileInfo);
            }

            unvisited.Remove(currentTileInfo);

            foreach (TileInfo neighbor in GetNeighbors(currentTileInfo))
            {
                if (!unvisited.Contains(neighbor)) continue;

                float tentativeDist = distances[currentTileInfo] + 1; // Assuming each step has equal weight
                if (tentativeDist < distances[neighbor])
                {
                    distances[neighbor] = tentativeDist;
                    neighbor.previous = currentTileInfo;
                }
            }
        }

        return new List<TileInfo>(); // Return empty if no path found
    }

    List<TileInfo> ReconstructPath(TileInfo endTileInfo)
    {
        List<TileInfo> path = new List<TileInfo>();
        TileInfo currentTileInfo = endTileInfo;
        int safetyCounter = 0; // Added to prevent potential infinite loop

        while (currentTileInfo != null)
        {
            path.Add(currentTileInfo);
            currentTileInfo = currentTileInfo.previous;

            safetyCounter++;
            if (safetyCounter > 1000) // Adjust this threshold as needed
            {
                Debug.LogError("Safety counter triggered, possible infinite loop in path reconstruction.");
                break;
            }
        }
        path.Reverse();
        return path;
    }

    List<TileInfo> GetNeighbors(TileInfo tileInfo)
    {
        List<TileInfo> neighbors = new List<TileInfo>();

        int[,] directions = new int[,]
        {
            { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } // Only up, right, down, and left
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newX = tileInfo.x + directions[i, 0];
            int newY = tileInfo.y + directions[i, 1];

            if (newX >= 0 && newY >= 0 && newX < gridManager.gridSize && newY < gridManager.gridSize)
            {
                TileInfo neighbor = gridManager.grid[newX, newY];
                if (!neighbor.isObstacle)
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    void Update()
    {
        if (tileHover == null || gridManager == null)
        {
            return; // Exit if tileHover or gridManager is not set
        }

        if (Input.GetMouseButtonDown(0) && tileHover.currentTile != null)
        {
            TileInfo endTileInfo = tileHover.currentTile;
            Vector3 playerPosition = player.transform.position;
            int startX = Mathf.RoundToInt(playerPosition.x);
            int startY = Mathf.RoundToInt(playerPosition.z); // Adjust this if needed

            // Ensure the player's position is within the grid bounds
            if (startX >= 0 && startY >= 0 && startX < gridManager.gridSize && startY < gridManager.gridSize)
            {
                TileInfo startTileInfo = gridManager.grid[startX, startY];

                List<TileInfo> path = FindPath(startTileInfo, endTileInfo);
                if (path != null && path.Count > 0)
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.SetPath(path);
                    }
                }
                else
                {
                    Debug.Log("No valid path found.");
                }
            }
            else
            {
                Debug.LogError("Player's position is out of grid bounds.");
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int gridSize = 10;
    public TileInfo[,] grid;
    public bool[,] grid2;

    void Start()
    {
        grid = new TileInfo[gridSize, gridSize];
        grid2 = new bool[gridSize, gridSize]; // Initialize grid2
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tile.name = $"Tile_{x}_{y}";

                TileInfo tileInfo = tile.GetComponent<TileInfo>();
                if (tileInfo == null)
                {
                    tileInfo = tile.AddComponent<TileInfo>();
                }

                tileInfo.x = x;
                tileInfo.y = y;
                tileInfo.isObstacle = false;

                grid[x, y] = tileInfo;
                grid2[x, y] = false; // Initialize grid2 value
            }
        }
    }

    public void MarkTileAsObstacle(int x, int y)
    {
        if (IsWithinGridBounds(x, y))
        {
            grid[x, y].isObstacle = true;
            grid2[x, y] = true; // Also mark in grid2
        }
        else
        {
            Debug.LogWarning($"Trying to mark tile ({x}, {y}) as an obstacle, but it's out of grid bounds.");
        }
    }

    bool IsWithinGridBounds(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
    }
}

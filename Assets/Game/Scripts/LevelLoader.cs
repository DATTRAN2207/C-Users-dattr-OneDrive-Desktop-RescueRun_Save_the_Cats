using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private GameObject terrianPrefab;
    [SerializeField] private GameObject road;
    [SerializeField] private GameObject endPoint;

    [Header("Spawn Settings")]
    [SerializeField] private int startBuffer;
    [SerializeField] private int endBuffer;

    private int[,] grid;
    private int roadLength;
    private int roadWidth = 10;
    private int laneWidth = 5;

    private void Start()
    {
        LoadLevel(GameManager.Instance.playerData.level);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is not assigned.");
            return;
        }

        if (levelIndex >= levelData.levels.Count)
        {
            Debug.LogError($"Invalid level index: {levelIndex}");
            return;
        }

        var level = levelData.levels[levelIndex];
        roadLength = level.roadLength;

        grid = new int[roadWidth, roadLength];

        GetRandomPositionToEndPoint(level);

        //PlaceObstacles(level);

        SetupRoad(roadLength);

        SpawnTerrain(roadLength);
    }

    private void PlaceObstacles(LevelData.Level level)
    {
        for (int i = 0; i < level.obstacleCount; i++)
        {
            var selectedObstacle = GetRandomObstacleData(level.obstacles);

            bool placed = false;

            for (int attempt = 0; attempt < 100; attempt++)
            {
                int startX = Random.Range(0, roadWidth - selectedObstacle.sizeInGrid.x + 1);
                int startZ = Random.Range(0, roadLength - selectedObstacle.sizeInGrid.y + 1);

                if (CanPlaceObstacle(startX, startZ, selectedObstacle.sizeInGrid))
                {
                    PlaceObstacleOnGrid(startX, startZ, selectedObstacle);

                    Vector3 position = new Vector3(startX + selectedObstacle.sizeInGrid.x / 2f, 0f, startZ + selectedObstacle.sizeInGrid.y / 2f);
                    Instantiate(selectedObstacle.obstaclePrefab, position, Quaternion.identity, spawnParent);

                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                Debug.LogWarning($"Failed to place obstacle {i + 1}. Grid may be full.");
            }
        }
    }

    private bool CanPlaceObstacle(int startX, int startZ, Vector2Int size)
    {
        for (int x = startX; x < startX + size.x; x++)
        {
            for (int z = startZ; z < startZ + size.y; z++)
            {
                if (x >= roadWidth || z >= roadLength || grid[x, z] != 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void PlaceObstacleOnGrid(int startX, int startZ, LevelData.ObstacleData obstacle)
    {
        for (int x = startX; x < startX + obstacle.sizeInGrid.x; x++)
        {
            for (int z = startZ; z < startZ + obstacle.sizeInGrid.y; z++)
            {
                grid[x, z] = 2;
            }
        }
    }

    private void GetRandomPositionToEndPoint(LevelData.Level level)
    {
        int effectiveLength = roadLength - startBuffer - endBuffer;

        int segmentLength = effectiveLength / level.catCount;

        int currentZ = startBuffer;

        for (int i = 0; i < level.catCount; i++)
        {
            int currentLaneStart = Random.Range(1, roadWidth - 1);
            grid[currentLaneStart, currentZ] = 3;
            var selectedCat = GetRandomCatData(level.cats);
            Instantiate(selectedCat.catPrefab, new Vector3(currentLaneStart, 0f, currentZ), Quaternion.identity, spawnParent);
            GetLaneWidth(currentLaneStart, currentZ);
            currentZ += segmentLength;
        }
    }

    private void GetLaneWidth(int currentX, int currentZ)
    {
        if (currentX == 0)
        {
            for (int i = 0; i < laneWidth; i++)
            {
                grid[i, currentZ] = 1;
            }
        }
        else if (currentX == roadWidth)
        {
            for (int i = roadWidth - laneWidth; i < roadWidth; i++)
            {
                grid[i, currentZ] = 1;
            }
        }
        else
        {
            int leftIndex = currentX - 1;
            int rightIndex = currentX + 1;
            int markedCells = 1;

            while (markedCells < laneWidth)
            {
                if (leftIndex >= 0 && markedCells < laneWidth)
                {
                    grid[leftIndex, currentZ] = 1;
                    leftIndex--;
                    markedCells++;
                }

                if (rightIndex < roadWidth && markedCells < laneWidth)
                {
                    grid[rightIndex, currentZ] = 1;
                    rightIndex++;
                    markedCells++;
                }
            }
        }
    }

    private void SetupRoad(float roadLength)
    {
        endPoint.transform.position = new Vector3(5f, 0.5f, roadLength - 20f);
        road.transform.position = new Vector3(5f, 0f, roadLength / 2f);
        road.transform.localScale = new Vector3(10f, 0.5f, roadLength);
    }

    private void SpawnTerrain(float roadLength)
    {
        var terrainCount = Mathf.CeilToInt(roadLength / 100f);
        float terrainPos = 0f;
        for (int i = 0; i < terrainCount; i++)
        {
            var terrain = Instantiate(terrianPrefab, Vector3.zero, Quaternion.identity, spawnParent);
            terrain.transform.position = new Vector3(-55f, 0f, terrainPos);
            terrainPos += 100f;
        }
    }

    private LevelData.CatData GetRandomCatData(List<LevelData.CatData> catTypes)
    {
        int randomIndex = Random.Range(0, catTypes.Count);
        return catTypes[randomIndex];
    }

    private LevelData.ObstacleData GetRandomObstacleData(List<LevelData.ObstacleData> obstacleTypes)
    {
        int randomIndex = Random.Range(0, obstacleTypes.Count);
        return obstacleTypes[randomIndex];
    }
}
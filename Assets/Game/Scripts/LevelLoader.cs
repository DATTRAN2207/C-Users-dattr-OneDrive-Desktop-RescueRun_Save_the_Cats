using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    [SerializeField] private LevelData levelData;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private GameObject terrianPrefab;
    [SerializeField] private GameObject road;
    [SerializeField] private GameObject blockLeft;
    [SerializeField] private GameObject blockRight;
    [SerializeField] private GameObject endPoint;

    [Header("Spawn Settings")]
    [SerializeField] private int startBuffer;
    [SerializeField] private int endBuffer;

    private int[,] grid;
    private int roadLength;
    private int roadWidth = 10;
    private int laneWidth;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (GameManager.Instance == null) return;
        LoadLevel(GameManager.Instance.PlayerData.level);
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
        laneWidth = level.laneWidth;
        grid = new int[roadWidth, roadLength];

        GetRandomPositionToEndPoint(level);

        SetupRoad(roadLength);

        SpawnTerrain(roadLength);
    }

    private void GetRandomPositionToEndPoint(LevelData.Level level)
    {
        int effectiveLength = roadLength - startBuffer - endBuffer;
        int segmentLength = effectiveLength / level.ziczacCount;

        int currentZ = startBuffer;
        Vector2Int? previousCatPosition = null;

        for (int i = 0; i < level.ziczacCount; i++)
        {
            int currentLaneStart = Random.Range(1, roadWidth - 1);
            if (previousCatPosition.HasValue)
            {
                MarkLineInGrid(previousCatPosition.Value, new Vector2Int(currentLaneStart, currentZ));
                PlaceObstacles(previousCatPosition.Value, new Vector2Int(currentLaneStart, currentZ), level);
            }

            previousCatPosition = new Vector2Int(currentLaneStart, currentZ);
            currentZ += segmentLength;
        }

        for (int i = 0; i < level.catCount; i++)
        {
            RandomCatPos(level);
        }
    }

    private void RandomCatPos(LevelData.Level level)
    {
        List<Vector2Int> lanePositions = new List<Vector2Int>();

        for (int z = 0; z < roadLength; z++)
        {
            for (int x = 0; x < roadWidth; x++)
            {
                if (grid[x, z] == 1)
                {
                    lanePositions.Add(new Vector2Int(x, z));
                }
            }
        }

        if (lanePositions.Count == 0)
        {
            Debug.LogError("No valid lane positions found in the grid.");
            return;
        }

        Vector2Int randomLanePosition = lanePositions[Random.Range(0, lanePositions.Count)];

        var selectedCat = GetRandomCatData(level.cats);
        Instantiate(selectedCat.catPrefab, new Vector3(randomLanePosition.x, 0f, randomLanePosition.y), Quaternion.identity, spawnParent);

        grid[randomLanePosition.x, randomLanePosition.y] = 3;
    }

    private void PlaceObstacles(Vector2Int start, Vector2Int end, LevelData.Level level)
    {
        int minZ = Mathf.Min(start.y, end.y);
        int maxZ = Mathf.Max(start.y, end.y);

        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        for (int z = minZ; z <= maxZ - 1; z++)
        {
            for (int x = 0; x < roadWidth; x++)
            {
                if (grid[x, z] == 0)
                {
                    emptyPositions.Add(new Vector2Int(x, z));
                }
            }
        }

        GetRandomPositionsOfSizeObstacle(emptyPositions, level, level.obstacleCount);
    }

    private List<Vector2Int> GetRandomPositionsOfSizeObstacle(List<Vector2Int> emptyPositions, LevelData.Level level, int count)
    {
        List<Vector2Int> placedPositions = new List<Vector2Int>();
        int attempts = 100;

        while (placedPositions.Count < count && attempts > 0)
        {
            var obstacle = GetRandomObstacleData(level.obstacles);

            attempts--;

            int randomIndex = Random.Range(0, emptyPositions.Count);
            Vector2Int startPosition = emptyPositions[randomIndex];

            if (CanPlaceObstacle(startPosition, obstacle.sizeInGrid))
            {
                MarkObstacleOnGrid(startPosition, obstacle.sizeInGrid);
                Instantiate(obstacle.obstaclePrefab, new Vector3(startPosition.x, 0f, startPosition.y), Quaternion.identity, spawnParent);
                placedPositions.Add(startPosition);
            }
        }

        return placedPositions;
    }

    private bool CanPlaceObstacle(Vector2Int start, Vector2Int sizeInGrid)
    {
        if (start.x + sizeInGrid.x > roadWidth || start.y + sizeInGrid.y > roadLength)
            return false;

        for (int x = start.x; x < start.x + sizeInGrid.x; x++)
        {
            for (int z = start.y; z < start.y + sizeInGrid.y; z++)
            {
                if (grid[x, z] != 0)
                    return false;
            }
        }

        return true;
    }

    private void MarkObstacleOnGrid(Vector2Int start, Vector2Int sizeInGrid)
    {
        for (int x = start.x; x < start.x + sizeInGrid.x; x++)
        {
            for (int z = start.y; z < start.y + sizeInGrid.y; z++)
            {
                grid[x, z] = 2;
            }
        }
    }

    private void MarkLineInGrid(Vector2Int start, Vector2Int end)
    {
        float deltaX = end.x - start.x;
        float deltaZ = end.y - start.y;
        int minZ = Mathf.Min(start.y, end.y);
        int maxZ = Mathf.Max(start.y, end.y);

        if (deltaX == 0)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                grid[start.x, z] = 1;
                GetLaneWidth(start.x, z);
            }
            return;
        }

        float a = deltaZ / deltaX;
        float b = start.y - a * start.x;

        for (int z = minZ; z <= maxZ; z++)
        {
            float x = (z - b) / a;
            int xRounded = Mathf.RoundToInt(x);

            grid[xRounded, z] = 1;
            GetLaneWidth(xRounded, z);
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
        blockLeft.transform.position = new Vector3(blockLeft.transform.position.x, blockLeft.transform.position.y, roadLength / 2 - 10f);
        blockLeft.transform.localScale = new Vector3(blockLeft.transform.localScale.x, blockLeft.transform.localScale.y, roadLength + 20f);
        blockRight.transform.position = new Vector3(blockRight.transform.position.x, blockRight.transform.position.y, roadLength / 2 - 10f);
        blockRight.transform.localScale = new Vector3(blockRight.transform.localScale.x, blockRight.transform.localScale.y, roadLength + 20f);
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

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        for (int z = 0; z < roadLength; z++)
        {
            for (int x = 0; x < roadWidth; x++)
            {
                Vector3 position = new Vector3(x, 0, z);
                Color color;

                switch (grid[x, z])
                {
                    case 1: // Lane
                        color = Color.green;
                        break;
                    case 2: // Obstacle
                        color = Color.red;
                        break;
                    case 3: // Cat
                        color = Color.yellow;
                        break;
                    default: // Empty
                        color = Color.gray;
                        break;
                }

                DrawGridCell(position, color);
            }
        }
    }

    private void DrawGridCell(Vector3 position, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawCube(position + new Vector3(0.5f, 0, 0.5f), new Vector3(1, 0.1f, 1));
    }

    public void ClearLevel()
    {
        if (grid != null)
        {
            for (int z = 0; z < roadLength; z++)
            {
                for (int x = 0; x < roadWidth; x++)
                {
                    grid[x, z] = 0;
                }
            }
        }

        for (int i = spawnParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(spawnParent.GetChild(i).gameObject);
        }

        Debug.Log("Level cleared.");
    }

    public void ReloadLevel(int levelIndex)
    {
        ClearLevel();
        LoadLevel(levelIndex);
        Debug.Log($"Level {levelIndex} reloaded.");
    }

    public LevelData GetLevelData()
    {
        return levelData;
    }
}
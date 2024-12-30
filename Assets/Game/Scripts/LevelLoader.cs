using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private Transform spawnParent;

    [Header("Spawn Settings")]
    public float startBuffer = 20f;
    public float endBuffer = 40f;

    private void Start()
    {
        LoadLevel(0);
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
        float roadLength = level.roadLength;

        List<Vector3> allPositions = new List<Vector3>();

        SpawnCats(level.catCount, roadLength, level.cats, allPositions);

        SpawnObstacles(level.obstacleCount, roadLength, level.obstacles, allPositions);
    }

    private List<Vector3> SpawnCats(int totalCatCount, float roadLength, List<LevelData.CatData> catTypes, List<Vector3> allPositions)
    {
        if (catTypes == null || catTypes.Count == 0)
        {
            Debug.LogError("No cat types available.");
            return new List<Vector3>();
        }

        List<Vector3> catPositions = new List<Vector3>();
        float effectiveRoadLength = roadLength - startBuffer - endBuffer;
        float segmentLength = effectiveRoadLength / totalCatCount;

        for (int i = 0; i < totalCatCount; i++)
        {
            Vector3 newPosition;
            int attempts = 0;

            var selectedCat = GetRandomCatData(catTypes);

            bool positionFound = false;

            while (attempts < 100)
            {
                float zPosition = Random.Range(i * segmentLength + startBuffer, (i + 1) * segmentLength + startBuffer);
                float xPosition = Random.Range(-4f, 4f);
                newPosition = new Vector3(xPosition, 0, zPosition);
                attempts++;

                if (IsPositionValid(newPosition, catPositions, selectedCat.safeRadius) &&
                    IsPositionValid(newPosition, allPositions, selectedCat.safeRadius))
                {
                    catPositions.Add(newPosition);
                    allPositions.Add(newPosition);
                    Instantiate(selectedCat.catPrefab, newPosition, Quaternion.identity, spawnParent);
                    positionFound = true;
                    break;
                }
            }

            if (!positionFound)
            {
                Debug.LogWarning($"Failed to place cat after 100 attempts in segment {i + 1}.");
            }
        }

        return catPositions;
    }

    private void SpawnObstacles(int totalObstacleCount, float roadLength, List<LevelData.ObstacleData> obstacleTypes, List<Vector3> allPositions)
    {
        if (obstacleTypes == null || obstacleTypes.Count == 0)
        {
            Debug.LogError("No obstacle types available.");
            return;
        }

        List<Vector3> obstaclePositions = new List<Vector3>();
        float effectiveRoadLength = roadLength - startBuffer - endBuffer;
        float segmentLength = effectiveRoadLength / totalObstacleCount;

        for (int i = 0; i < totalObstacleCount; i++)
        {
            Vector3 newPosition;
            int attempts = 0;

            var selectedObstacle = GetRandomObstacleData(obstacleTypes);

            bool positionFound = false;

            while (attempts < 100)
            {
                float zPosition = Random.Range(i * segmentLength + startBuffer, (i + 1) * segmentLength + startBuffer);
                float xPosition = Random.Range(-4f, 4f);
                newPosition = new Vector3(xPosition, 0, zPosition);
                attempts++;

                if (IsPositionValid(newPosition, obstaclePositions, selectedObstacle.safeRadius) &&
                    IsPositionValid(newPosition, allPositions, selectedObstacle.safeRadius))
                {
                    obstaclePositions.Add(newPosition);
                    allPositions.Add(newPosition);
                    Instantiate(selectedObstacle.obstaclePrefab, newPosition, Quaternion.identity, spawnParent);
                    positionFound = true;
                    break;
                }
            }

            if (!positionFound)
            {
                Debug.LogWarning($"Failed to place obstacle after 100 attempts in segment {i + 1}.");
            }
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

    private bool IsPositionValid(Vector3 position, List<Vector3> existingPositions, float safeRadius)
    {
        float safeRadiusSquared = safeRadius * safeRadius;

        foreach (var existing in existingPositions)
        {
            if ((position - existing).sqrMagnitude < safeRadiusSquared)
            {
                return false;
            }
        }

        return true;
    }
}
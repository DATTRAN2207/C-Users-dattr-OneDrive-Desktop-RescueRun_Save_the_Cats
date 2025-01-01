using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game Data/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    [Serializable]
    public class CatData
    {
        public GameObject catPrefab;
        public Vector2Int sizeInGrid;
    }

    [Serializable]
    public class ObstacleData
    {
        public GameObject obstaclePrefab;
        public Vector2Int sizeInGrid;
    }

    [Serializable]
    public class Level
    {
        public int roadLength;
        public int ziczacCount;
        public int catCount;
        public int obstacleCount;

        [Header("Cat Configuration")]
        public List<CatData> cats = new List<CatData>();

        [Header("Obstacle Configuration")]
        public List<ObstacleData> obstacles = new List<ObstacleData>();
    }

    public List<Level> levels = new List<Level>();
}
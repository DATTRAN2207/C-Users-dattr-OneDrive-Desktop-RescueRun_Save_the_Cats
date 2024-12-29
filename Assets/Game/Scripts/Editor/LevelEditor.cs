using UnityEditor;
using UnityEngine;

public class LevelEditor : EditorWindow
{
    private LevelData levelData;
    private int selectedLevelIndex = -1;
    private Vector2 levelListScrollPos;
    private Vector2 levelDetailsScrollPos;

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Level Manager", EditorStyles.boldLabel);

        levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);
        if (levelData == null)
        {
            EditorGUILayout.HelpBox("Please assign a Level Data object.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        DrawLevelList();
        DrawLevelDetails();
        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(levelData);
        }
    }

    private void DrawLevelList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.LabelField("Levels", EditorStyles.boldLabel);

        levelListScrollPos = EditorGUILayout.BeginScrollView(levelListScrollPos, GUILayout.Height(300));
        for (int i = 0; i < levelData.levels.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button($"Level {i + 1}", EditorStyles.toolbarButton))
            {
                selectedLevelIndex = i;
            }

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                levelData.levels.RemoveAt(i);
                selectedLevelIndex = -1;
                break;
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add New Level"))
        {
            levelData.levels.Add(new LevelData.Level {});
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawLevelDetails()
    {
        if (selectedLevelIndex < 0 || selectedLevelIndex >= levelData.levels.Count)
        {
            EditorGUILayout.HelpBox("Select a level to view its details.", MessageType.Info);
            return;
        }

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Level Details", EditorStyles.boldLabel);

        levelDetailsScrollPos = EditorGUILayout.BeginScrollView(levelDetailsScrollPos, GUILayout.Height(300));

        var level = levelData.levels[selectedLevelIndex];
        level.roadLength = EditorGUILayout.FloatField("Road Length", level.roadLength);

        DrawCatConfiguration(level);
        DrawObstacleConfiguration(level);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawCatConfiguration(LevelData.Level level)
    {
        EditorGUILayout.LabelField("Cat Configuration", EditorStyles.boldLabel);
        level.catCount = EditorGUILayout.IntField("Cat Count", level.catCount);

        for (int i = 0; i < level.cats.Count; i++)
        {
            var cat = level.cats[i];

            EditorGUILayout.BeginHorizontal();
            cat.catPrefab = (GameObject)EditorGUILayout.ObjectField("Cat Prefab", cat.catPrefab, typeof(GameObject), false);
            cat.safeRadius = EditorGUILayout.FloatField("Safe Radius", cat.safeRadius);

            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                level.cats.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add New Cat Type"))
        {
            level.cats.Add(new LevelData.CatData { catPrefab = null, safeRadius = 5f });
        }
    }

    private void DrawObstacleConfiguration(LevelData.Level level)
    {
        EditorGUILayout.LabelField("Obstacle Configuration", EditorStyles.boldLabel);
        level.obstacleCount = EditorGUILayout.IntField("Obstacle Count", level.obstacleCount);

        for (int i = 0; i < level.obstacles.Count; i++)
        {
            var obstacle = level.obstacles[i];

            EditorGUILayout.BeginHorizontal();
            obstacle.obstaclePrefab = (GameObject)EditorGUILayout.ObjectField("Obstacle Prefab", obstacle.obstaclePrefab, typeof(GameObject), false);
            obstacle.safeRadius = EditorGUILayout.FloatField("Safe Radius", obstacle.safeRadius);

            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                level.obstacles.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add New Obstacle Type"))
        {
            level.obstacles.Add(new LevelData.ObstacleData { obstaclePrefab = null, safeRadius = 5f });
        }
    }
}
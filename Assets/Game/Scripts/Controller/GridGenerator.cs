using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private int gridWidth = 100;
    [SerializeField] private int gridLength = 400;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject catPrefab;

    private int[,] grid;

    private void Awake()
    {
        GeneratePath(1);
    }

    public void GeneratePath(int difficultyLevel)
    {
        grid = new int[gridLength, gridWidth];

        int laneWidth = difficultyLevel == 1 ? 5 : difficultyLevel == 2 ? 4 : 3;

        CreatePath(laneWidth);

        FillObstacles();

        PlaceSpecialObstacles();

        DisplayGrid();
    }

    private void CreatePath(int laneWidth)
    {
        int startLane = Random.Range(0, gridWidth - laneWidth);
        int currentLaneStart = startLane;

        for (int x = 0; x < gridLength; x++)
        {
            for (int z = currentLaneStart; z < currentLaneStart + laneWidth; z++)
            {
                grid[x, z] = 1;
            }

            if (x < gridLength - 1)
            {
                int laneShift = Random.Range(-1, 2);
                currentLaneStart = Mathf.Clamp(currentLaneStart + laneShift, 0, gridWidth - laneWidth);
            }
        }
    }

    private void FillObstacles()
    {
        for (int x = 0; x < gridLength; x++)
        {
            for (int z = 0; z < gridWidth; z++)
            {
                if (grid[x, z] == 0)
                {
                    grid[x, z] = 2;
                }
            }
        }
    }

    private void PlaceSpecialObstacles()
    {
        int specialObstacleCount = 10;

        for (int i = 0; i < specialObstacleCount; i++)
        {
            int x, z;

            do
            {
                x = Random.Range(0, gridLength);
                z = Random.Range(0, gridWidth);
            } while (grid[x, z] != 2);

            grid[x, z] = 3;

            PlaceCatNear(x, z);
        }
    }

    private void PlaceCatNear(int x, int z)
    {
        int catX, catZ;

        do
        {
            catX = x + Random.Range(-1, 2);
            catZ = z + Random.Range(-1, 2);
        } while (catX < 0 || catX >= gridLength || catZ < 0 || catZ >= gridWidth || grid[catX, catZ] != 0);

        grid[catX, catZ] = 4;
    }

    private void DisplayGrid()
    {
        for (int x = 0; x < gridLength; x++)
        {
            for (int z = 0; z < gridWidth; z++)
            {
                Vector3 position = new Vector3(x, 0, z);
                GameObject obj = Instantiate(cubePrefab, position, Quaternion.identity);

                Renderer renderer = obj.GetComponent<Renderer>();
                switch (grid[x, z])
                {
                    case 1: renderer.material.color = Color.green; break;
                    case 2: renderer.material.color = Color.red; break;
                    case 3: renderer.material.color = Color.blue; break;
                    case 4:
                        Instantiate(catPrefab, position + Vector3.up * 0.5f, Quaternion.identity);
                        renderer.material.color = Color.yellow;
                        break;
                    default: renderer.material.color = Color.white; break;
                }
            }
        }
    }
}
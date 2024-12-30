using UnityEngine;

public class InfiniteRoad : MonoBehaviour
{
    public static InfiniteRoad Instance;
    [SerializeField] private GameObject road1;
    [SerializeField] private GameObject road2;

    private GameObject currentRoad;

    private void Awake()
    {
        Instance = this;
    }

    public void SwithRoad()
    {
        if (currentRoad == road1)
        {
            currentRoad = road2;
            road1.transform.position = road2.transform.position + Vector3.forward * 100f;
        }
        else
        {
            currentRoad = road1;
            road2.transform.position = road1.transform.position + Vector3.forward * 100f;
        }
    }
}
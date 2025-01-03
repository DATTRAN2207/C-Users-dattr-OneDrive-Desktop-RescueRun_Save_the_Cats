using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private List<Transform> followPositions = new List<Transform>();
    private Dictionary<Transform, CatBehaviour> catPositions = new Dictionary<Transform, CatBehaviour>();

    private void Awake()
    {
        foreach(Transform position in followPositions)
        {
            catPositions.Add(position, null);
        }
    }

    public Transform GetNextAvailablePosition(CatBehaviour cat)
    {
        foreach (var pos in catPositions)
        {
            if (pos.Value == null)
            {
                catPositions[pos.Key] = cat;
                return pos.Key;
            }
        }
        return null;
    }

    public int GetTotalRescuedCats()
    {
        int count = 0;

        foreach (var pos in catPositions)
        {
            if (pos.Value != null)
            {
                count++;
            }
        }

        return count;
    }
}
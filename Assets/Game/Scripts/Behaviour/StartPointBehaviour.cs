using System;
using UnityEngine;

public class StartPointBehaviour : MonoBehaviour
{
    public Action StartCountDistanceTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCountDistanceTrigger?.Invoke();
        }
    }
}
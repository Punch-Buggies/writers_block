using System;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    [SerializeField] bool spawnOccupied = false;


    public void SetOccupation(bool occupied)
    {
        spawnOccupied = occupied;
    }

    public bool GetOccupation()
    {
        return spawnOccupied; 
    }

    public Transform GetTransform()
    {
        return transform;        
    }
}

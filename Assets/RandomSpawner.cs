using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public DrawGizmo gizmo;
    public List<GameObject> prefabs;
    public float minimumDistance = 0.05f; // in meters

    private List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        
    }

    public void SpawnObjects()
    {
        Vector3 boxSize = gizmo.boxSize - new Vector3(minimumDistance * 2, minimumDistance * 2, minimumDistance * 2);
        Vector3 spawnAreaMin = gizmo.boxCenter - boxSize / 2;
        Vector3 spawnAreaMax = gizmo.boxCenter + boxSize / 2;

        foreach (GameObject prefab in prefabs)
        {
            Vector3 spawnPosition = FindPosition(spawnAreaMin, spawnAreaMax);
            if (spawnPosition != Vector3.zero)
            {
                Instantiate(prefab, spawnPosition, Quaternion.identity);
                usedPositions.Add(spawnPosition);  
            }
        }
    }

    Vector3 FindPosition(Vector3 min, Vector3 max)
    {
        bool positionFound = false;
        Vector3 potentialPosition = Vector3.zero;
        int attempts = 0;

        while (!positionFound && attempts < 10000)
        {
            potentialPosition = new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y),
                Random.Range(min.z, max.z)
            );

            if (IsPositionValid(potentialPosition))
            {
                positionFound = true;
            }

            attempts++;
        }

        return positionFound ? potentialPosition : Vector3.zero;
    }

    bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Vector3.Distance(usedPosition, position) < minimumDistance)
            {
                return false;
            }
        }

        return true;
    }

    
    public List<Vector3> GetSpawnedPositions()
    {
        return new List<Vector3>(usedPositions);
    }
}

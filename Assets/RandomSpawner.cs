using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomSpawner : MonoBehaviour
{
    public DrawGizmo gizmo;
    public List<GameObject> prefabs;
    public float minimumDistance = 0.15f; // in meters
    private float distanceFromEdge = 0.00f; // 5 cm from the edge

    public Material transparentMaterial; // Reference to the material set via the Unity Editor
    private Dictionary<Vector3, string> spawnedObjects = new Dictionary<Vector3, string>();
    public Dictionary<GameObject, string> spawnedGameObjects = new Dictionary<GameObject, string>(); // Store the instantiated GameObjects

    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("Part4"))
        {
            if (transparentMaterial == null)
            {
                Debug.LogWarning("Transparent material is not assigned. Please assign it in the Unity Editor.");
            }
        }
    }

    public void LoadSpawnedObjects()
    {
        int count = PlayerPrefs.GetInt("SpawnedObjectCount", 0);
        for (int i = 0; i < count; i++)
        {
            float x = PlayerPrefs.GetFloat($"SpawnedObject_{i}_PosX");
            float y = PlayerPrefs.GetFloat($"SpawnedObject_{i}_PosY");
            float z = PlayerPrefs.GetFloat($"SpawnedObject_{i}_PosZ");
            string prefabName = PlayerPrefs.GetString($"SpawnedObject_{i}_Name");

            GameObject prefab = prefabs.Find(p => p.name == prefabName);
            if (prefab != null)
            {
                Vector3 position = new Vector3(x, y, z);
                GameObject spawnedObject = Instantiate(prefab, position, Quaternion.identity);
                spawnedObjects[position] = prefab.name;
                spawnedGameObjects[spawnedObject] = prefab.name; // Store the instantiated GameObject

                if (SceneManager.GetActiveScene().name.Equals("Part4"))
                {
                    MakeObjectTransparentAndDeactivateChildren(spawnedObject);
                }
            }
        }
    }

    public void SpawnObjects()
    {
        Vector3 boxSize = gizmo.getBoxSize();
        Vector3 spawnAreaMin = gizmo.getBoxCenter() - boxSize / 2 + new Vector3(distanceFromEdge, distanceFromEdge, distanceFromEdge);
        Vector3 spawnAreaMax = gizmo.getBoxCenter() + boxSize / 2 - new Vector3(distanceFromEdge, distanceFromEdge, distanceFromEdge);

        foreach (GameObject prefab in prefabs)
        {
            Vector3 spawnPosition = FindPosition(spawnAreaMin, spawnAreaMax);
            if (spawnPosition != Vector3.zero)
            {
                GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
                spawnedObjects[spawnPosition] = prefab.name;
                spawnedGameObjects[spawnedObject] = prefab.name; // Store the instantiated GameObject

                if (SceneManager.GetActiveScene().name.Equals("Part4"))
                {
                    MakeObjectTransparentAndDeactivateChildren(spawnedObject);
                }              
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
        foreach (Vector3 usedPosition in spawnedObjects.Keys)
        {
            if (Vector3.Distance(usedPosition, position) < minimumDistance)
            {
                return false;
            }
        }

        return true;
    }

    public void SpawnObjectsAtSpecificPositions(List<PrefabPosition> prefabPositions)
    {
        foreach (PrefabPosition prefabPosition in prefabPositions)
        {
            if (IsPositionValid(prefabPosition.position))
            {
                GameObject spawnedObject = Instantiate(prefabPosition.prefab, prefabPosition.position, Quaternion.identity);
                spawnedObjects[prefabPosition.position] = prefabPosition.prefab.name;
                spawnedGameObjects[spawnedObject] = prefabPosition.prefab.name; // Store the instantiated GameObject

                if (SceneManager.GetActiveScene().name.Equals("Part4"))
                {
                    MakeObjectTransparentAndDeactivateChildren(spawnedObject);
                }
            }
            else
            {
                Debug.LogWarning("Position " + prefabPosition.position + " is too close to another object.");
            }
        }
    }

    public void SaveSpawnedObjects()
    {
        int count = 0;
        foreach (var entry in spawnedObjects)
        {
            PlayerPrefs.SetFloat($"SpawnedObject_{count}_PosX", entry.Key.x);
            PlayerPrefs.SetFloat($"SpawnedObject_{count}_PosY", entry.Key.y);
            PlayerPrefs.SetFloat($"SpawnedObject_{count}_PosZ", entry.Key.z);
            PlayerPrefs.SetString($"SpawnedObject_{count}_Name", entry.Value);
            count++;
        }
        PlayerPrefs.SetInt("SpawnedObjectCount", count);
        PlayerPrefs.Save();
    }

    public Dictionary<Vector3, string> GetSpawnedPositionsAndNames()
    {
        return new Dictionary<Vector3, string>(spawnedObjects);
    }

    public Dictionary<GameObject, string> GetSpawnedGameObjectsAndNames()
    {
        return new Dictionary<GameObject, string>(spawnedGameObjects);
    }

    public void MakeObjectTransparentAndDeactivateChildren(GameObject obj)
    {
        if (transparentMaterial == null)
        {
            Debug.LogWarning("Transparent material is not assigned.");
            return;
        }

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = transparentMaterial;
        }

        // Deactivate all child components
        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void MakeAllSpawnedObjectsInvisible()
    {
        foreach (GameObject obj in spawnedGameObjects.Keys)
        {
            MakeObjectTransparentAndDeactivateChildren(obj);
        }
    }
}



[System.Serializable]
public class PrefabPosition
{
    public GameObject prefab;
    public Vector3 position;
}

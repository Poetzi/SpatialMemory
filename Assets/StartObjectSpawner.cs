using UnityEngine;

public class StartObjectSpawner : MonoBehaviour
{
    public GameObject cubePrefab; // Assign the cube prefab in the inspector
    private GameObject spawnedCube; // Holds the spawned cube instance

    // Method to spawn the cube
    public void SpawnCube()
    {
        if (cubePrefab == null)
        {
            Debug.LogError("Cube prefab is not assigned.");
            return;
        }

        // Instantiate the cube at the prefab's position and rotation
        if (spawnedCube == null)
        {
            spawnedCube = Instantiate(cubePrefab, cubePrefab.transform.position, cubePrefab.transform.rotation);
        }
    }

    // Method to check if given coordinates are inside the spawned cube
    public bool IsInside(Vector3 point)
    {
        if (spawnedCube != null)
        {
            Collider cubeCollider = spawnedCube.GetComponent<Collider>();
            if (cubeCollider != null)
            {
                return cubeCollider.bounds.Contains(point);
            }
            else
            {
                Debug.LogError("The cube does not have a collider component.");
            }
        }
        else
        {
            Debug.LogError("No cube has been spawned yet.");
        }
        return false;
    }
}

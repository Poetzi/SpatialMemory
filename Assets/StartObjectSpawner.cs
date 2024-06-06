using UnityEngine;

public class StartObjectSpawner : MonoBehaviour
{
    public GameObject cubePrefab; // Assign the cube prefab in the inspector
    private GameObject spawnedCube; // Holds the spawned cube instance

    private Color color1 = Color.white; 
    private Color color2 = Color.green; 
    private bool isColor1 = true; // Track the current color

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
            // Set the initial color to red
            Renderer cubeRenderer = spawnedCube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                cubeRenderer.material.color = color1;
            }
            else
            {
                Debug.LogError("The cube does not have a Renderer component.");
            }
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

    // Method to switch the color of the spawned cube
    public void SwitchColor()
    {
        if (spawnedCube != null)
        {
            Renderer cubeRenderer = spawnedCube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                // Toggle the color
                cubeRenderer.material.color = isColor1 ? color2 : color1;
                isColor1 = !isColor1;
            }
            else
            {
                Debug.LogError("The cube does not have a Renderer component.");
            }
        }
        else
        {
            Debug.LogError("No cube has been spawned yet.");
        }
    }
}

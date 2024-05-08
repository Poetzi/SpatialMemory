using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NameDisplayHandler : MonoBehaviour
{
    public Text displayText; // Assign this in the inspector
    public RandomSpawner spawner; // Assign this to reference the RandomSpawner
    public int iterations = 1; // Number of full iterations to display names

    private List<string> names = new List<string>();
    private int currentNameIndex = 0;
    private int totalIterationsDone = 0;
    private bool isActive = true; // Flag to control the display process

    public void InitializeNames()
    {
        if (displayText == null)
        {
            Debug.LogError("Display Text is not assigned!");
            return;
        }

        if (spawner == null)
        {
            Debug.LogError("RandomSpawner is not assigned!");
            return;
        }

        PopulateNamesList();
        ShuffleNames();

        // Set the display text to the first name in the shuffled list if there are any names
        if (names.Count > 0)
        {
            displayText.text = names[0];
            currentNameIndex = 1; // Start from the next name for the next call to DisplayNextName
        }
        else
        {
            Debug.LogError("No names available to display.");
        }
    }


    public void DisplayNextName()
    {
        if (!isActive || names.Count == 0)
        {
            return; // Stop function if the handler is inactive or there are no names
        }

        displayText.text = names[currentNameIndex];
        Debug.Log("Index " + currentNameIndex);
        currentNameIndex++;

        if (currentNameIndex >= (names.Count))
        {
            currentNameIndex = 0;
            totalIterationsDone++;
            if (totalIterationsDone >= iterations)
            {
                isActive = false; // Stop changing names after reaching the iteration limit
                Debug.Log("Completed all iterations of name display.");
            }
        }
    }

    private void PopulateNamesList()
    {
        names.Clear();  // Clear existing names
        foreach (GameObject obj in spawner.prefabs)
        {
            names.Add(obj.name);
        }
    }

    private void ShuffleNames()
    {
        for (int i = 0; i < names.Count; i++)
        {
            string temp = names[i];
            int randomIndex = Random.Range(i, names.Count);
            names[i] = names[randomIndex];
            names[randomIndex] = temp;
        }
    }

    public void ResetHandler()
    {
        isActive = true;
        totalIterationsDone = 0;
        currentNameIndex = 0;
        ShuffleNames();
    }

    // Method to check if all iterations are done
    public bool IsIterationComplete()
    {
        return !isActive;
    }

    public string GetCurrentDisplayText()
    {
        if (displayText != null)
            return displayText.text;
        return "Display Text is not assigned.";
    }
}

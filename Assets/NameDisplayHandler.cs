using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NameDisplayHandler : MonoBehaviour
{
    public TextMeshProUGUI displayText; // Assign this in the inspector
    public RandomSpawner spawner; // Assign this to reference the RandomSpawner
    public int iterations = 1; // Number of full iterations to display names

    private List<string> names = new List<string>();
    private int currentNameIndex = 0;
    private int totalIterationsDone = 0;
    private bool isActive = true; // Flag to control the display process
    private int trialNumber = 0; // Track the trial number

    private Color color1 = Color.white;
    private Color color2 = Color.green;
    private bool isColor1 = true; // Track the current color

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

        // Check if the scene is "Part4" to determine behavior
        if (SceneManager.GetActiveScene().name.Equals("Part4"))
        {
            // Set the display text to the first name in the shuffled list if there are any names
            if (names.Count > 0)
            {
                displayText.text = names[0];
                currentNameIndex = 0; // Start from the first name for the next call to DisplayNextName
                trialNumber = 0; // Initialize the trial number
            }
            else
            {
                Debug.LogError("No names available to display.");
            }
        }
        else
        {
            // Original behavior for other parts
            // Set the display text to the first name in the shuffled list if there are any names
            if (names.Count > 0)
            {
                displayText.text = names[0];
                currentNameIndex = 1; // Start from the next name for the next call to DisplayNextName
                trialNumber = 1; // Initialize the trial number
            }
            else
            {
                Debug.LogError("No names available to display.");
            }
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
        trialNumber++; // Increment the trial number

        if (currentNameIndex >= names.Count)
        {
            currentNameIndex = 0;
            totalIterationsDone++;
            if (totalIterationsDone >= iterations)
            {
                isActive = false; // Stop changing names after reaching the iteration limit
                Debug.Log("Completed all iterations of name display.");
            }
            else
            {
                ShuffleNames(); // Shuffle names for the next iteration
            }
        }
    }

    public void UpdateCurrentName()
    {
        if (!isActive || names.Count == 0)
        {
            return; // Stop function if the handler is inactive or there are no names
        }

        displayText.text = names[currentNameIndex];
        Debug.Log("Index " + currentNameIndex);
        trialNumber++; // Increment the trial number

        // Check if the current name has been displayed 12 times
        if (trialNumber % 12 == 0)
        {
            currentNameIndex++; // Move to the next name

            if (currentNameIndex >= names.Count)
            {
                currentNameIndex = 0;
                totalIterationsDone++;

                if (totalIterationsDone >= iterations)
                {
                    isActive = false; // Stop changing names after reaching the iteration limit
                    Debug.Log("Completed all iterations of name display.");
                }
                else
                {
                    ShuffleNames(); // Shuffle names for the next iteration if necessary
                }
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
        trialNumber = 0; // Reset the trial number
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

    public int GetCurrentTrialNumber()
    {
        return trialNumber;
    }

    public void SwitchColor()
    {
        if (displayText != null)
        {
            
                // Toggle the color
                displayText.color = isColor1 ? color2 : color1;
                isColor1 = !isColor1;           
        }
        else
        {
            Debug.LogError("Displaytext is null!");
        }
    }
}

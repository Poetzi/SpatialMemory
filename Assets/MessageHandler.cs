using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MessageHandler : MonoBehaviour
{
    public VRServer vrServer;
    public HandPositionManager handPositionManager;
    public RandomSpawner spawner;
    public NameDisplayHandler nameDisplayHandler;
    public StartObjectSpawner startObjectSpawner;
    public CSVWriter csvWriter;
    public Timer timer;
    private bool debug = true;

    private int clickCount = 0; // Track the number of clicks

    void Awake()

    {
        if (vrServer == null)
        {
            Debug.LogError("VRServer is not assigned in the inspector");
            enabled = false; // Disable script if no vrServer is assigned
        }
    }

    void Start()
    {
        vrServer.OnMessageReceived += HandleMessage;
        startObjectSpawner.SpawnCube();
        spawner.SpawnObjects();
        nameDisplayHandler.InitializeNames();
    }

    private void HandleMessage(string message)
    {
        if (nameDisplayHandler.IsIterationComplete())
        {
            HandleCompletedIteration();
        }
        else
        {
            HandleOngoingIteration();
        }
    }

    private void HandleOngoingIteration()
    {
        if (clickCount % 2 == 1) // Odd clicks
        {
            PerformActionsAndLog();
        }
        else // Even clicks
        {
            HandleEvenClick();
        }
    }

    private void HandleCompletedIteration()
    {
        if (clickCount % 2 == 1) // Odd clicks
        {
            PerformActionsAndLog();
            vrServer.OnMessageReceived -= HandleMessage;
        }
        else // Even clicks
        {
            HandleEvenClick();
        }
    }

    private void HandleEvenClick()
    {
        if (startObjectSpawner.IsInside(debug ? new Vector3(0.3f, 1, 0) : handPositionManager.GetIndexTipPosition()))
        {
            timer.StartTimer();
        }
        else
        {
            Debug.Log("Index finger not in start position; no actions performed.");
        }
        clickCount++;
    }

    private List<string> collectedData = new List<string>();

    private void PerformActionsAndLog()
    {
        // Perform the action which collects data
        PerformActions();

        // Stop the timer and get the elapsed time
        float time = timer.EndTimerAndGetElapsedTime();

        // Add the additional data to the collected data
        collectedData.Add("Time Elapsed: " + time.ToString());
        collectedData.Add("Click Count: " + clickCount.ToString());
        collectedData.Add("Index Tip Position: " + (debug ? new Vector3(0.3f, 1, 0) : handPositionManager.GetIndexTipPosition()).ToString());

        // Write the collected data to CSV in one line
        csvWriter.AddDataToCSV(collectedData);

        // Increment the click count for next use
        clickCount++;

        // Display the next name on the list
        nameDisplayHandler.DisplayNextName();

        // Clear the collected data list for the next round of data collection
        collectedData.Clear();
    }

    private void PerformActions()
    {
        // Get the current target name from the display.
        string targetName = nameDisplayHandler.GetCurrentDisplayText();

        // Retrieve the spawned positions and their corresponding names.
        var spawnedPositions = spawner.GetSpawnedPositionsAndNames();

        foreach (KeyValuePair<Vector3, string> pos in spawnedPositions)
        {
            // Check if the name at the current position matches the target name.
            if (pos.Value == targetName)
            {
                // Instead of writing here, add it to the collected data
                collectedData.Add("Spawned Position: " + pos.Key);
                collectedData.Add("Name: " + pos.Value);
                collectedData.Add("Target: " + targetName);
            }
        }
    }

    void OnDestroy()
    {
        if (vrServer != null)
        {
            vrServer.OnMessageReceived -= HandleMessage;
        }
    }
}

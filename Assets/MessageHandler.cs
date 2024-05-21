using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
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
    public bool randomSpawns = true;
    public List<PrefabPosition> customPrefabPositions;
    private bool debug = true;

    private int clickCount = 0; // Track the number of clicks

    private Vector3 evenClickIndexTipPosition;
    private bool evenClickPositionCaptured = false; // Flag to check if position was captured on even click


    private List<string> collectedData = new List<string>();

    // Create a CultureInfo object with dot as the decimal separator
    CultureInfo dotCulture = new CultureInfo("en-US");

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
        if(randomSpawns)
        {
            spawner.SpawnObjects();
        }
        else
        {
            spawner.SpawnObjectsAtSpecificPositions(customPrefabPositions);
        }        
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
        Vector3 indexTipPosition = debug ? new Vector3(0.3f, 1, 0) : handPositionManager.GetIndexTipPosition();

        if (startObjectSpawner.IsInside(indexTipPosition))
        {
            timer.StartTimer();
            // Store the index tip position on even clicks
            evenClickIndexTipPosition = indexTipPosition;
            evenClickPositionCaptured = true; // Mark that the position has been captured
            clickCount++;
        }
        else
        {
            Debug.Log("Index finger not in start position; no actions performed.");
        }

        
    }


    private bool isCSVInitialized = false;

    private void InitializeCSV()
    {
        if (!isCSVInitialized)
        {
            // Define the header for the CSV based on the data points being collected
            List<string> header = new List<string>
        {
            "StartObjectX", "StartObjectY", "StartObjectZ", "Object", "Target",
            "TimeElapsed", "ClickCount", "IndexTipX", "IndexTipY", "IndexTipZ",
            "StartObjectIndexTipX", "StartObjectIndexTipY", "StartObjectIndexTipZ"
        };

            // Write the header to the CSV
            csvWriter.WriteHeader(header);
            isCSVInitialized = true;
        }
    }



    private void PerformActionsAndLog()
    {
        // Ensure the CSV is ready to log data
        InitializeCSV();

        // Perform the action which collects data
        PerformActions();

        // Stop the timer and get the elapsed time
        float time = timer.EndTimerAndGetElapsedTime();
        Vector3 indexTipPosition = debug ? new Vector3(0.3f, 1, 0) : handPositionManager.GetIndexTipPosition();

        // Add additional data to the collected data
        collectedData.Add(time.ToString(dotCulture));
        collectedData.Add(clickCount.ToString(dotCulture));
        collectedData.Add(indexTipPosition.x.ToString(dotCulture));
        collectedData.Add(indexTipPosition.y.ToString(dotCulture));
        collectedData.Add(indexTipPosition.z.ToString(dotCulture));

        // Add the even click index tip position if it was captured
        if (evenClickPositionCaptured)
        {
            collectedData.Add(evenClickIndexTipPosition.x.ToString(dotCulture));
            collectedData.Add(evenClickIndexTipPosition.y.ToString(dotCulture));
            collectedData.Add(evenClickIndexTipPosition.z.ToString(dotCulture));
            evenClickPositionCaptured = false; // Reset flag
        }
        else
        {
            // Ensure columns are aligned if no data was captured
            collectedData.Add("");
            collectedData.Add("");
            collectedData.Add("");
        }

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
            if (pos.Value == targetName)
            {
                // Collect only values, ensuring order matches the header
                collectedData.Add(pos.Key.x.ToString(dotCulture));
                collectedData.Add(pos.Key.y.ToString(dotCulture));
                collectedData.Add(pos.Key.z.ToString(dotCulture));
                collectedData.Add(pos.Value);
                collectedData.Add(targetName);
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

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
        if (startObjectSpawner.IsInside(handPositionManager.GetIndexTipPosition()))
        {
            timer.StartTimer();
        }
        else
        {
            Debug.Log("Index finger not in start position; no actions performed.");
        }
        clickCount++;
    }

    private void PerformActionsAndLog()
    {
        PerformActions();
        float time = timer.EndTimerAndGetElapsedTime();
        List<string> data = new List<string>
    {
        "Time Elapsed: " + time.ToString(),
        "Click Count: " + clickCount.ToString(),
        "Index Tip Position: " + handPositionManager.GetIndexTipPosition().ToString(),
    };
        csvWriter.AddDataToCSV(data);
        clickCount++;
        nameDisplayHandler.DisplayNextName();
    }

    private void PerformActions()
    {
        var spawnedPositions = spawner.GetSpawnedPositions();
        foreach (Vector3 pos in spawnedPositions)
        {
            List<string> data = new List<string>
            {
                "Spawned Position: " + pos.ToString(),
                "Action: Performed",
                "Click Count: " + clickCount.ToString()
            };

            csvWriter.AddDataToCSV(data);
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

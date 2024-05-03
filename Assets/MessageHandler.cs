using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MessageHandler : MonoBehaviour
{
    public VRServer vrServer;
    public HandPositionManager handPositionManager;
    public RandomSpawner spawner;
    private bool firstTime = true;
    public NameDisplayHandler nameDisplayHandler;
    public StartObjectSpawner startObjectSpawner;
    public CSVWriter csvWriter;

    void Awake()
    {
        // Safeguard to ensure vrServer is assigned
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
    }

    private void HandleMessage(string message)
    {
        if (!nameDisplayHandler.IsIterationComplete())
        {
            
            if (firstTime)
            {
                spawner.SpawnObjects();
                nameDisplayHandler.InitializeNames();
                firstTime = false;
            }
            Debug.Log("Message Handler: " + handPositionManager.GetIndexTipPosition());
            var spawnedPositions = spawner.GetSpawnedPositions();
            foreach (Vector3 pos in spawnedPositions)
            {
                Debug.Log("Spawned Position: " + pos);
            }
            nameDisplayHandler.DisplayNextName();
            if (startObjectSpawner.IsInside(handPositionManager.GetIndexTipPosition()))
            {
                Debug.Log("Index Finger is inside StartPosition: ");
            }
            List<string> data = new List<string> { "John", "Doe", "30" };
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

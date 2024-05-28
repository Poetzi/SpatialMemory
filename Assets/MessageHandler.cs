using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using static UnityEditor.PlayerSettings;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MessageHandler : MonoBehaviour
{
    public VRServer vrServer;
    public HandPositionManager handPositionManager;
    public RandomSpawner spawner;
    public NameDisplayHandler nameDisplayHandler;
    public StartObjectSpawner startObjectSpawner;
    public CSVWriter csvWriter;
    public SceneTransitionManager sceneTransitionManager;
    public Timer timer;
    public DrawGizmo drawGizmo;
    public ClearPlayerPrefs clearPlayerPrefs;
    public bool randomSpawns = true;
    public List<PrefabPosition> customPrefabPositions;
    private bool debug = true;
    public int sceneChangeId;

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
        
        if (PlayerPrefs.HasKey("SpawnedObjectCount"))
            {
                spawner.LoadSpawnedObjects();
            }
            else
            {
                if (randomSpawns)
                {
                    spawner.SpawnObjects();
                }
                else
                {
                    spawner.SpawnObjectsAtSpecificPositions(customPrefabPositions);
                }
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
            spawner.SaveSpawnedObjects(); // Save positions before transition
            vrServer.OnMessageReceived -= HandleMessage;
            if(clearPlayerPrefs != null)
            {
                clearPlayerPrefs.ClearSavedData();
            }
            if (SceneManager.GetActiveScene().name.Equals("Part0"))
            {
                csvWriter.DeleteCSV();
            }
            StartCoroutine(sceneTransitionManager.TransitionToScene(sceneChangeId));
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
            startObjectSpawner.SwitchColor();
            // Store the index tip position on even clicks
            evenClickIndexTipPosition = indexTipPosition;
            evenClickPositionCaptured = true; // Mark that the position has been captured
            // Log the middle point of the box
            LogBoxCenter();
            // Log if the even click is inside the target object
            LogIfInsideTargetObject(indexTipPosition);
            // Find the nearest object and log it
            FindAndLogNearestObject(indexTipPosition);
            clickCount++;
        }
        else
        {
            Debug.Log("Index finger not in start position; no actions performed.");
        }

        
    }

    private void LogBoxCenter()
    {
        Vector3 boxCenter = drawGizmo.GetBoxCenter();
        collectedData.Add(boxCenter.x.ToString(dotCulture));
        collectedData.Add(boxCenter.y.ToString(dotCulture));
        collectedData.Add(boxCenter.z.ToString(dotCulture));
    }


    private bool isCSVInitialized = false;

    private void InitializeCSV()
    {
        if (!isCSVInitialized)
        {
            // Define the header for the CSV based on the data points being collected
            List<string> header = new List<string>
        {
                "BoxCenterX", "BoxCenterY", "BoxCenterZ", "IsInsideTargetObject", "NearestObjectX", "NearestObjectY", "NearestObjectZ", "NearestObjectName", "TargetX", "TargetY", "TargetZ", "Object", "Target",
                "TimeElapsed", "ClickCount", "IndexTipX", "IndexTipY", "IndexTipZ",
                "StartObjectIndexTipX", "StartObjectIndexTipY", "StartObjectIndexTipZ", "TrialNumber",

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
        startObjectSpawner.SwitchColor();
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

        // Add trial number
        collectedData.Add(nameDisplayHandler.GetCurrentTrialNumber().ToString());

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

    private void FindAndLogNearestObject(Vector3 indexTipPosition)
    {
        var spawnedPositions = spawner.GetSpawnedPositionsAndNames();
        Vector3 nearestObjectPosition = Vector3.zero;
        string nearestObjectName = "";
        float minDistance = float.MaxValue;

        foreach (var pos in spawnedPositions)
        {
            float distance = Vector3.Distance(indexTipPosition, pos.Key);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestObjectPosition = pos.Key;
                nearestObjectName = pos.Value;
            }
        }

        collectedData.Add(nearestObjectPosition.x.ToString(dotCulture));
        collectedData.Add(nearestObjectPosition.y.ToString(dotCulture));
        collectedData.Add(nearestObjectPosition.z.ToString(dotCulture));
        collectedData.Add(nearestObjectName);
    }

    private void LogIfInsideTargetObject(Vector3 indexTipPosition)
    {
        var spawnedGameObjects = spawner.GetSpawnedGameObjectsAndNames();
        string targetName = nameDisplayHandler.GetCurrentDisplayText();
        bool isInside = false;

        foreach (var obj in spawnedGameObjects)
        {
            Collider collider = obj.Key.GetComponent<Collider>();
            if (collider != null && collider.bounds.Contains(indexTipPosition) && obj.Value == targetName)
            {
                isInside = true;
                break;
            }
        }

        collectedData.Add(isInside.ToString());
    }

    void OnDestroy()
    {
        if (vrServer != null)
        {
            vrServer.OnMessageReceived -= HandleMessage;
        }
    }
}

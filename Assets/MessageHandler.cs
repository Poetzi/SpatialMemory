using UnityEngine;

public class MessageHandler : MonoBehaviour
{
    public VRServer vrServer;
    public HandPositionManager handPositionManager;

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
    }

    private void HandleMessage(string message)
    {
        Debug.Log("Message Handler: " + handPositionManager.GetIndexTipPosition());
    }

    void OnDestroy()
    {
        if (vrServer != null)
        {
            vrServer.OnMessageReceived -= HandleMessage;
        }
    }
}

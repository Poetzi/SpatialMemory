using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class VRServer : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

    // Define an event to notify subscribers when a message is received
    public event Action<string> OnMessageReceived;

    void Start()
    {
        udpClient = new UdpClient(8193);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 8193);

        StartListening();
    }

    private void StartListening()
    {
        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        byte[] bytes = udpClient.EndReceive(ar, ref remoteEndPoint);
        string message = Encoding.UTF8.GetString(bytes);
        Debug.Log("Received message: " + message);

        // Queue the invocation of the OnMessageReceived event on the main thread
        mainThreadActions.Enqueue(() => OnMessageReceived?.Invoke(message));

        // Continue listening for more messages
        StartListening();
    }

    void Update()
    {
        while (mainThreadActions.TryDequeue(out Action action))
        {
            action.Invoke();
        }
    }

    void OnDestroy()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}

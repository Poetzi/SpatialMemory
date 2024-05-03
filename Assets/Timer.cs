using UnityEngine;

public class Timer : MonoBehaviour
{
    private float startTime; // To store the start time of the timer
    private float endTime;   // To store the end time of the timer

    // Method to start the timer
    public void StartTimer()
    {
        startTime = Time.time;
    }

    // Method to end the timer and retrieve the elapsed time in seconds
    public float EndTimerAndGetElapsedTime()
    {
        endTime = Time.time;
        return endTime - startTime;
    }

    // Method to generate a timestamp
    public string GenerateTimestamp()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

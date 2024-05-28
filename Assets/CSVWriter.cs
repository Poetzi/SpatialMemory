using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CSVWriter : MonoBehaviour
{
    private string filePath;
    public int participantID = 0;
    public int blockNumber = 0; 

    void Start()
    {
        // Set block number based on the scene
        blockNumber = SceneManager.GetActiveScene().buildIndex;

        // Generate the file path dynamically at the start of the application
        string sceneName = SceneManager.GetActiveScene().name;
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filePath = Path.Combine(Application.persistentDataPath, $"{participantID}_{sceneName}_{timestamp}.csv");

        // Optionally, you can clear the existing file to start fresh
        // ResetCSV(); // Uncomment this if you need a fresh start each time
    }

    // Method to write the header to the CSV file
    public void WriteHeader(List<string> headerColumns)
    {
        headerColumns.Insert(0, "BlockNumber"); // Add BlockNumber as the first column
        string header = string.Join(";", headerColumns); // Combine the header titles with commas
        // Write or overwrite the header to the file
        using (StreamWriter writer = new StreamWriter(filePath, false)) // false to overwrite existing content
        {
            writer.WriteLine(header);
        }
    }

    // Method to add a new row of data to the CSV file
    public void AddDataToCSV(string rowData)
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(rowData);
        }
    }

    // Overloaded method for adding multiple fields in a row
    public void AddDataToCSV(List<string> rowData)
    {
        rowData.Insert(0, blockNumber.ToString()); // Add block number as the first column
        string row = string.Join(";", rowData);
        AddDataToCSV(row);
    }

    // Optional: Method to clear the existing CSV file
    public void ResetCSV()
    {
        using (StreamWriter writer = new StreamWriter(filePath, false)) // false to overwrite existing content
        {
            writer.Flush();
        }
    }

    // Method to delete the CSV file
    public void DeleteCSV()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}

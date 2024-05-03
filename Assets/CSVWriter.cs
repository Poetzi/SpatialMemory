using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVWriter : MonoBehaviour
{
    private string filePath;

    void Start()
    {
        // Set the file path at start or dynamically set it based on requirements
        filePath = Application.persistentDataPath + "/data.csv";
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
        string row = string.Join(",", rowData);
        AddDataToCSV(row);
    }
}

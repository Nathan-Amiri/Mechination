using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLayout : MonoBehaviour
{
    // Create a field for the save file.
    string saveFile;

    // Create a GameData field.
    LayoutData layoutData = new();

    void Awake()
    {
        // Update the path once the persistent path exists.
        saveFile = Application.persistentDataPath + "/gamedata.json";
    }

    public void ReadFile()
    {
        // Does the file exist?
        if (File.Exists(saveFile))
        {
            // Read the entire file and save its contents.
            string fileContents = File.ReadAllText(saveFile);

            // Deserialize the JSON data 
            //  into a pattern matching the GameData class.
            layoutData = JsonUtility.FromJson<LayoutData>(fileContents);
        }
    }

    public void WriteFile()
    {
        foreach (KeyValuePair<Vector2Int, Cell> gridIndexEntry in Cell.gridIndex)
        {
            Cell cell = gridIndexEntry.Value;
            CellData cellData = new()
            {
                cellType = cell.cellType,
                nodeColorNumber = cell.nodeColorNumber,
                cellRotation = Mathf.RoundToInt(cell.transform.rotation.eulerAngles.z),
                cellPosition = gridIndexEntry.Key
            };

            layoutData.cellsInLayout.Add(cellData);
        }

        // Serialize the object into JSON and save string.
        string jsonString = JsonUtility.ToJson(layoutData);

        // Write JSON to file.
        File.WriteAllText(saveFile, jsonString);
    }
}
[System.Serializable]
public class LayoutData
{
    public List<CellData> cellsInLayout = new();
}
public struct CellData
{
    public int cellType; //0 = node, 1 = pulser, 2 = magnet
    public int nodeColorNumber;
    public int cellRotation;
    public Vector2Int cellPosition;
}
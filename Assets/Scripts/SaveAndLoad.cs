using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveAndLoad : MonoBehaviour
{
    [SerializeField] private EditModeManager editModeManager;

    private readonly List<string> saveFiles = new();

    void Awake()
    {
        saveFiles.Add(Application.persistentDataPath + "/Grid1.json");
        saveFiles.Add(Application.persistentDataPath + "/Grid2.json");
        saveFiles.Add(Application.persistentDataPath + "/Grid3.json");
        saveFiles.Add(Application.persistentDataPath + "/Grid4.json");
        saveFiles.Add(Application.persistentDataPath + "/Grid5.json");

        foreach (string file in saveFiles)
            if (!File.Exists(file))
                File.WriteAllText(file, "");
    }

    public void LoadLayout(int newLayoutNumber)
    {
        editModeManager.ClearGrid();

        // Get layoutData from file
        string fileContents = File.ReadAllText(saveFiles[newLayoutNumber]);

        if (fileContents.Length == 0)
            return;

        LayoutData layoutData = JsonUtility.FromJson<LayoutData>(fileContents);

        foreach (CellData cellData in layoutData.cellsInLayout)
        {
            Quaternion cellRotation = Quaternion.Euler(0, 0, cellData.cellRotation);

            editModeManager.SpawnCell(cellData.cellType, cellData.cellPosition, cellRotation, cellData.nodeColorNumber, true);
        }
    }

    public void SaveLayout(int currentLayoutNumber)
    {
        LayoutData layoutData = new();

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

        // Save layoutData to file
        string jsonString = JsonUtility.ToJson(layoutData, true);

        File.WriteAllText(saveFiles[currentLayoutNumber], jsonString);
    }
}

[System.Serializable]
public class LayoutData
{
    public List<CellData> cellsInLayout = new();
}

[System.Serializable]
public class CellData
{
    public int cellType; // 0 = node, 1 = pulser, 2 = magnet
    public int nodeColorNumber;
    public int cellRotation;
    public Vector2Int cellPosition;
}
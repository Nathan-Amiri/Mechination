using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveAndLoad : MonoBehaviour
{
    [SerializeField] private EditModeManager editModeManager;
    [SerializeField] private Tutorial tutorial;

    private readonly List<string> saveFiles = new();

    private string tutorialFile;

    private void Awake()
    {
        // Application.persistentDataPath cannot be called in a constructor
        saveFiles.Add(Application.persistentDataPath + "/Grid1.json");
        saveFiles.Add(Application.persistentDataPath + "/Grid2.json");
        saveFiles.Add(Application.persistentDataPath + "/Grid3.json");
        saveFiles.Add(Application.persistentDataPath + "/Grid4.json");
        saveFiles.Add(Application.persistentDataPath + "/Grid5.json");

        tutorialFile = Application.persistentDataPath + "/tutorialGrid.json";

        // Create save files
        foreach (string file in saveFiles)
            if (!File.Exists(file))
                File.WriteAllText(file, string.Empty);

        if (!File.Exists(tutorialFile))
            File.WriteAllText(tutorialFile, string.Empty);
    }

    // Called by EditModeManager
    public void LoadLayout(int newLayoutNumber)
    {
        // In tutorial mode, newLayoutNumber is unneeded

        editModeManager.ClearGrid();

        // Get layoutData from file
        if (tutorial.tutorialMode && !File.Exists(tutorialFile))
            Debug.LogError("tutorial file not found");

        string file = tutorial.tutorialMode ? tutorialFile : saveFiles[newLayoutNumber];
        string fileContents = File.ReadAllText(file);

        if (fileContents.Length == 0)
            return;

        LayoutData layoutData = JsonUtility.FromJson<LayoutData>(fileContents);

        foreach (CellData cellData in layoutData.cellsInLayout)
        {
            Quaternion cellRotation = Quaternion.Euler(0, 0, cellData.cellRotation);

            editModeManager.SpawnCell(cellData.cellType, cellData.cellPosition, cellRotation, cellData.nodeColorNumber, true);
        }
    }

    // Called by EditModeManager
    public void SaveLayout(int currentLayoutNumber)
    {
        // In tutorial mode, currentLayoutNumber is unneeded

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

        // Save layoutData to current layout's file. If in tutorial mode, instead save to the temporary tutorial file
        string jsonString = JsonUtility.ToJson(layoutData, false);

        if (tutorial.tutorialMode && !File.Exists(tutorialFile))
            Debug.LogError("tutorial file not found");

        string saveFile = tutorial.tutorialMode ? tutorialFile : saveFiles[currentLayoutNumber];
        File.WriteAllText(saveFile, jsonString);
    }

    // Called by Tutorial
    public void ClearTutorialFile()
    {
        File.WriteAllText(tutorialFile, string.Empty);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveAndLoad : MonoBehaviour
{
    [SerializeField] private HUD hud;

    private string saveFile;

    private LayoutData layoutData = new();

    void Awake()
    {
        saveFile = Application.persistentDataPath + "/gamedata.json";
    }

    public void LoadLayout(int newLayoutNumber)
    {
        if (!File.Exists(saveFile)) return;

        hud.ClearGrid();

        // Get layoutData from file
        string fileContents = File.ReadAllText(saveFile);
        layoutData = JsonUtility.FromJson<LayoutData>(fileContents);

        foreach (CellData cellData in layoutData.layouts[newLayoutNumber].layout)
        {
            Quaternion cellRotation = Quaternion.Euler(0, 0, cellData.cellRotation);

            hud.SpawnCell(cellData.cellType, cellData.cellPosition, cellRotation, cellData.nodeColorNumber);
        }
    }

    public void SaveLayout(int currentLayoutNumber)
    {
        // Update layoutData
        layoutData.layouts[currentLayoutNumber].layout.Clear();

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

            layoutData.layouts[currentLayoutNumber].layout.Add(cellData);
        }

        // Save layoutData to file
        string jsonString = JsonUtility.ToJson(layoutData, true);
        File.WriteAllText(saveFile, jsonString);
    }
}

[System.Serializable]
public class LayoutData
{
    // JsonUtility doesn't support lists of lists, needs wrapper class to hold each list
    public List<LayoutWrapper> layouts = new()
    {
        new LayoutWrapper(),
        new LayoutWrapper(),
        new LayoutWrapper(),
        new LayoutWrapper(),
        new LayoutWrapper()
    };
}

[System.Serializable]
public class LayoutWrapper
{
    public List<CellData> layout = new();
}

[System.Serializable]
public struct CellData
{
    public int cellType; // 0 = node, 1 = pulser, 2 = magnet
    public int nodeColorNumber;
    public int cellRotation;
    public Vector2Int cellPosition;
}
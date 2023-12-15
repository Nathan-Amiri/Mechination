using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    /*
    todo:

    check/x marks from iconfinder for save icon once wifi is on again

    saves (save current loadout in playerprefs, load on start)

    play saves before playing
    stop reloads save

    clear
    clear/save/exit popups

    tutorial (start game first time with how to play, save in playerprefs to not do again)
    */


    //SCENE REFERENCE
    [SerializeField] private Cell pulserPref;
    [SerializeField] private Cell magnetPref;
    [SerializeField] private Cell nodePref;

    [SerializeField] private CycleManager cycleManager;
    [SerializeField] private SaveAndLoad saveAndLoad;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform cellParent;
    
    [SerializeField] private GameObject playIcon;
    [SerializeField] private GameObject stopIcon;

    [SerializeField] private GameObject eraserIcon;
    [SerializeField] private GameObject clearIcon;

    [SerializeField] private TMP_Text tickSpeedMultiplierText;

    [SerializeField] private Button saveButton;

    [SerializeField] private Button pulserButton;
    [SerializeField] private Button magnetButton;
    [SerializeField] private Button nodeButton;
    [SerializeField] private Button eraserButton;

    [SerializeField] private Transform pulserButtonTR;
    [SerializeField] private Transform magnetButtonTR;

    [SerializeField] private Image nodeImage;
    [SerializeField] private List<Color32> nodeColors;

    //DYNAMIC
    public enum SpawnType { pulser, magnet, node, eraser }
    private SpawnType currentSpawnType;

    private Button currentCellButton;

    private readonly List<float> tickSpeedMultipliers = new() { .25f, .5f, 1, 2, 4 };
    private float currentTickSpeedMultiplier = 1;

    private bool isPlaying;

    private int pulserZRotation;
    private int magnetZRotation;

    private int currentNodeColorNumber;

    private bool layoutSaved = true;

    private bool currentlyErasing;


    private void Start()
    {
        SelectEraserTrash();

        if (PlayerPrefs.HasKey("tickSpeedMultiplier"))
            UpdateTickMultiplier(PlayerPrefs.GetFloat("tickSpeedMultiplier"));
        else
            cycleManager.SetTickSpeed(1);

        //get the layout number from playerprefs


        //load layout
        saveAndLoad.LoadLayout();
    }

    private void Update()
    {
        if (isPlaying) return;

        //check if mouse is over ui
        if (EventSystem.current.IsPointerOverGameObject()) return;

        //rotate before spawning so that the spawned cell will be of the correct rotation
        if (Input.GetMouseButtonDown(0))
            RotateGadget();

        if (Input.GetMouseButton(0))
            PrepareToSpawnCell();
    }

    private void RotateGadget()
    {
        //check if a gadget is selected
        if (currentSpawnType == SpawnType.node || currentSpawnType == SpawnType.eraser) return;

        //check if cell exists at mouse position
        Vector2Int gridPosition = MouseGridPosition();
        if (!Cell.gridIndex.TryGetValue(gridPosition, out Cell cell)) return;

        //check if cell is a gadget
        if (cell is not Gadget gadget) return;

        //check if the gadget is of the selected type
        if (gadget.isPulser != (currentSpawnType == SpawnType.pulser)) return;

        //rotate gadget, gadget button, and future spawned gadgets of the selected type
        gadget.transform.rotation *= Quaternion.Euler(0, 0, -90);
        gadget.gadgetDirection = Vector2Int.RoundToInt(gadget.transform.up);

        if (gadget.isPulser)
        {
            pulserZRotation = Mathf.RoundToInt(gadget.transform.rotation.eulerAngles.z);
            pulserButtonTR.rotation = gadget.transform.rotation;
        }
        else //if magnet
        {
            magnetZRotation = Mathf.RoundToInt(gadget.transform.rotation.eulerAngles.z);
            magnetButtonTR.rotation = gadget.transform.rotation;
        }

        //after rotating, layout has changed
        UpdateLayoutSaved(false);

        //unfasten and refasten gadget after rotating
        gadget.UnFastenCell();
        gadget.FastenCell();
    }

    private void PrepareToSpawnCell()
    {
        Quaternion spawnRotation = Quaternion.identity;
        int newCellType = 0;

        if (currentSpawnType == SpawnType.pulser)
        {
            spawnRotation = Quaternion.Euler(0, 0, pulserZRotation);
            newCellType = 1;
        }
        else if (currentSpawnType == SpawnType.magnet)
        {
            spawnRotation = Quaternion.Euler(0, 0, magnetZRotation);
            newCellType = 2;
        }
        //for eraser and node, remain default

        //get grid position
        Vector2Int gridPosition = MouseGridPosition();

        //if cell exists, return if we aren't erasing and the cell is identical to the cell that would be spawned
        //else, erase existing cell
        if (Cell.gridIndex.TryGetValue(gridPosition, out Cell cellAtPosition))
        {
            if (currentSpawnType != SpawnType.eraser && CellAlreadySpawned(cellAtPosition)) return;

            DespawnCell(cellAtPosition, gridPosition, true);
        }

        if (currentSpawnType == SpawnType.eraser) return;

        //spawn new cell
        SpawnCell(newCellType, gridPosition, spawnRotation, currentNodeColorNumber);
        //after spawning, layout has changed
        UpdateLayoutSaved(false);
    }
    
    //called by PrepareToSpawnCell and SaveAndLoad's LoadLayout
    public void SpawnCell(int newCellType, Vector2Int gridPosition, Quaternion spawnRotation, int nodeColorNumber)
    {
        Cell prefToSpawn = nodePref;
        if (newCellType != 0)
            prefToSpawn = newCellType == 1 ? pulserPref : magnetPref;

        Cell newCell = Instantiate(prefToSpawn, (Vector2)gridPosition, spawnRotation, cellParent);
        Cell.gridIndex.Add(gridPosition, newCell);
        newCell.currentPosition = gridPosition;
        //used for layout saving
        newCell.cellType = newCellType;

        if (newCell is Gadget newGadget)
            //must set gadgetDirection before fastening
            newGadget.gadgetDirection = Vector2Int.RoundToInt(newGadget.transform.up);
        else //if node
        {
            newCell.sr.color = nodeColors[nodeColorNumber];
            newCell.nodeColorNumber = nodeColorNumber;
        }

        //fasten cell
        newCell.FastenCell();
    }

    private bool CellAlreadySpawned(Cell cell)
    {
        //returns true if the cell is identical to the cell that would be spawned

        if (cell is Gadget gadget)
        {
            if (currentSpawnType == SpawnType.node)
                return false;

            return gadget.isPulser == (currentSpawnType == SpawnType.pulser);
        }

        if (currentSpawnType != SpawnType.node)
            return false;

        return cell.nodeColorNumber == currentNodeColorNumber;
    }

    private void DespawnCell(Cell cell, Vector2Int cellPosition, bool unfastenCell)
    {
        //if clearing grid, cell can't be unfastened since fasteners are removed based on
        //current position. Also, there's no need because they will be destroyed when their
        //parented cells are despawned
        if (unfastenCell)
            cell.UnFastenCell();

        Destroy(cell.gameObject);
        Cell.gridIndex.Remove(cellPosition);

        //after erasing, layout has changed
        UpdateLayoutSaved(false);
    }

    private Vector2Int MouseGridPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        Vector3 selectedPosition = mainCamera.ScreenToWorldPoint(mousePos);
        return new(RoundFloatToOddInt(selectedPosition.x), RoundFloatToOddInt(selectedPosition.y));
    }

    private int RoundFloatToOddInt(float f)
    {
        return Mathf.FloorToInt(f * 0.5f) * 2 + 1;
    }

    public void SelectExit()
    {
        Application.Quit();
    }

    public void SelectSave()
    {
        saveAndLoad.SaveLayout();
        UpdateLayoutSaved(true);
    }
    private void UpdateLayoutSaved(bool saved)
    {
        layoutSaved = saved;

        saveButton.interactable = !layoutSaved;
    }

    public void SelectLoadSaveFile(int saveFile)
    {

    }

    public void SelectPlayStop()
    {
        isPlaying = !isPlaying;

        //if stopping, stop cycle, clear, load layout, then adjust interactable in that order
        if (!isPlaying)
        {
            cycleManager.StartStopCycle(false);

            saveAndLoad.LoadLayout();
        }

        //adjust interactable
        SetButtonsInteractable(currentCellButton, isPlaying);

        playIcon.SetActive(!isPlaying);
        stopIcon.SetActive(isPlaying);

        //if playing, adjust interactable, save layout, then start cycle in that order
        if (isPlaying)
        {
            SelectSave();

            cycleManager.StartStopCycle(true);
        }
    }
    private void SetButtonsInteractable(Button newUninteractableButton, bool disableAll = false)
    {
        pulserButton.interactable = !disableAll;
        magnetButton.interactable = !disableAll;
        nodeButton.interactable = !disableAll;
        eraserButton.interactable = !disableAll;

        if (newUninteractableButton != null)
            newUninteractableButton.interactable = false;

        //even if null
        currentCellButton = newUninteractableButton;
    }

    public void SelectTickMultipler()
    {
        //if current multiplier isn't found in the first 4 multipliers, then it's 4, so it remains at .25f
        float newMultiplier = .25f;
        for (int i = 0; i < tickSpeedMultipliers.Count - 1; i++)
            if (currentTickSpeedMultiplier == tickSpeedMultipliers[i])
                newMultiplier = tickSpeedMultipliers[i + 1];

        PlayerPrefs.SetFloat("tickSpeedMultiplier", newMultiplier);

        UpdateTickMultiplier(newMultiplier);
    }
    private void UpdateTickMultiplier(float newMultiplier)
    {
        //set text. Remove 0 from 0.25 and 0.5
        tickSpeedMultiplierText.text = newMultiplier.ToString().TrimStart('0') + "x";

        //reset current multiplier
        currentTickSpeedMultiplier = newMultiplier;

        //change tick speed
        cycleManager.SetTickSpeed(newMultiplier);
    }

    public void SelectPulser()
    {
        currentSpawnType = SpawnType.pulser;
        SetButtonsInteractable(pulserButton);

        ToggleErasing(false);
    }

    public void SelectMagnet()
    {
        currentSpawnType = SpawnType.magnet;
        SetButtonsInteractable(magnetButton);

        ToggleErasing(false);
    }

    public void SelectNode()
    {
        currentSpawnType = SpawnType.node;
        SetButtonsInteractable(nodeButton);

        ToggleErasing(false);
    }

    public void SelectEraserTrash()
    {
        currentlyErasing = !currentlyErasing;

        if (currentlyErasing)
        {
            currentSpawnType = SpawnType.eraser;
            //turn on all buttons
            SetButtonsInteractable(null);

            ToggleErasing(true);
        }
        else
        {
            ClearGrid();

            //continue erasing
            currentlyErasing = true;
        }
    }
    //called by SelectEraserTrash and SaveAndLoad's LoadLayout;
    public void ClearGrid()
    {
        //must despawn outside foreach loop since despawning modifies the dict
        List<KeyValuePair<Vector2Int, Cell>> pairs = new();
        foreach (KeyValuePair<Vector2Int, Cell> pair in Cell.gridIndex)
            pairs.Add(pair);

        foreach(KeyValuePair<Vector2Int, Cell> pair in pairs)
            DespawnCell(pair.Value, pair.Key, false);

        //fasteners have already been destroyed due to cell despawning,
        //but fastenedCells and fastenerIndex need to be reset
        Cell.fastenedCells.Clear();
        Cell.fastenerIndex.Clear();
    }

    private void ToggleErasing(bool erasing)
    {
        eraserIcon.SetActive(!erasing);
        clearIcon.SetActive(erasing);

        if (!erasing)
            currentlyErasing = false;
    }

    public void SelectNodeColor(int colorNumber)
    {
        nodeImage.color = nodeColors[colorNumber];
        currentNodeColorNumber = colorNumber;
    }

    public void SelectTutorial()
    {

    }
}
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
    [SerializeField] private SaveLayout saveLayout;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform cellParent;

    [SerializeField] private GameObject playIcon;
    [SerializeField] private GameObject stopIcon;

    [SerializeField] private GameObject eraserIcon;
    [SerializeField] private GameObject clearIcon;

    [SerializeField] private TMP_Text tickSpeedMultiplierText;

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


    private void Start()
    {
        SelectEraserTrash();

        if (PlayerPrefs.HasKey("tickSpeedMultiplier"))
            UpdateTickMultiplier(PlayerPrefs.GetFloat("tickSpeedMultiplier"));
        else
            cycleManager.ChangeTickSpeed(1);
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
            SpawnCell();
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

        //unfasten and refasten gadget after rotating
        gadget.UnFastenCell();
        gadget.FastenCell();
    }

    private void SpawnCell()
    {
        //get prefab
        Cell prefToSpawn = null;

        Quaternion spawnRotation = Quaternion.identity;
            //used for layout saving
        int newCellType = 0;

        if (currentSpawnType == SpawnType.node)
            prefToSpawn = nodePref;
            //spawnRotation and cellType remains default
        else if (currentSpawnType == SpawnType.pulser)
        {
            prefToSpawn = pulserPref;
            spawnRotation = Quaternion.Euler(0, 0, pulserZRotation);
            newCellType = 1;
        }
        else if (currentSpawnType == SpawnType.magnet)
        {
            prefToSpawn = magnetPref;
            spawnRotation = Quaternion.Euler(0, 0, magnetZRotation);
            newCellType = 2;
        }
        //else remain null

        //get grid position
        Vector2Int gridPosition = MouseGridPosition();

        //if cell exists, return if we aren't erasing and the cell is identical to the cell that would be spawned
        //else, erase existing cell
        if (Cell.gridIndex.TryGetValue(gridPosition, out Cell cellAtPosition))
        {
            if (currentSpawnType != SpawnType.eraser && CellAlreadySpawned(cellAtPosition)) return;

            cellAtPosition.UnFastenCell();
            Destroy(cellAtPosition.gameObject);
            Cell.gridIndex.Remove(gridPosition);
        }

        if (currentSpawnType == SpawnType.eraser) return;

        //spawn new cell
        Cell newCell = Instantiate(prefToSpawn, (Vector2)gridPosition, spawnRotation, cellParent);
        Cell.gridIndex.Add(gridPosition, newCell);
        newCell.currentPosition = gridPosition;
        newCell.cellType = newCellType;

        if (newCell is Gadget newGadget)
            //must set gadgetDirection before fastening
            newGadget.gadgetDirection = Vector2Int.RoundToInt(newGadget.transform.up);
        else //if node
        {
            newCell.sr.color = nodeColors[currentNodeColorNumber];
            newCell.nodeColorNumber = currentNodeColorNumber;
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

    private void SetCellButtonsInteractable(Button newUninteractableButton, bool disableAll = false)
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

    private void UpdateTickMultiplier(float newMultiplier)
    {
        //set text. Remove 0 from 0.25 and 0.5
        tickSpeedMultiplierText.text = newMultiplier.ToString().TrimStart('0') + "x";

        //reset current multiplier
        currentTickSpeedMultiplier = newMultiplier;

        //change tick speed
        cycleManager.ChangeTickSpeed(newMultiplier);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Save()
    {

    }

    public void LoadSaveFile(int saveFile)
    {

    }

    public void PlayStop()
    {
        cycleManager.StartStopCycle(!isPlaying);

        isPlaying = !isPlaying;

        SetCellButtonsInteractable(currentCellButton, isPlaying);

        playIcon.SetActive(!isPlaying);
        stopIcon.SetActive(isPlaying);

        saveLayout.WriteFile();
    }

    public void TickMultipler()
    {
        //if current multiplier isn't found in the first 4 multipliers, then it's 4, so it remains at .25f
        float newMultiplier = .25f;
        for (int i = 0; i < tickSpeedMultipliers.Count - 1; i++)
            if (currentTickSpeedMultiplier == tickSpeedMultipliers[i])
                newMultiplier = tickSpeedMultipliers[i + 1];

        PlayerPrefs.SetFloat("tickSpeedMultiplier", newMultiplier);

        UpdateTickMultiplier(newMultiplier);
    }

    public void SelectPulser()
    {
        currentSpawnType = SpawnType.pulser;
        SetCellButtonsInteractable(pulserButton);

        ToggleEraser(true);
    }

    public void SelectMagnet()
    {
        currentSpawnType = SpawnType.magnet;
        SetCellButtonsInteractable(magnetButton);

        ToggleEraser(true);
    }

    public void SelectNode()
    {
        currentSpawnType = SpawnType.node;
        SetCellButtonsInteractable(nodeButton);

        ToggleEraser(true);
    }

    public void SelectEraserTrash()
    {
        currentSpawnType = SpawnType.eraser;
        //turn on all buttons
        SetCellButtonsInteractable(null);

        ToggleEraser(false);
    }

    private void ToggleEraser(bool eraserOn)
    {
        eraserIcon.SetActive(eraserOn);
        clearIcon.SetActive(!eraserOn);
    }

    public void NodeColor(int colorNumber)
    {
        nodeImage.color = nodeColors[colorNumber];
        currentNodeColorNumber = colorNumber;
    }

    public void Tutorial()
    {

    }
}
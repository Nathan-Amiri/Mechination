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

    rotation
        gridposition not changing means rotation code isn't reached when mousebutton is down again
        I need to allow for mouse button down for rotation
        multiplying pulserRotation by (0, 0, 90) does nothing

    reorganize variables

    prevent gadget reversal on play

    fastening

    saves (save current loadout in playerprefs, load on start)

    play saves before playing
    stop reloads save

    save/exit popups

    tutorial (start game first time with how to play, save in playerprefs to not do again)
    */



    [SerializeField] private Cell pulserPref;
    [SerializeField] private Cell magnetPref;
    [SerializeField] private Cell nodePref;

    [SerializeField] private CycleManager cycleManager;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform cellParent;

    [SerializeField] private GameObject playIcon;
    [SerializeField] private GameObject stopIcon;

    [SerializeField] private TMP_Text tickSpeedMultiplierText;

    [SerializeField] private Button pulserButton;
    [SerializeField] private Button magnetButton;
    [SerializeField] private Button nodeButton;
    [SerializeField] private Button eraserButton;

    [SerializeField] private Transform pulserTR;
    [SerializeField] private Transform magnetTR;

    [SerializeField] private Image nodeImage;
    [SerializeField] private Color32 nodeColor;

    public enum CellType { pulser, magnet, node, eraser }
    private CellType currentCellType;

    private Button currentCellButton;

    private Vector2Int currentGridPosition;

    private readonly List<float> tickSpeedMultipliers = new() { .25f, .5f, 1, 2, 4 };
    private float currentTickSpeedMultiplier = 1;

    private bool isPlaying;

    private Quaternion pulserRotation = Quaternion.identity;
    private Quaternion magnetRotation;


    private void Start()
    {
        SelectEraser();

        if (PlayerPrefs.HasKey("tickSpeedMultiplier"))
            UpdateTickMultiplier(PlayerPrefs.GetFloat("tickSpeedMultiplier"));
        else
            cycleManager.ChangeTickSpeed(1);
    }

    private void Update()
    {
        if (isPlaying) return;

        bool mouseOverUI = EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButton(0) && !mouseOverUI)
            SpawnCell();
    }

    private void UpdateTickMultiplier(float newMultiplier)
    {
        //set text. Remove 0 from 0.25 and 0.5
        tickSpeedMultiplierText.text = newMultiplier.ToString().TrimStart('0') + "x";

        //reset current multiplier
        currentTickSpeedMultiplier = newMultiplier;

        //change tick speed using reciprocal
        cycleManager.ChangeTickSpeed(1 / newMultiplier);
    }

    private void SpawnCell()
    {
        //get prefab
        Cell prefToSpawn = null;
        Quaternion spawnRotation = Quaternion.identity;
        if (currentCellType == CellType.node)
            prefToSpawn = nodePref;
            //spawnRotation remains default
        else if (currentCellType == CellType.pulser)
        {
            prefToSpawn = pulserPref;
            spawnRotation = pulserRotation;
        }
        else if (currentCellType == CellType.magnet)
        {
            prefToSpawn = magnetPref;
            spawnRotation = magnetRotation;
        }
        //else remain null

        //get grid position
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        Vector3 selectedPosition = mainCamera.ScreenToWorldPoint(mousePos);
        Vector2Int gridPosition = new(RoundFloatToOddInt(selectedPosition.x), RoundFloatToOddInt(selectedPosition.y));

        //if grid position hasn't changed, return
        if (gridPosition == currentGridPosition) return;
        //else update currentGridPosition
        currentGridPosition = gridPosition;

        //if cell exists, check if rotation is necessary, then erase cell
        if (Cell.gridIndex.TryGetValue(gridPosition, out Cell cellAtPosition))
        {
            //if cell is a gadget of the selected type, rotate
            if (GadgetWillRotate(cellAtPosition))
            {
                spawnRotation *= Quaternion.Euler(0, 0, -90);

                if (currentCellType == CellType.pulser)
                {
                    pulserRotation *= Quaternion.Euler(0, 0, -90);
                    pulserTR.rotation = pulserRotation;
                }
                else //if magnet
                {
                    magnetRotation *= Quaternion.Euler(0, 0, -90);
                    magnetTR.rotation = magnetRotation;
                }
            }

            Destroy(cellAtPosition.gameObject);
            Cell.gridIndex.Remove(gridPosition);
        }

        //if eraser, return
        if (prefToSpawn == null) return;

        //spawn new cell
        Cell newCell = Instantiate(prefToSpawn, (Vector2)gridPosition, spawnRotation, cellParent);
        Cell.gridIndex.Add(gridPosition, newCell);
        if (currentCellType == CellType.node)
            newCell.sr.color = nodeColor;

        //fasten cell

    }

    private bool GadgetWillRotate(Cell cellAtSpawnPosition)
    {
        //check if cell is a gadget
        if (cellAtSpawnPosition is not Gadget gadget)
            return false;

        //check if a gadget is selected
        if (currentCellType == CellType.node || currentCellType == CellType.eraser)
            return false;

        //check if the gadget is of the selected type
        if (gadget.isPulser != (currentCellType == CellType.pulser))
            return false;

        return true;
    }

    private int RoundFloatToOddInt(float f)
    {
        return Mathf.FloorToInt(f * 0.5f) * 2 + 1;
    }

    private void SetCellButtonsInteractable(Button newUninteractableButton)
    {
        pulserButton.interactable = true;
        magnetButton.interactable = true;
        nodeButton.interactable = true;
        eraserButton.interactable = true;

        if (newUninteractableButton != null)
        {
            newUninteractableButton.interactable = false;
            currentCellButton = newUninteractableButton;
        }
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

        if (isPlaying)
        {
            pulserButton.interactable = false;
            magnetButton.interactable = false;
            nodeButton.interactable = false;
            eraserButton.interactable = false;
        }
        else
        {
            SetCellButtonsInteractable(currentCellButton);
        }

        playIcon.SetActive(!isPlaying);
        stopIcon.SetActive(isPlaying);
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
        currentCellType = CellType.pulser;
        SetCellButtonsInteractable(pulserButton);
        currentGridPosition = default;
    }

    public void SelectMagnet()
    {
        currentCellType = CellType.magnet;
        SetCellButtonsInteractable(magnetButton);
        currentGridPosition = default;
    }

    public void SelectNode()
    {
        currentCellType = CellType.node;
        SetCellButtonsInteractable(nodeButton);
        currentGridPosition = default;
    }

    public void SelectEraser()
    {
        currentCellType = CellType.eraser;
        SetCellButtonsInteractable(eraserButton);
        currentGridPosition = default;
    }

    public void NodeColor(Color32 newNodeColor)
    {
        nodeImage.color = newNodeColor;
        nodeColor = newNodeColor;
    }

    public void Tutorial()
    {

    }
}
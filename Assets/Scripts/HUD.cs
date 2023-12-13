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

    prevent gadget reversal on play

    fastening

    saves (save current loadout in playerprefs, load on start)

    play saves before playing
    stop reloads save

    save/exit popups

    tutorial (start game first time with how to play, save in playerprefs to not do again)
    */


    //SCENE REFERENCE
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

    [SerializeField] private Transform pulserButtonTR;
    [SerializeField] private Transform magnetButtonTR;

    [SerializeField] private Image nodeImage;
    [SerializeField] private Color32 nodeColor;

    //DYNAMIC
    public enum CellType { pulser, magnet, node, eraser }
    private CellType currentCellType;

    private Button currentCellButton;

    private Vector2Int currentGridPosition;

    private readonly List<float> tickSpeedMultipliers = new() { .25f, .5f, 1, 2, 4 };
    private float currentTickSpeedMultiplier = 1;

    private bool isPlaying;

    private Quaternion pulserRotation;
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

        //check if mouse is over ui
        if (EventSystem.current.IsPointerOverGameObject()) return;

        //rotate before spawning so that the spawned cell will be of the correct rotation
        if (Input.GetMouseButtonDown(0))
            RotateGadget();

        if (Input.GetMouseButton(0))
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

    private void RotateGadget()
    {
        //check if a gadget is selected
        if (currentCellType == CellType.node || currentCellType == CellType.eraser) return;

        //check if cell exists at mouse position
        Vector2Int gridPosition = MouseGridPosition();
        if (!Cell.gridIndex.TryGetValue(gridPosition, out Cell cell)) return;

        //check if cell is a gadget
        if (cell is not Gadget gadget) return;

        //check if the gadget is of the selected type
        if (gadget.isPulser != (currentCellType == CellType.pulser)) return;

        //rotate gadget, gadget button, and future spawned gadgets of the selected type
        gadget.transform.rotation *= Quaternion.Euler(0, 0, -90);

        if (gadget.isPulser)
        {
            pulserRotation = gadget.transform.rotation;
            pulserButtonTR.rotation = pulserRotation;
        }
        else //if magnet
        {
            magnetRotation = gadget.transform.rotation;
            magnetButtonTR.rotation = pulserRotation;
        }
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
        Vector2Int gridPosition = MouseGridPosition();

        //if grid position hasn't changed, return
        if (gridPosition == currentGridPosition) return;
        //else update currentGridPosition
        currentGridPosition = gridPosition;

        //if cell exists, check if rotation is necessary, then erase cell
        if (Cell.gridIndex.TryGetValue(gridPosition, out Cell cellAtPosition))
        {
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
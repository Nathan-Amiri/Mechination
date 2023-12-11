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

    list prefabs/celltypes/buttons
    buttons not interactable in play mode

    colors
    rotation

    saves

    tutorial

    save/exit popups

    play saves before playing
    stop reloads save

    fastening
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

    public enum CellType { pulser, magnet, node, eraser }
    private CellType currentCellType;

    private Vector2Int currentGridPosition;

    private readonly List<float> tickSpeedMultipliers = new() { .25f, .5f, 1, 2, 4 };
    private float currentTickSpeedMultiplier = 1;

    private bool isPlaying;


    private void Start()
    {
        RoundFloatToOddInt(1.9f);
        SelectEraser();

        if (PlayerPrefs.HasKey("tickSpeedMultiplier"))
            UpdateTickMultiplier(PlayerPrefs.GetFloat("tickSpeedMultiplier"));
        else
            cycleManager.ChangeTickSpeed(1);
    }

    private void Update()
    {
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
        if (currentCellType == CellType.pulser)
            prefToSpawn = pulserPref;
        else if (currentCellType == CellType.magnet)
            prefToSpawn = magnetPref;
        else if (currentCellType == CellType.node)
            prefToSpawn = nodePref;
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

        //erase cell if it exists
        if (Cell.gridIndex.TryGetValue(gridPosition, out Cell cellAtPosition))
        {
            Destroy(cellAtPosition.gameObject);
            Cell.gridIndex.Remove(gridPosition);
        }

        //if eraser, return
        if (prefToSpawn == null) return;

        //spawn new cell
        Cell newCell = Instantiate(prefToSpawn, (Vector2)gridPosition, Quaternion.identity, cellParent);
        Cell.gridIndex.Add(gridPosition, newCell);
    }

    public int RoundFloatToOddInt(float f)
    {
        int roundTowardZero = (int)f;
        //if roundTowardZero is even, increase magnitude by 1. Else, return roundTowardZero
        if (roundTowardZero % 2 == 0)
            return roundTowardZero + (int)Mathf.Sign(f);
        else
            return roundTowardZero;
    }

    private void SetGadgetButtonsInteractable(Button newUninteractableButton)
    {
        pulserButton.interactable = true;
        magnetButton.interactable = true;
        nodeButton.interactable = true;
        eraserButton.interactable = true;

        if (newUninteractableButton != null)
            newUninteractableButton.interactable = false;
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
        //make buttons, celltypes, and prefabs into list that can be iterated through


        if (!isPlaying)
            SetGadgetButtonsInteractable(null);

        cycleManager.StartStopCycle(!isPlaying);

        isPlaying = !isPlaying;

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
        SetGadgetButtonsInteractable(pulserButton);
        currentGridPosition = default;
    }

    public void SelectMagnet()
    {
        currentCellType = CellType.magnet;
        SetGadgetButtonsInteractable(magnetButton);
        currentGridPosition = default;
    }

    public void SelectNode()
    {
        currentCellType = CellType.node;
        SetGadgetButtonsInteractable(nodeButton);
        currentGridPosition = default;
    }

    public void SelectEraser()
    {
        currentCellType = CellType.eraser;
        SetGadgetButtonsInteractable(eraserButton);
        currentGridPosition = default;
    }

    public void NodeColor(int color)
    {

    }

    public void Tutorial()
    {

    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditModeManager : MonoBehaviour
{
    // SCENE REFERENCE
    [SerializeField] private Cell pulserPref;
    [SerializeField] private Cell magnetPref;
    [SerializeField] private Cell nodePref;

    [SerializeField] private PlayModeManager playModeManager;
    [SerializeField] private SaveAndLoad saveAndLoad;
    [SerializeField] private GameAudio gameAudio;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform cellParent;

    [SerializeField] private GameObject playIcon;
    [SerializeField] private GameObject stopIcon;

    [SerializeField] private GameObject eraserIcon;
    [SerializeField] private GameObject clearIcon;

    [SerializeField] private TMP_Text tickSpeedMultiplierText;

    [SerializeField] private Button saveButton;

    [SerializeField] private List<Button> saveFileButtons;

    [SerializeField] private Button pulserButton;
    [SerializeField] private Button magnetButton;
    [SerializeField] private Button nodeButton;
    [SerializeField] private Button eraserButton;

    [SerializeField] private Transform pulserButtonTR;
    [SerializeField] private Transform magnetButtonTR;

    [SerializeField] private Image nodeImage;
    [SerializeField] private List<Color32> nodeColors;

    [SerializeField] private List<Button> nodeColorButtons;

    [SerializeField] private GameObject warning;
    [SerializeField] private TMP_Text warningText;

    [SerializeField] private GameObject openTutorialMessage;
    [SerializeField] private GameObject tutorialScreen;
    [SerializeField] private GameObject tutorialPage1;
    [SerializeField] private GameObject tutorialPage2;
    [SerializeField] private Button tutorialBackButton;
    [SerializeField] private GameObject tutorialNextButton;
    [SerializeField] private GameObject tutorialFinishButton;

    // DYNAMIC
    public enum SpawnType { pulser, magnet, node, eraser }
    private SpawnType currentSpawnType;

    private int pulserZRotation;
    private int magnetZRotation;

    private Button currentCellButton;

    private bool isPlaying;

    private readonly List<float> tickSpeedMultipliers = new() { .25f, .5f, 1, 2, 4 };
    private float currentTickSpeedMultiplier = 1;

    private bool layoutSaved = true;

    private int currentLayoutNumber;

    private int currentNodeColorNumber;

    private bool currentlyErasing;

    public delegate void WarningDelegate();
    private WarningDelegate warningDelegate;

        // Cached when warning is being displayed after selecting a new layout before saving
    private int newLayoutNumber;


    private void Start()
    {
        SelectEraserClear();

        if (PlayerPrefs.HasKey("tickSpeedMultiplier"))
            UpdateTickMultiplier(PlayerPrefs.GetFloat("tickSpeedMultiplier"));
        else
            playModeManager.SetTickSpeed(1);

        if (PlayerPrefs.HasKey("currentLayoutNumber"))
            SelectLoadSaveFile(PlayerPrefs.GetInt("currentLayoutNumber"));
        else
            SelectLoadSaveFile(0);

        if (!PlayerPrefs.HasKey("TutorialOpened"))
            openTutorialMessage.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(gameAudio.PlayShortcut());
            SelectExit();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(gameAudio.PlayShortcut());
            SelectPlayStop();
        }

        if (isPlaying) return;

        if (Input.GetKeyDown(KeyCode.S) && saveButton.interactable)
        {
            StartCoroutine(gameAudio.PlayShortcut());
            SelectSave();
        }
        if (Input.GetKeyDown(KeyCode.Q) && pulserButton.interactable)
        {
            StartCoroutine(gameAudio.PlayShortcut());
            SelectPulser();
        }
        if (Input.GetKeyDown(KeyCode.W) && magnetButton.interactable)
        {
            StartCoroutine(gameAudio.PlayShortcut());
            SelectMagnet();
        }
        if (Input.GetKeyDown(KeyCode.E) && nodeButton.interactable)
        {
            StartCoroutine(gameAudio.PlayShortcut());
            SelectNode();
        }
        if (Input.GetKeyDown(KeyCode.R) && eraserButton.interactable)
        {
            StartCoroutine(gameAudio.PlayShortcut());
            SelectEraserClear();
        }

        // Check if mouse is over UI
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Rotate before spawning so that the spawned cell will be of the correct rotation
        if (Input.GetMouseButtonDown(0))
            RotateGadget();

        if (Input.GetMouseButton(0))
            PrepareToSpawnCell();
    }

    private void RotateGadget()
    {
        // Check if a gadget is selected
        if (currentSpawnType == SpawnType.node || currentSpawnType == SpawnType.eraser) return;

        // Check if cell exists at mouse position
        Vector2Int gridPosition = MouseGridPosition();
        if (!Cell.gridIndex.TryGetValue(gridPosition, out Cell cell)) return;

        // Check if cell is a gadget
        if (cell is not Gadget gadget) return;

        // Check if the gadget is of the selected type
        if (gadget.isPulser != (currentSpawnType == SpawnType.pulser)) return;
        StartCoroutine(

                // Rotate gadget, gadget button, and future spawned gadgets of the selected type
                gameAudio.PlayCellRotate());
        gadget.transform.rotation *= Quaternion.Euler(0, 0, -90);
        gadget.gadgetDirection = Vector2Int.RoundToInt(gadget.transform.up);

        if (gadget.isPulser)
        {
            pulserZRotation = Mathf.RoundToInt(gadget.transform.rotation.eulerAngles.z);
            pulserButtonTR.rotation = gadget.transform.rotation;
        }
        else // If magnet
        {
            magnetZRotation = Mathf.RoundToInt(gadget.transform.rotation.eulerAngles.z);
            magnetButtonTR.rotation = gadget.transform.rotation;
        }

        // After rotating, layout has changed
        UpdateLayoutSaved(false);

        // Unfasten and refasten gadget after rotating
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
        // For eraser and node, remain default

        // Get grid position
        Vector2Int gridPosition = MouseGridPosition();

        // If cell exists, return if we aren't erasing and the cell is identical to the cell that would be spawned
        // Else, erase existing cell
        if (Cell.gridIndex.TryGetValue(gridPosition, out Cell cellAtPosition))
        {
            if (currentSpawnType != SpawnType.eraser && CellAlreadySpawned(cellAtPosition)) return;
            StartCoroutine(gameAudio.PlayCellPlaceErase());
            DespawnCell(cellAtPosition, gridPosition, true);
            // After erasing, layout has changed
            UpdateLayoutSaved(false);
        }

        if (currentSpawnType == SpawnType.eraser) return;

        // Spawn new cell
        SpawnCell(newCellType, gridPosition, spawnRotation, currentNodeColorNumber, true);
        // After spawning, layout has changed
        UpdateLayoutSaved(false);
    }
    
    // Called by PrepareToSpawnCell and SaveAndLoad's LoadLayout
    public void SpawnCell(int newCellType, Vector2Int gridPosition, Quaternion spawnRotation, int nodeColorNumber, bool playSpawnAudio)
    {
        if (playSpawnAudio)
            StartCoroutine(gameAudio.PlayCellPlaceErase());

        Cell prefToSpawn = nodePref;
        if (newCellType != 0)
            prefToSpawn = newCellType == 1 ? pulserPref : magnetPref;

        Cell newCell = Instantiate(prefToSpawn, (Vector2)gridPosition, spawnRotation, cellParent);
        Cell.gridIndex.Add(gridPosition, newCell);
        newCell.currentPosition = gridPosition;
        // Used for layout saving
        newCell.cellType = newCellType;

        if (newCell is Gadget newGadget)
            // Must set gadgetDirection before fastening
            newGadget.gadgetDirection = Vector2Int.RoundToInt(newGadget.transform.up);
        else // If node
        {
            newCell.sr.color = nodeColors[nodeColorNumber];
            newCell.nodeColorNumber = nodeColorNumber;
        }

        // Fasten cell
        newCell.FastenCell();
    }

    private bool CellAlreadySpawned(Cell cell)
    {
        // Returns true if the cell is identical to the cell that would be spawned

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
        // If clearing grid, cell can't be unfastened since fasteners are removed based on
        // current position. Also, there's no need because they will be destroyed when their
        // parented cells are despawned
        if (unfastenCell)
            cell.UnFastenCell();

        Destroy(cell.gameObject);
        Cell.gridIndex.Remove(cellPosition);
    }

    private Vector2Int MouseGridPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        Vector3 mousePositionOnGrid = mainCamera.ScreenToWorldPoint(mousePos);

        return new(RoundFloatToOddInt(mousePositionOnGrid.x), RoundFloatToOddInt(mousePositionOnGrid.y));
    }

    private int RoundFloatToOddInt(float f)
    {
        return Mathf.FloorToInt(f * 0.5f) * 2 + 1;
    }

    public void SelectExit()
    {
        if (layoutSaved)
            ConfirmExit();
        else
        {
            ToggleWarningMessage(true);
            warningDelegate = ConfirmExit;
            warning.SetActive(true);
        }
    }
    private void ConfirmExit()
    {
        Application.Quit();
    }

    public void SelectSave()
    {
        saveAndLoad.SaveLayout(currentLayoutNumber);
        UpdateLayoutSaved(true);
    }
    private void UpdateLayoutSaved(bool saved)
    {
        layoutSaved = saved;

        saveButton.interactable = !layoutSaved;
    }

    public void SelectLoadSaveFile(int saveFile)
    {
        // Don't set currentLayoutNumber until confirmed
        newLayoutNumber = saveFile;

        if (layoutSaved)
            ConfirmLoadSaveFile();
        else
        {
            ToggleWarningMessage(true);
            warningDelegate = ConfirmLoadSaveFile;
            warning.SetActive(true);
        }
    }
    private void ConfirmLoadSaveFile()
    {
        currentLayoutNumber = newLayoutNumber;

        saveAndLoad.LoadLayout(currentLayoutNumber);

        PlayerPrefs.SetInt("currentLayoutNumber", currentLayoutNumber);

        for (int i = 0; i < saveFileButtons.Count; i++)
            saveFileButtons[i].interactable = i != currentLayoutNumber;

        UpdateLayoutSaved(true);
    }

    public void SelectPlayStop()
    {
        isPlaying = !isPlaying;

        // If stopping, stop cycle, clear, load layout, then adjust interactable in that order
        if (!isPlaying)
        {
            playModeManager.StartStopCycle(false);

            saveAndLoad.LoadLayout(currentLayoutNumber);
        }

        // Adjust interactable
        SetCellButtonsInteractable(currentCellButton, isPlaying);

        for (int i = 0; i < saveFileButtons.Count; i++)
            saveFileButtons[i].interactable = !isPlaying && currentLayoutNumber != i;
        for (int i = 0; i < nodeColorButtons.Count; i++)
            nodeColorButtons[i].interactable = !isPlaying && currentNodeColorNumber != i;

        playIcon.SetActive(!isPlaying);
        stopIcon.SetActive(isPlaying);

        // If playing, adjust interactable, save layout, then start cycle in that order
        if (isPlaying)
        {
            SelectSave();

            playModeManager.StartStopCycle(true);
        }
    }
    private void SetCellButtonsInteractable(Button newUninteractableButton, bool disableAll = false)
    {
        pulserButton.interactable = !disableAll;
        magnetButton.interactable = !disableAll;
        nodeButton.interactable = !disableAll;
        eraserButton.interactable = !disableAll;

        if (newUninteractableButton != null)
            newUninteractableButton.interactable = false;

        // Even if null
        currentCellButton = newUninteractableButton;
    }

    public void SelectTickMultipler()
    {
        // If current multiplier isn't found in the first 4 multipliers, then it's 4, so it remains at .25f
        float newMultiplier = .25f;
        for (int i = 0; i < tickSpeedMultipliers.Count - 1; i++)
            if (currentTickSpeedMultiplier == tickSpeedMultipliers[i])
                newMultiplier = tickSpeedMultipliers[i + 1];

        PlayerPrefs.SetFloat("tickSpeedMultiplier", newMultiplier);

        UpdateTickMultiplier(newMultiplier);
    }
    private void UpdateTickMultiplier(float newMultiplier)
    {
        // Set text. Remove 0 from 0.25 and 0.5
        tickSpeedMultiplierText.text = newMultiplier.ToString().TrimStart('0') + "x";

        // Reset current multiplier
        currentTickSpeedMultiplier = newMultiplier;

        // Change tick speed
        playModeManager.SetTickSpeed(newMultiplier);
    }

    public void SelectPulser()
    {
        currentSpawnType = SpawnType.pulser;
        SetCellButtonsInteractable(pulserButton);

        ToggleErasing(false);
    }

    public void SelectMagnet()
    {
        currentSpawnType = SpawnType.magnet;
        SetCellButtonsInteractable(magnetButton);

        ToggleErasing(false);
    }

    public void SelectNode()
    {
        currentSpawnType = SpawnType.node;
        SetCellButtonsInteractable(nodeButton);

        ToggleErasing(false);
    }

    public void SelectEraserClear()
    {
        currentlyErasing = !currentlyErasing;

        if (currentlyErasing)
        {
            currentSpawnType = SpawnType.eraser;
            // Turn on all buttons
            SetCellButtonsInteractable(null);

            ToggleErasing(true);
        }
        else
        {
            ToggleWarningMessage(false);

            warningDelegate = ClearConfirmed;
            warning.SetActive(true);

            // Continue erasing whether confirmed or not
            currentlyErasing = true;
        }
    }
    // Run after warning message confirmed the clear
    private void ClearConfirmed()
    {
        ClearGrid();

        // After clearing, layout has changed
        UpdateLayoutSaved(false);
    }
    // Called by SelectEraserClear and SaveAndLoad's LoadLayout;
    public void ClearGrid()
    {
        // Must despawn outside foreach loop since despawning modifies the dict
        List<KeyValuePair<Vector2Int, Cell>> pairs = new();
        foreach (KeyValuePair<Vector2Int, Cell> pair in Cell.gridIndex)
            pairs.Add(pair);

        foreach(KeyValuePair<Vector2Int, Cell> pair in pairs)
            DespawnCell(pair.Value, pair.Key, false);

        // Fasteners have already been destroyed due to cell despawning,
        // but fastenedCells and fastenerIndex need to be reset
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

        for (int i = 0; i < nodeColorButtons.Count; i++)
            nodeColorButtons[i].interactable = i != currentNodeColorNumber;

        SelectNode();
    }

    public void SelectCancel()
    {
        warning.SetActive(false);
    }

    public void SelectProceed()
    {
        warning.SetActive(false);

        warningDelegate();
    }

    private void ToggleWarningMessage(bool unsavedWarning)
    {
        warningText.text = unsavedWarning ? "You have unsaved changes!" : "The entire grid will be erased!";
    }

    public void SelectTutorial()
    {
        // Keep tutorial button interactable when playing in case a new user hits play by accident, then gets confused
        if (isPlaying)
            SelectPlayStop();

        if (openTutorialMessage.activeSelf)
        {
            openTutorialMessage.SetActive(false);
            // Value doesn't matter -- if key exists, it won't display again
            PlayerPrefs.SetInt("TutorialOpened", 0);
        }

        tutorialScreen.SetActive(true);
    }
    public void SelectTutorialBack()
    {
        tutorialBackButton.interactable = false;
        tutorialFinishButton.SetActive(false);
        tutorialNextButton.SetActive(true);

        tutorialPage1.SetActive(true);
        tutorialPage2.SetActive(false);
    }
    public void SelectTutorialNext()
    {
        tutorialBackButton.interactable = true;
        tutorialNextButton.SetActive(false);
        tutorialFinishButton.SetActive(true);

        tutorialPage1.SetActive(false);
        tutorialPage2.SetActive(true);
    }
    public void SelectTutorialFinish()
    {
        // Reset pages/buttons before closing
        SelectTutorialBack();

        tutorialScreen.SetActive(false);
    }
}
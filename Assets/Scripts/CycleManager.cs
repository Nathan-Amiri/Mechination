using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleManager : MonoBehaviour
{
    //static:
    public delegate void FillGridIndexAction();
    public static FillGridIndexAction FillGridIndex;

    public delegate void FastenCellsAction();
    public static FastenCellsAction FastenCells;

    public delegate void ReversePrepareGadgetsAction();
    public static ReversePrepareGadgetsAction ReversePrepareGadgets;

    public static float TickSpeed { get; private set; }

    public static List<Gadget> preparedGadgets = new();

        //positionSafety entry is set for each position a cell prepares to move into. If
        //another cell attempts to move into a position another cell has claimed, position
        //is declared unsafe
    public static Dictionary<Vector2Int, PositionSafetyInfo> positionSafety = new();

    //public:
        //set by HUD
    [NonSerialized] public float tickSpeedMultipler;

    //readonly:
    private readonly float defaultTickSpeed = .5f;

    //dynamic:
    private Coroutine cycleRoutine;

    private void Start()
    {
        //first, all cells place their positions in the gridIndex
        FillGridIndex?.Invoke();

        //then, all cells form attachments
        FastenCells?.Invoke();

        //LATER, REMOVE ABOVE EVENTS
    }

    private IEnumerator StartCycle()
    {
        while (true)
        {
            Cycle();
            yield return new WaitForSeconds(TickSpeed);
        }
    }

    //run once per 'gamemanager tick'
    private void Cycle()
    {
        //step 1: reverse, then prepare gadgets
        ReversePrepareGadgets?.Invoke();

        //step 2: gadgets check fail conditions, then activate
        foreach (Gadget preparedGadget in preparedGadgets)
            preparedGadget.ActivateGadget();

        //step 3: reset for next cycle (all gadgets must check fails/activate before any reset)
        foreach (Gadget preparedGadget in preparedGadgets)
            preparedGadget.ResetAfterCycle();

        preparedGadgets.Clear();
        positionSafety.Clear();
    }

    //called by HUD
    public void ChangeTickSpeed(float newTickSpeedMultipler)
    {
        TickSpeed = defaultTickSpeed * newTickSpeedMultipler;
    }

    //called by HUD
    public void StartStopCycle(bool start)
    {
        if (start)
            cycleRoutine = StartCoroutine(StartCycle());
        else
            StopCoroutine(cycleRoutine);
    }
}
public struct PositionSafetyInfo
{
    public Cell cellClaimingPosition;
    public bool positionUnsafe;
}
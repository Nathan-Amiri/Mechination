using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //static:
    public delegate void FillGridIndexAction();
    public static FillGridIndexAction FillGridIndex;

    public delegate void FastenCellsAction();
    public static FastenCellsAction FastenCells;

    public delegate void PrepareGadgetsAction();
    public static PrepareGadgetsAction PrepareGadgets;

    public static float TickSpeed { get; private set; }

    public static Vector2Int GridSize { get; private set; }

    public static List<Gadget> preparedGadgets = new();

        //positionSafety entry is set for each position a cell prepares to move into. If
        //another cell attempts to move into a position another cell has claimed, position
        //is declared unsafe
    public static Dictionary<Vector2Int, PositionSafetyInfo> positionSafety = new();

    //readonly:
    private readonly float startDelay = .25f;

    private void Start()
    {
        TickSpeed = .5f; //remove this later
        GridSize = new Vector2Int(100, 100); //remove this later

        //first, all cells place their positions in the gridIndex
        FillGridIndex?.Invoke();

        //then, all cells form attachments
        FastenCells?.Invoke();

        //LATER, REMOVE ABOVE EVENTS

        //finally, begin cycles
        StartCoroutine(StartCycle());
    }

    private IEnumerator StartCycle()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            Cycle();
            yield return new WaitForSeconds(TickSpeed);
        }
    }

    //run once per 'gamemanager tick'
    private void Cycle()
    {
        //must finish each step before proceeding to the next

        //step :. prepare gadgets
        PrepareGadgets?.Invoke();

        //step 2: gadgets check fail conditions, then activate
        foreach (Gadget preparedGadget in preparedGadgets)
            preparedGadget.ActivateGadget();

        //step 3: transform gadgets

        //step 4: reset for next cycle
        foreach (Gadget preparedGadget in preparedGadgets)
            preparedGadget.ResetAfterCycle();

        preparedGadgets.Clear();
        positionSafety.Clear();
    }

}
public struct PositionSafetyInfo
{
    public Cell cellClaimingPosition;
    public bool positionUnsafe;
}
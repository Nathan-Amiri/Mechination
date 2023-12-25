using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayModeManager : MonoBehaviour
{
    // STATIC:
    public delegate void OnPlayAction();
    public static OnPlayAction OnPlay;

    public delegate void ReversePrepareGadgetsAction();
    public static ReversePrepareGadgetsAction ReversePrepareGadgets;

    public static float TickSpeed { get; private set; }

    public static List<Gadget> preparedGadgets = new();

        // PositionSafety entry is set for each position a cell prepares to move into. If
        // another cell attempts to move into a position another cell has claimed, position
        // is declared unsafe
    public static Dictionary<Vector2Int, PositionSafetyInfo> positionSafety = new();

    // CONSTANT:
    private readonly float defaultTickSpeed = .5f;

    // DYNAMIC:
    [NonSerialized] public float tickSpeedMultipler;

    private Coroutine cycleRoutine;

    private IEnumerator Cycle()
    {
        OnPlay?.Invoke();

        while (true)
        {
            CycleTick();
            yield return new WaitForSeconds(TickSpeed);
        }
    }

    private void CycleTick()
    {
        // Step 1: reverse, then prepare gadgets
        ReversePrepareGadgets?.Invoke();

        // Step 2: gadgets check fail conditions, then activate
        foreach (Gadget preparedGadget in preparedGadgets)
            preparedGadget.ActivateGadget();

        // Step 3: reset for next cycle (all gadgets must check fails/activate before any reset)
        foreach (Gadget preparedGadget in preparedGadgets)
            preparedGadget.ResetAfterCycle();

        preparedGadgets.Clear();
        positionSafety.Clear();
    }

    // Called by HUD
    public void SetTickSpeed(float newTickSpeedMultipler)
    {
       // Change tick speed using reciprocal
       TickSpeed = defaultTickSpeed * (1 / newTickSpeedMultipler);
    }

    // Called by HUD
    public void StartStopCycle(bool start)
    {
        if (start)
            cycleRoutine = StartCoroutine(Cycle());
        else
            StopCoroutine(cycleRoutine);
    }
}
public struct PositionSafetyInfo
{
    public Cell cellClaimingPosition;
    public bool positionUnsafe;
}
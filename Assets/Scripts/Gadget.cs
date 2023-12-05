using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : Cell
{
    //assigned in prefab:
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color32 pulserColor;
    [SerializeField] private Color32 magnetColor;

    //readonly:
    [NonSerialized] public readonly List<Cell> movingCells = new();

    //dynamic:
    [SerializeField] private bool isPulser; //else is magnet

    [NonSerialized] public Vector2Int gadgetDirection;

    [NonSerialized] public List<Cell> adjacentNodes = new();

    protected new void OnEnable()
    {
        base.OnEnable();

        GameManager.ReversePrepareGadgets += ReverseGadget;
    }

    protected new void OnDisable()
    {
        base.OnDisable();

        GameManager.ReversePrepareGadgets -= ReverseGadget;
    }

    protected new void Awake()
    {
        base.Awake();

        gadgetDirection = Vector2Int.RoundToInt(transform.up); //replace this later!
    }

    protected new void FastenCells()
    {
        base.FastenCells();

        
    }

    private void ReverseGadget()
    {
        //compare current adjacent nodes and previous ones
        List<Cell> currentAdjacentCells = new();
        foreach (Vector2Int direction in directions)
        {
            //don't reverse if node is in front of gadget
            if (direction == gadgetDirection) continue;

            //continue if there's no cell in this direction or if it's not a node
            if (!gridIndex.TryGetValue(currentPosition + direction * 2, out Cell adjacentCell)) continue;
            if (adjacentCell is Gadget) continue;

            currentAdjacentCells.Add(adjacentCell);
        }

        //if any nodes are adjacent that weren't last tick, reverse
        foreach (Cell cell in currentAdjacentCells)
            if (!adjacentNodes.Contains(cell))
            {
                isPulser = !isPulser;
                sr.color = isPulser ? pulserColor : magnetColor;

                break;
            }

        adjacentNodes = currentAdjacentCells;

        //prepare gadget
        if (isPulser)
            PreparePulser();
        else
            PrepareMagnet();
    }

    private void PreparePulser()
    {
        //check for cell directly in front of this one
        Vector2Int frontPosition = currentPosition + gadgetDirection * 2;
        if (!gridIndex.TryGetValue(frontPosition, out Cell frontCell)) return;

        //get all moving cells
        movingCells.Clear();
        frontCell.GetMovingCell(this, gadgetDirection);

        //add to prepared gadgets
        GameManager.preparedGadgets.Add(this);
    }

    private void PrepareMagnet()
    {
        //check for cell blocking magnet (directly in front of this one)
        Vector2Int frontPosition = currentPosition + gadgetDirection * 2;
        if (gridIndex.ContainsKey(frontPosition)) return;

        //check for cell in the position 2 spaces in front of this one
        Vector2Int targetPosition = currentPosition + gadgetDirection * 4;
        if (!gridIndex.TryGetValue(targetPosition, out Cell targetCell)) return;

        //get all moving cells
        movingCells.Clear();
        targetCell.GetMovingCell(this, -gadgetDirection);

        //add to prepared gadgets
        GameManager.preparedGadgets.Add(this);
    }

    public void ActivateGadget()
    {
        //check for fails, then move gadget
        //note: can't check gridIndex or a cell's currentPosition in this method since both are updated
        //for a cell once a cell starts to move

        Vector2Int moveDirection = isPulser ? gadgetDirection : -gadgetDirection;

        foreach (Cell cell in movingCells)
        {
            //fail checks ordered by performance cost

            //fail 1: tearing cell (preparing to move cell in multiple directions)
            if (cell.tearFail) return;

            //fail 2: cell is a gadget that's preparing to activate
            if (cell is Gadget gadget && gadget.movingCells.Count > 0)
                return;

            //fail 3: moving cell off grid
            float maxMoveDistanceFromOrigin;
            float targetPositionDistanceFromOrigin;
                //if moving along the y axis
            if (moveDirection.x == 0)
            {
                //maxMoveDistanceFrom Origin = half of GridSize.y times the grid's scale, which is 2 so it cancels out
                maxMoveDistanceFromOrigin = GameManager.GridSize.y;
                targetPositionDistanceFromOrigin = cell.preparedMovePosition.y;
            }
                //if moving along the x axis
            else
            {
                //maxMoveDistanceFrom Origin = half of GridSize.x times the grid's scale, which is 2 so it cancels out
                maxMoveDistanceFromOrigin = GameManager.GridSize.x;
                targetPositionDistanceFromOrigin = cell.preparedMovePosition.x;
            }

            if (Mathf.Abs(targetPositionDistanceFromOrigin) > Mathf.Abs(maxMoveDistanceFromOrigin))
                return;

            //fail 4: cell is moving into an unsafe space (a space other cell(s) are preparing to move into)
            if (!GameManager.positionSafety.TryGetValue(cell.preparedMovePosition, out PositionSafetyInfo positionSafetyInfo))
                Debug.LogError("position not found in positionSafe");
            else if (positionSafetyInfo.positionUnsafe == true)
                return;
        }

        //move cells
        foreach (Cell cell in movingCells)
            StartCoroutine(cell.LerpMovement());
    }

    public void ResetAfterCycle()
    {
        foreach (Cell cell in movingCells)
            cell.CellReset();

        movingCells.Clear();
    }
}
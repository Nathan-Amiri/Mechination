using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : Cell
{
    //PREFAB REFERENCE:
    [SerializeField] private SpriteRenderer cellSr;
    [SerializeField] private SpriteRenderer iconSr;
    [SerializeField] private Color32 pulserColor;
    [SerializeField] private Color32 magnetColor;
    [SerializeField] private Sprite pulserSprite;
    [SerializeField] private Sprite magnetSprite;

    //CONSTANT:
    [NonSerialized] public readonly List<Cell> movingCells = new();

    //DYNAMIC:
        //if false, is magnet
    public bool isPulser;

    [NonSerialized] public Vector2Int gadgetDirection;

    [NonSerialized] public List<Cell> adjacentNodes = new();

    protected void OnEnable()
    {
        CycleManager.ReversePrepareGadgets += ReverseGadget;
        CycleManager.OnPlay += OnPlay;
    }

    protected void OnDisable()
    {
        CycleManager.ReversePrepareGadgets -= ReverseGadget;
        CycleManager.OnPlay -= OnPlay;
    }

    private List<Cell> GetAdjacentNodes()
    {
        List<Cell> currentAdjacentNodes = new();
        foreach (Vector2Int direction in directions)
        {
            //only check for nodes behind and to the side, not in front
            if (direction == gadgetDirection) continue;

            //continue if there's no cell in this direction or if it's not a node
            if (!gridIndex.TryGetValue(currentPosition + direction * 2, out Cell adjacentCell)) continue;
            if (adjacentCell is Gadget) continue;

            currentAdjacentNodes.Add(adjacentCell);
        }
        return currentAdjacentNodes;
    }

    public void OnPlay()
    {
        adjacentNodes = GetAdjacentNodes();
    }

    private void ReverseGadget()
    {
        List<Cell> currentAdjacentNodes = GetAdjacentNodes();

        //if any nodes are adjacent that weren't last tick, reverse
        foreach (Cell node in currentAdjacentNodes)
            if (!adjacentNodes.Contains(node))
            {
                isPulser = !isPulser;
                cellSr.color = isPulser ? pulserColor : magnetColor;
                iconSr.sprite = isPulser ? pulserSprite : magnetSprite;

                break;
            }

        adjacentNodes = currentAdjacentNodes;

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
        CycleManager.preparedGadgets.Add(this);
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
        CycleManager.preparedGadgets.Add(this);
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
            //maxMoveDistanceFrom Origin = half of the grid's length (1000) x the grid's scale (2) = 1000
            float maxMoveDistanceFromOrigin = 1000;

            float targetPositionDistanceFromOrigin;
                //if moving along the y axis
            if (moveDirection.x == 0)
                targetPositionDistanceFromOrigin = cell.preparedMovePosition.y;
                //if moving along the x axis
            else
                targetPositionDistanceFromOrigin = cell.preparedMovePosition.x;

            if (Mathf.Abs(targetPositionDistanceFromOrigin) > Mathf.Abs(maxMoveDistanceFromOrigin))
                return;

            //fail 4: cell is moving into an unsafe space (a space other cell(s) are preparing to move into)
            if (!CycleManager.positionSafety.TryGetValue(cell.preparedMovePosition, out PositionSafetyInfo positionSafetyInfo))
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
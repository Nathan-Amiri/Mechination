using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    //static:
    public static Dictionary<Vector2Int, Cell> gridIndex = new();

        //key = a cell with fastenings, value = list of cells the key cell is fastened to
    public static Dictionary<Cell, List<Cell>> fastenedCells = new();

    //assigned in prefab:
    [SerializeField] private GameObject fastener;

    //readonly:
    private readonly List<Vector2Int> directions = new()
    {
        Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down
    };

    //dynamic:
        //zero unless this cell is a gadget, set by Gadget.
    protected Vector2Int gadgetDirection = Vector2Int.zero;

    [NonSerialized] public Vector2Int currentPosition;

    protected void OnEnable()
    {
        GameManager.FillGridIndex += FillGridIndex;
        GameManager.FastenCells += FastenCells;
    }
    protected void OnDisable()
    {
        GameManager.FillGridIndex -= FillGridIndex;
        GameManager.FastenCells -= FastenCells;
    }

    protected void Awake() //before GameManager's Awake (temporary method!)
    {
        currentPosition = Vector2Int.RoundToInt(transform.position); //replace this later!
    }

    //called by GameManager at the start of the game
    private void FillGridIndex()
    {
        gridIndex.Add(currentPosition, this);
    }

    //called by GameManager at the start of the game, after gridindex is filled
    private void FastenCells()
    {
        List<Cell> cellsToFasten = new();
        foreach (Vector2Int direction in directions)
        {
            //check if this cell is a gadget and it's facing this direction
            if (gadgetDirection == direction) continue;

            //check if there's an adjacent cell in this direction
            if (!gridIndex.TryGetValue(currentPosition + direction * 2, out Cell adjacentCell)) continue;

            //check if the adjacent cell is a gadget facing this cell
            if (adjacentCell.gadgetDirection * 2 == currentPosition - adjacentCell.currentPosition) continue;

            //if adjacent cell isn't in static dictionary (meaning it's already created its fastenings), create fastener sprite
            if (!fastenedCells.ContainsKey(adjacentCell))
            {
                GameObject fastenerObject = Instantiate(fastener, transform);
                //fastener position = current position + direction * .5 * grid scale, which is 2 so it cancels out
                fastenerObject.transform.position = (Vector2)currentPosition + (Vector2)direction;
                //if direction is vertical, rotate fastener
                //multiply rotation by this cell's rotation in case this cell (the fastener's parent) is a gadget rotated 90 or 270 degrees
                if (direction.x == 0)
                    fastenerObject.transform.rotation *= Quaternion.Euler(0, 0, 90) * transform.rotation;
            }

            //cache adjacent cell
            cellsToFasten.Add(adjacentCell);
        }

        //if this cell has fastenings, save to static dictionary
        if (cellsToFasten.Count > 0)
            fastenedCells.Add(this, cellsToFasten);
    }

    public void GetMovingCell(Gadget movingGadget, Vector2Int moveDirection)
    {
        //add this moving cell to gadget's movingCells
        movingGadget.movingCells.Add(this);

        //if there are cells fastened to this one, add each one if it isn't already moving
        if (fastenedCells.TryGetValue(this, out List<Cell> myFastenedCells))
            foreach (Cell fastenedCell in myFastenedCells)
                if (!movingGadget.movingCells.Contains(fastenedCell))
                    fastenedCell.GetMovingCell(movingGadget, moveDirection);

        //if there's a cell directly in front of this cell, add it if it isn't already moving
        Vector2Int frontPosition = currentPosition + moveDirection * 2;
        if (gridIndex.TryGetValue(frontPosition, out Cell frontCell))
            if (!movingGadget.movingCells.Contains(frontCell))
                frontCell.GetMovingCell(movingGadget, moveDirection);
    }
}
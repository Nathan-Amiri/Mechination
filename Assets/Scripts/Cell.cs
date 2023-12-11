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
    protected readonly List<Vector2Int> directions = new()
    {
        Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down
    };

    //dynamic:

    [NonSerialized] public Vector2Int preparedMovePosition;
        //true when this cell is preparing to move into more than one position
    [NonSerialized] public bool tearFail;

    [NonSerialized] public Vector2Int currentPosition;

    protected void OnEnable()
    {
        CycleManager.FillGridIndex += FillGridIndex;
        CycleManager.FastenCells += FastenCells;
    }
    protected void OnDisable()
    {
        CycleManager.FillGridIndex -= FillGridIndex;
        CycleManager.FastenCells -= FastenCells;
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
            //check if there's an adjacent cell in this direction
            if (!gridIndex.TryGetValue(currentPosition + direction * 2, out Cell adjacentCell)) continue;

            if (this is Gadget thisGadget)
            {
                //check if this cell is a gadget and it's facing this direction
                if (thisGadget.gadgetDirection == direction) continue;

                //if this cell is a gadget and adjacent cell is not, add it to this gadget's adjacentNodes
                //so that this cell doesn't reverse when the simulation starts
                else if (adjacentCell is not Gadget)
                    thisGadget.adjacentNodes.Add(adjacentCell);
            }

            //check if the adjacent cell is a gadget facing this cell
            if (adjacentCell is Gadget adjacentGadget && adjacentGadget.gadgetDirection * 2 == currentPosition - adjacentCell.currentPosition) continue;

            //if adjacent cell isn't in static dictionary (meaning it's already created its fastenings), create fastener sprite
            if (!fastenedCells.ContainsKey(adjacentCell))
            {
                GameObject fastenerObject = Instantiate(fastener, transform);

                //fastener position = current position + direction * .5 * grid scale, which is 2 so it cancels out
                fastenerObject.transform.position = (Vector2)currentPosition + (Vector2)direction;

                //if this is a horizontal gadget, rotate fastener if direction is horizontal
                if (this is Gadget horizontalGadget && horizontalGadget.gadgetDirection.y == 0)
                {
                    if (direction.y == 0)
                        fastenerObject.transform.rotation *= Quaternion.Euler(0, 0, 90);
                }
                //else, rotate fastener if direction is vertical
                else if (direction.x == 0)
                    fastenerObject.transform.rotation *= Quaternion.Euler(0, 0, 90);
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
        //the position this cell will prepare to move to
        Vector2Int movePosition = currentPosition + moveDirection * 2;

        //add this moving cell to gadget's movingCells
        movingGadget.movingCells.Add(this);

        //add preparedMovePosition. If a different move position already exists, tearFail (this cell can't move this tick)
        if (preparedMovePosition == default)
            preparedMovePosition = movePosition;
        else if (preparedMovePosition != movePosition)
            tearFail = true;

        //add the position this cell is preparing to move to to positionSafety. If position has been
        //claimed by another cell, declare the position unsafe
        if (!CycleManager.positionSafety.TryGetValue(movePosition, out PositionSafetyInfo positionSafetyInfo))
            CycleManager.positionSafety.Add(movePosition, new PositionSafetyInfo { cellClaimingPosition = this, positionUnsafe = false });
        else if (positionSafetyInfo.cellClaimingPosition != this)
        {
            positionSafetyInfo.positionUnsafe = true;
            CycleManager.positionSafety[movePosition] = positionSafetyInfo;
        }
        //else if cellClaimingPosition IS this, do nothing

        //if there are cells fastened to this one, add each one if it isn't already moving
        if (fastenedCells.TryGetValue(this, out List<Cell> myFastenedCells))
            foreach (Cell fastenedCell in myFastenedCells)
                if (!movingGadget.movingCells.Contains(fastenedCell))
                    fastenedCell.GetMovingCell(movingGadget, moveDirection);

        //if there's a cell directly in front of this cell, add it if it isn't already moving
        if (gridIndex.TryGetValue(movePosition, out Cell frontCell))
            if (!movingGadget.movingCells.Contains(frontCell))
                frontCell.GetMovingCell(movingGadget, moveDirection);
    }

    public IEnumerator LerpMovement()
    {
        //update grid index

            //if the cell in gridIndex's entry for currentPosition is still this one,
            //remove the entry
        if (gridIndex.TryGetValue(currentPosition, out Cell cell) && cell == this)
            gridIndex.Remove(currentPosition);

            //if there's a cell in gridIndex's entry for preparedMovePosition, replace it
            //with this one. Else, merely add this one
        if (gridIndex.ContainsKey(preparedMovePosition))
            gridIndex[preparedMovePosition] = this;
        else
            gridIndex.Add(preparedMovePosition, this);

        //update current position
        currentPosition = preparedMovePosition;

        //move cell
        float timeLerped = 0;
            //cache both since transform.position will change and preparedMovePosition will be reset
        Vector2 startPosition = transform.position;
        Vector2 endPosition = preparedMovePosition;

            //cache tick speed in case it changes during the lerp
        float tickSpeed = CycleManager.TickSpeed;
        while (timeLerped < tickSpeed)
        {
            transform.position = Vector2.Lerp(startPosition, endPosition, timeLerped / tickSpeed);
            timeLerped += Time.deltaTime;
            yield return null;
        }
        transform.position = endPosition;
    }

    public void CellReset()
    {
        preparedMovePosition = Vector2Int.zero;
        tearFail = false;
    }
}
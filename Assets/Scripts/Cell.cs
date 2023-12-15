using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    //STATIC:
    public static Dictionary<Vector2Int, Cell> gridIndex = new();

        //key = Stopmode position between fastened cells, value = fastener
        //used during Stopmode
    public static Dictionary<Vector2Int, GameObject> fastenerIndex = new();

        //key = a cell with fastenings, value = list of cells the key cell is fastened to
        //used during Playmode
    public static Dictionary<Cell, List<Cell>> fastenedCells = new();

    //PREFAB REFERENCE:
    [SerializeField] private GameObject fastenerPref;
    public SpriteRenderer sr;

    //CONSTANT:
    protected readonly List<Vector2Int> directions = new()
    {
        Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down
    };

    //DYNAMIC:
    [NonSerialized] public Vector2Int preparedMovePosition;
        //true when this cell is preparing to move into more than one position
    [NonSerialized] public bool tearFail;
        //set by HUD when Cell is spawned
    [NonSerialized] public Vector2Int currentPosition;

    //used for layout saving:
    [NonSerialized] public int cellType; //0 = node, 1 = pulser, 2 = magnet
    [NonSerialized] public int nodeColorNumber;

    public void FastenCell()
    {
        List<Cell> cellsToFasten = new();
        foreach (Vector2Int direction in directions)
        {
            //check if there's an adjacent cell in this direction
            if (!gridIndex.TryGetValue(currentPosition + direction * 2, out Cell adjacentCell)) continue;

            //check if this cell is a gadget and it's facing this direction
            if (this is Gadget thisGadget && thisGadget.gadgetDirection == direction) continue;

            //check if the adjacent cell is a gadget facing this cell
            if (adjacentCell is Gadget adjacentGadget && adjacentGadget.gadgetDirection * 2 == currentPosition - adjacentCell.currentPosition) continue;

            //spawn fastener sprite
            GameObject fastenerObject = Instantiate(fastenerPref, transform);

                //fastener position = current position + direction * .5 * grid scale, which is 2 so it cancels out
            Vector2Int fastenerPosition = currentPosition + direction;
            fastenerObject.transform.position = (Vector2)fastenerPosition;

                //if this is a horizontal gadget, rotate fastener if direction is horizontal
            if (this is Gadget horizontalGadget && horizontalGadget.gadgetDirection.y == 0)
            {
                if (direction.y == 0)
                    fastenerObject.transform.rotation *= Quaternion.Euler(0, 0, 90);
            }
                //else, rotate fastener if direction is vertical
            else if (direction.x == 0)
                fastenerObject.transform.rotation *= Quaternion.Euler(0, 0, 90);

            //add fastener to static dictionary so it can be destroyed when either cell is erased
            if (fastenerIndex.ContainsKey(fastenerPosition))
            {
                Debug.LogError("Fastener already in FastenerIndex");
                return;
            }
            fastenerIndex.Add(fastenerPosition, fastenerObject);

            //add this cell to adjacentCell's fastenedCells entry, or create a new one if it doesn't exist
            if (!fastenedCells.ContainsKey(adjacentCell))
                fastenedCells.Add(adjacentCell, new List<Cell> { this });
            else
                fastenedCells[adjacentCell].Add(this);

            //cache adjacent cell
            cellsToFasten.Add(adjacentCell);
        }

        //if this cell is fastened to any cells, save them to its fastenedCells entry
        if (cellsToFasten.Count > 0)
            fastenedCells.Add(this, cellsToFasten);
    }

    public void UnFastenCell()
    {
        //check if this cell is fastened to any other cells
        if (!fastenedCells.ContainsKey(this)) return;
        
        foreach (Cell adjacentCell in fastenedCells[this])
        {
            //remove this cell's fastenedCells entry
            fastenedCells.Remove(this);

            //remove this cell from adjacent cell's fastenedCells entry
            if (!fastenedCells.TryGetValue(adjacentCell, out List<Cell> cellsFastenedToAdjacentCell))
            {
                Debug.LogError("Adjacent fastened cell not in this cell's FastenedCells entry");
                return;
            }

            if (!cellsFastenedToAdjacentCell.Contains(this))
            {
                Debug.LogError("This cell not found in adjacent fastened cell's FastenedCells entry");
                return;
            }

            cellsFastenedToAdjacentCell.Remove(this);

            //destroy fastener
            Vector2Int fastenerPosition = new((currentPosition.x + adjacentCell.currentPosition.x) / 2, (currentPosition.y + adjacentCell.currentPosition.y) / 2);
            if (!fastenerIndex.TryGetValue(fastenerPosition, out GameObject fastener))
            {
                Debug.LogError("Fastener not found in FastenerIndex");
                return;
            }
            fastenerIndex.Remove(fastenerPosition);
            Destroy(fastener);
        }
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

    public int GetRotation()
    {
        return Mathf.RoundToInt(transform.rotation.eulerAngles.z);
    }
}
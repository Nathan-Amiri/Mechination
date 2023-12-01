using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : Cell
{
    //readonly:
    [NonSerialized] public readonly List<Cell> movingCells = new();

    //dynamic:
    [SerializeField] private bool isPulser; //else is magnet

    protected new void OnEnable()
    {
        base.OnEnable();

        GameManager.PrepareGadgets += Prepare;
    }
    protected new void OnDisable()
    {
        base.OnDisable();

        GameManager.PrepareGadgets -= Prepare;
    }

    protected new void Awake()
    {
        base.Awake();

        gadgetDirection = Vector2Int.RoundToInt(transform.up); //replace this later!
    }

    private void Prepare()
    {
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

        foreach (var cell in movingCells)
            cell.gameObject.SetActive(false);
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

        foreach (var cell in movingCells)
            cell.gameObject.SetActive(false);
    }
}
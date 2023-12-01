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

    //assigned in scene:
    [SerializeField] private float tickSpeed;

    //readonly:
    private readonly float startDelay = .25f;

    //dynamic:
    


    private void Start()
    {
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
            yield return new WaitForSeconds(tickSpeed);
        }
    }

    //run once per 'gamemanager tick'
    private void Cycle()
    {
        PrepareGadgets?.Invoke();

        //check fail conditions

        //move simulateously

        //transform gadgets
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer grid;
    [SerializeField] private float tickSpeed;

    private readonly float startDelay = .25f;

    public delegate void FillGridIndexAction();
    public static FillGridIndexAction FillGridIndex;

    public delegate void FastenNodesAction();
    public static FastenNodesAction FastenNodes;

    public delegate void RefillQueueAction();
    public static RefillQueueAction RefillQueue;


    public static Vector2 GridSize {  get; private set; }

    public static List<Engine> engineQueue = new();

    private void Start()
    {
        GridSize = grid.size;

        //first, all nodes place their positions in the gridIndex
        FillGridIndex?.Invoke();

        //then, all nodes form attachments NOTE: LATER, CHANGE THIS SO THAT FASTENING HAPPENS AS NODES ARE PLACED ON GRID
        FastenNodes?.Invoke();

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
        //refill queue
        RefillQueue?.Invoke();

        //activate all engines in queue
        foreach (Engine engine in engineQueue)
            engine.ActivateEngine();

        engineQueue.Clear();
    }
}
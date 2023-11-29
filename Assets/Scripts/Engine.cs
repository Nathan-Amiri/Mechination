using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : Node
{
    [NonSerialized] public List<Node> movingNodes = new();
    [NonSerialized] public bool movingFailed;

    protected new void OnEnable()
    {
        base.OnEnable();

        GameManager.RefillQueue += RefillQueue;
    }
    protected new void OnDisable()
    {
        base.OnDisable();

        GameManager.RefillQueue -= RefillQueue;
    }

    protected new void Awake()
    {
        base.Awake();

        engineDirection = Vector2Int.RoundToInt(transform.up); //replace this later!
    }

    //called by GameManager if this engine is in queue
    public void ActivateEngine()
    {
        //the position in front of this one
        Vector2Int targetPosition = currentPosition + engineDirection * 2;

        //check for node in front of engine
        if (!gridIndex.TryGetValue(targetPosition, out Node targetNode))
            return;

        //reset moving variables
        movingNodes.Clear();
        movingFailed = false;

        //get all moving nodes
        targetNode.GetMovingNode(this, engineDirection);

        //check if moving failed
        if (movingFailed) return;

        //move all moving nodes. remove all keyValuePairs from gridindex before adding any new ones
        Dictionary<Vector2Int, Node> keyValuePairsToAddToIndex = new();
        foreach (Node node in movingNodes)
        {
            Vector2Int oldPosition = node.currentPosition;

            //move node
            node.currentPosition += engineDirection * 2;
            node.transform.position = (Vector2)node.currentPosition;

            //remove old keyValuePairs
            gridIndex.Remove(oldPosition);

            //cache keyValuePair to add later
            keyValuePairsToAddToIndex.Add(node.currentPosition, node);
        }
        //add cached keyValuePairs to gridIndex
        foreach (KeyValuePair<Vector2Int, Node> keyValuePair in keyValuePairsToAddToIndex)
            gridIndex.Add(keyValuePair.Key, keyValuePair.Value);
    }

    //called by GameManager on all Engines at the end of each cycle
    public void RefillQueue()
    {
        //if there's a node in front of engine, add engine to queue and attempt to activate
        if (gridIndex.ContainsKey(currentPosition + engineDirection * 2))
            GameManager.engineQueue.Add(this);
    }
}
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

    private void Awake()
    {
        engineDirection = transform.up;
    }

    //called by GameManager if this engine is in queue
    public void ActivateEngine()
    {
        //check for node in front of engine
        if (!GridIndex.gridIndex.ContainsKey((Vector2)transform.position + engineDirection))
            return;

        //reset moving variables
        movingNodes.Clear();
        movingFailed = false;

        //get all moving nodes
        GridIndex.gridIndex[(Vector2)transform.position + engineDirection].GetMovingNode(this, engineDirection);

        //check if moving failed
        if (movingFailed) return;

        //move all moving nodes. remove all keyValuePairs from gridindex before adding any new ones
        Dictionary<Vector2, Node> keyValuePairsToAddToIndex = new();
        foreach (Node node in movingNodes)
        {
            Vector2 oldPosition = node.transform.position;

            //engineDirection already set to magnitude of 1
            node.transform.position += (Vector3)engineDirection;

            //remove old keyValuePairs
            GridIndex.gridIndex.Remove(oldPosition);

            //cache keyValuePair to add later
            keyValuePairsToAddToIndex.Add(node.transform.position, node);
        }
        //add cached keyValuePairs to gridIndex
        foreach (KeyValuePair<Vector2, Node> keyValuePair in keyValuePairsToAddToIndex)
            GridIndex.gridIndex.Add(keyValuePair.Key, keyValuePair.Value);
    }

    //called by GameManager on all Engines at the end of each cycle
    public void RefillQueue()
    {
        //if there's a node in front of engine, add engine to queue and attempt to activate
        if (GridIndex.gridIndex.ContainsKey((Vector2)transform.position + engineDirection))
            GameManager.engineQueue.Add(this);
    }
}
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
        //the position in front of this one
        Vector2 targetPosition = (Vector2)transform.position + engineDirection;

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
        Dictionary<Vector2, Node> keyValuePairsToAddToIndex = new();
        foreach (Node node in movingNodes)
        {
            Vector2 oldPosition = node.transform.position;

            //engineDirection already set to magnitude of 1
            node.transform.position += (Vector3)engineDirection;

            //remove old keyValuePairs
            gridIndex.Remove(oldPosition);

            //cache keyValuePair to add later
            keyValuePairsToAddToIndex.Add(node.transform.position, node);
        }
        //add cached keyValuePairs to gridIndex
        foreach (KeyValuePair<Vector2, Node> keyValuePair in keyValuePairsToAddToIndex)
            gridIndex.Add(keyValuePair.Key, keyValuePair.Value);
    }

    //called by GameManager on all Engines at the end of each cycle
    public void RefillQueue()
    {
        //Debug.Log((Vector2)transform.position + engineDirection);
        //Debug.Log(gridIndex.ContainsKey((Vector2)transform.position + engineDirection));

        Vector2 targetPosition = (Vector2)transform.position + engineDirection;

        Debug.Log("position being checked is " + targetPosition);
        Debug.Log("dictionary has key -5.50, -1.50? " + gridIndex.ContainsKey(new Vector2(-5.50f, -1.50f)));
        Debug.Log("position being checked is -5.50, -1.50? " + (targetPosition == new Vector2(-5.50f, -1.50f)));
        Debug.Log("dictionary has checkPosition? " + gridIndex.ContainsKey(targetPosition));

        //if there's a node in front of engine, add engine to queue and attempt to activate
        if (gridIndex.ContainsKey(targetPosition))
            GameManager.engineQueue.Add(this);
    }
}
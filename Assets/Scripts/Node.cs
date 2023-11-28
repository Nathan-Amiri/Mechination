using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private readonly List<Vector2> directions = new()
    {
        Vector2.right, Vector2.left, Vector2.up, Vector2.down
    };

    //assigned in prefab
    [SerializeField] private GameObject fastener;

    //default unless this node is an engine, set by Engine.
    protected Vector2 engineDirection = Vector2.zero;

    //nodes that have already fastened themselves to all adjacent nodes
    public static List<Node> fastenedNodes = new();

    protected void OnEnable()
    {
        GameManager.FillGridIndex += FillGridIndex;
        GameManager.FastenNodes += FastenNodes;
    }
    protected void OnDisable()
    {
        GameManager.FillGridIndex -= FillGridIndex;
        GameManager.FastenNodes -= FastenNodes;
    }

    //run at the start of the game
    private void FillGridIndex()
    {
        //round to nearest .5 in case current position is slightly off (it shouldn't be)
        Vector2 position;
        position.x = Mathf.Round(transform.position.x * 2) / 2;
        position.y = Mathf.Round(transform.position.y * 2) / 2;
        transform.position = position;

        GridIndex.gridIndex.Add(transform.position, this);
    }

    //run at the start of the game, after gridindex is filled
    private void FastenNodes()
    {
        foreach (Vector2 direction in directions)
        {
            //check if this node is an engine and it's facing this direction
            if (engineDirection == direction) continue;

            //check if there's an adjacent node in this direction
            if (!GridIndex.gridIndex.ContainsKey((Vector2)transform.position + direction)) continue;

            Node adjacentNode = GridIndex.gridIndex[(Vector2)transform.position + direction];

            //check if the adjacent node has already created all its necessary fastenings
            if (fastenedNodes.Contains(adjacentNode)) continue;

            //check if the adjacent node is an engine facing this direction
            if (adjacentNode.engineDirection == (Vector2)transform.position - (Vector2)adjacentNode.transform.position) continue;


            //create fastener sprite
            Quaternion fastenerRotation = direction.x == 0 ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;
                //transform.position makes more sense since the fastener is purely visual, though it should be the same as logicposition here anyway
            Instantiate(fastener, (Vector2)transform.position + direction * .5f, fastenerRotation, transform);

            //save this node to fastenedNodes so adjacent nodes won't try to refasten to it
            fastenedNodes.Add(this);
        }
    }

    //when an engine activates, it calls this method on all nodes it's trying to move
    //isFirstNode = true when this node is the one in front of the engine (the first node this method is called on)
    public void GetMovingNode(Engine movingEngine, Vector2 moveDirection)
    {
        //1. check if moving already failed
        if (movingEngine.movingFailed) return;

        //2. set movingFailed to true if this node is trying to move off the grid UNFINISHED


        //3. cache all nodes that this node will move
        List<Node> cachedNodes = new();
        foreach (Vector2 direction in directions)
        {
            //check if there's an adjacent node in this direction
            if (!GridIndex.gridIndex.ContainsKey((Vector2)transform.position + direction)) continue;

            //cache the adjacent node
            Node adjacentNode = GridIndex.gridIndex[(Vector2)transform.position + direction];

            //check if the node is already moving
            if (movingEngine.movingNodes.Contains(adjacentNode)) continue;

            //if adjacent node is in front of this node, or if it's fastened, cache it
            if (direction == moveDirection) //|| adjacentNode is fastened to this UNFINISHED
                cachedNodes.Add(adjacentNode);
        }


        //4. set movingFailed to true if any cached nodes are the movingEngine
        if (cachedNodes.Contains(movingEngine))
        {
            movingEngine.movingFailed = true;
            return;
        }

        //6. add this moving node to engine's movingNodes
        movingEngine.movingNodes.Add(this);

        //7. call GetMovingNode in the cached nodes
        foreach (Node cachedNode in cachedNodes)
            cachedNode.GetMovingNode(movingEngine, moveDirection);
    }
}
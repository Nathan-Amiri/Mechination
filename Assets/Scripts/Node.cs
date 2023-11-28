using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    //assigned in prefab
    [SerializeField] private GameObject fastener;

    private readonly List<Vector2> directions = new()
    {
        Vector2.right, Vector2.left, Vector2.up, Vector2.down
    };

    //zero unless this node is an engine, set by Engine.
    protected Vector2 engineDirection = Vector2.zero;

    public static Dictionary<Vector2, Node> gridIndex = new();

    //key = a node with fastenings, value = list of nodes the key node is fastened to
    public static Dictionary<Node, List<Node>> fastenedNodes = new();

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

    //called by GameManager at the start of the game
    private void FillGridIndex()
    {
        gridIndex.Add((Vector2)transform.position, this);
    }

    //called by GameManager at the start of the game, after gridindex is filled
    private void FastenNodes()
    {
        List<Node> nodesToFasten = new();
        foreach (Vector2 direction in directions)
        {
            //check if this node is an engine and it's facing this direction
            if (engineDirection == direction) continue;

            //check if there's an adjacent node in this direction
            if (!gridIndex.TryGetValue((Vector2)transform.position + direction, out Node adjacentNode)) continue;

            //check if the adjacent node is an engine facing this direction
            if (adjacentNode.engineDirection == (Vector2)transform.position - (Vector2)adjacentNode.transform.position) continue;

            //create fastener sprite
            Quaternion fastenerRotation = direction.x == 0 ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;
                //transform.position makes more sense since the fastener is purely visual, though it should be the same as logicposition here anyway
            Instantiate(fastener, (Vector2)transform.position + direction * .5f, fastenerRotation, transform);

            //cache adjacent node
            nodesToFasten.Add(adjacentNode);
        }
        
        //if this node has fastenings, save to static dictionary
        if (nodesToFasten.Count > 0)
            fastenedNodes.Add(this, nodesToFasten);
    }

    //when an engine activates, it calls this method on all nodes it's trying to move
    //isFirstNode = true when this node is the one in front of the engine (the first node this method is called on)
    public void GetMovingNode(Engine movingEngine, Vector2 moveDirection)
    {
        //the position this node would move to
        Vector2 targetPosition = (Vector2)transform.position + moveDirection;

        //1. check if moving already failed
        if (movingEngine.movingFailed) return;

        //2. set movingFailed to true if this node is trying to move off the grid
        float maxMoveDistanceFromOrigin;
        float targetPositionDistanceFromOrigin;
        //if moving along the y axis
        if (moveDirection.x == 0)
        {
            maxMoveDistanceFromOrigin = GameManager.GridSize.y / 2;
            targetPositionDistanceFromOrigin = targetPosition.y;
        }
        //if moving along the x axis
        else
        {
            maxMoveDistanceFromOrigin = GameManager.GridSize.x / 2;
            targetPositionDistanceFromOrigin = targetPosition.x;
        }

        //fail if target position is greater than maxMoveDistanceFromOrigin (the limit set by the grid)
        if (Mathf.Abs(targetPositionDistanceFromOrigin) > Mathf.Abs(maxMoveDistanceFromOrigin))
        {
            movingEngine.movingFailed = true;
            return;
        }

        //3. cache all other nodes that this node will move
        List<Node> cachedNodes = new();

        //if there are nodes fastened to this one, cache each one if it isn't already moving
        if (fastenedNodes.TryGetValue(this, out List<Node> myFastenedNodes))
            foreach (Node fastenedNode in myFastenedNodes)
                if (!movingEngine.movingNodes.Contains(fastenedNode))
                    cachedNodes.Add(fastenedNode);

        //if there's a node in front of this node, cache it if it isn't already cached or moving
        if (gridIndex.TryGetValue(targetPosition, out Node frontNode))
            if (!cachedNodes.Contains(frontNode) && !movingEngine.movingNodes.Contains(frontNode))
                cachedNodes.Add(frontNode);


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
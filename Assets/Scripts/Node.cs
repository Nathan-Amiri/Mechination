using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    //assigned in prefab
    [SerializeField] private GameObject fastener;

    private readonly List<Vector2Int> directions = new()
    {
        Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down
    };

    //zero unless this node is an engine, set by Engine.
    protected Vector2Int engineDirection = Vector2Int.zero;

    public static Dictionary<Vector2Int, Node> gridIndex = new();

    //key = a node with fastenings, value = list of nodes the key node is fastened to
    public static Dictionary<Node, List<Node>> fastenedNodes = new();


    [NonSerialized] public Vector2Int currentPosition;

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

    protected void Awake()
    {
        currentPosition = Vector2Int.RoundToInt(transform.position); //replace this later!
    }

    //called by GameManager at the start of the game
    private void FillGridIndex()
    {
        gridIndex.Add(currentPosition, this);
    }

    //called by GameManager at the start of the game, after gridindex is filled
    private void FastenNodes()
    {
        List<Node> nodesToFasten = new();
        foreach (Vector2Int direction in directions)
        {
            //check if this node is an engine and it's facing this direction
            if (engineDirection == direction) continue;

            //check if there's an adjacent node in this direction
            if (!gridIndex.TryGetValue(currentPosition + direction * 2, out Node adjacentNode)) continue;

            //check if the adjacent node is an engine facing this node
            if (adjacentNode.engineDirection * 2 == currentPosition - adjacentNode.currentPosition) continue;

            //if adjacent node isn't in static dictionary (meaning it's already created its fastenings), create fastener sprite
            if (!fastenedNodes.ContainsKey(adjacentNode))
            {
                GameObject fastenerObject = Instantiate(fastener, transform);
                //fastener position = current position + direction * .5 * grid scale, which is 2 so it cancels out
                fastenerObject.transform.position = (Vector2)currentPosition + (Vector2)direction;
                //if direction is vertical, rotate fastener
                //multiply rotation by this node's rotation in case this node (the fastener's parent) is an engine rotated 90 degrees
                if (direction.x == 0)
                    fastenerObject.transform.rotation *= Quaternion.Euler(0, 0, 90) * transform.rotation;
            }

            //cache adjacent node
            nodesToFasten.Add(adjacentNode);
        }
        
        //if this node has fastenings, save to static dictionary
        if (nodesToFasten.Count > 0)
            fastenedNodes.Add(this, nodesToFasten);
    }

    //when an engine activates, it calls this method on all nodes it's trying to move
    public void GetMovingNode(Engine movingEngine, Vector2Int moveDirection)
    {
        //the position this node would move to
        Vector2Int targetPosition = currentPosition + moveDirection * 2;

        //1. check if moving already failed
        if (movingEngine.movingFailed) return;

        //2. set movingFailed to true if this node is trying to move off the grid
        float maxMoveDistanceFromOrigin;
        float targetPositionDistanceFromOrigin;
        //if moving along the y axis
        if (moveDirection.x == 0)
        {
            //maxMoveDistanceFrom Origin = half of GridSize.y times the grid's scale, which is 2 so it cancels out
            maxMoveDistanceFromOrigin = GameManager.GridSize.y;
            targetPositionDistanceFromOrigin = targetPosition.y;
        }
        //if moving along the x axis
        else
        {
            //maxMoveDistanceFrom Origin = half of GridSize.x times the grid's scale, which is 2 so it cancels out
            maxMoveDistanceFromOrigin = GameManager.GridSize.x;
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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemies
{
    
    /// <summary> Class used for implementing A* Search, which is used for Enemy Pathfinding </summary>
    public class AStarSearch : MonoBehaviour
    {
        
        /* * * * * * * * * *
         * A* Main Methods *
         * * * * * * * * * */
        
        /// <summary> Node that represents a position in A* Search </summary>
        public class AStarNode
        {
            public Vector3 position { get; }
            public AStarNode parent { get; }
            public float fScore { get; } // Score representing how close the position is to the goal
            public float gScore { get; } // Score representing how much it took to get to this point from the start

            // Constructor
            public AStarNode(Vector3 p_position, AStarNode p_parent, float p_fScore, float p_gScore)
            {
                position = p_position;
                parent = p_parent;
                fScore = p_fScore;
                gScore = p_gScore;
            }
        }

        /// <summary> Use A* Search to find a path between a Starting and Goal position </summary>
        public static Stack<Vector3> Search(GameObject entity, Vector3 goal)
        {
            //HEADER: __Initializations__ 
            
            var start = entity.transform.position; // The Entity's Starting Position
            var size = entity.transform.localScale; // Size of the entity
            
            var openList = new List<AStarNode>(); // Queue for storing unchecked positions
            var closedList = new List<AStarNode>(); // Queue for storing already checked positions
            
            var roundedStart = RoundPosition(start); // Round the starting position to the nearest integers
            var startingNode = new AStarNode(roundedStart, null, 0, 0); // The node at the starting position

            //HEADER ___Main Loop___

            openList.Add(startingNode); // Add the starting node to the open list

            var stopSearch = false; // Can be used to stop the search when the goal is found
            AStarNode goalNode = null;
            
            while (openList.Count > 0 && !stopSearch) // While the list is not empty
            {
                
                //SUBHEADER ___Obtain Successors___
                var q = Pop(openList); // Pop from Open List
                var successors = GetSuccessors(q.position);

                
                // For each successor position that is valid for the enemy to move into
                foreach (var successor in successors.Where(successor => IsValidPosition(successor, size)))
                {
                    //SUBHEADER ___Goal is Found___
                    
                    if (Vector3.Distance(successor, goal) < 1)
                    {
                        goalNode = new AStarNode(successor, q, 0, 999);
                        stopSearch = true; 
                        break;
                    } // Check if the distance from the goal is below a threshold
                    
                    //SUBHEADER ___Compute G, H, & F___

                    var g = q.gScore + Vector3.Distance(q.position, successor);
                    var h = Vector3.Distance(successor, goal);
                    var f = g + h;
                    
                    // SUBHEADER ___Check Open and Closed List___

                    // Skip this node if another with the same position already exists and has a lower (or equal) f score
                    var skipNode = openList.Concat(closedList).ToList().Any(node => node.position == successor && node.fScore <= f);
                    if (skipNode) continue;
                    
                    // SUBHEADER ___Add New Node___
                    
                    var newNode = new AStarNode(successor, q, f, g);
                    AddNode(newNode, openList);
                }
                closedList.Add(q);
            }

            if (goalNode != null)
            {
               return CreatePath(goalNode, startingNode);
            }
            else
            {
                print("Goal was NULL");
                return null;
            }
        }

        
        /* * * * * * * * * * *
         * A* Helper Methods *
         * * * * * * * * * * */
        
        /// <summary> Get the 8 surrounding successors of a given position </summary>
        private static List<Vector3> GetSuccessors(Vector3 pos)
        {
            var successors = new List<Vector3>(); // List to fill and return
            
            // Loop through the 3x3 offset square around the pos. Get every position except pos itself
            for (var i = -1; i < 2; i++) {
                for (var j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0) continue; // Skip the (0,0) offset (which is just Pos)
                    var successor = pos + new Vector3(i, 0, j);
                    successors.Add(successor);
                }
            }

            return successors;
        }
        
        /// <summary> Determine if a position is valid to move to by using a BoxCast to determine if any obstacles are in it</summary>
        /// <param name="pos">The position of the BoxCast</param>
        /// <param name="size">The size of the BoxCast</param>
        private static bool IsValidPosition(Vector3 pos, Vector3 size)
        {
            var halfExtents = new Vector3(size.x / 2, size.y / 2, size.z / 2); // Determine size of BoxCast
            
            // Get all the colliders at this position
            // ReSharper disable once Unity.PreferNonAllocApi
            var hits = Physics.OverlapBox(pos, halfExtents, new Quaternion(0,0,0,0));
            if (hits.Length <= 0) return true; // If colliders found
            // Check if any collider is an obstacle. If so, this position is not valid to move to
            return hits.All(hit => !hit.transform.CompareTag("Obstacle"));
        }

        /// <summary> Add a node to the open list, whilst maintaining an ascending order of f </summary>
        private static void AddNode(AStarNode node, List<AStarNode> list)
        {
            for (var index = 0; index < list.Count; index++)
            {
                var compNode = list[index]; 
                if (!(node.fScore < compNode.fScore)) continue;
                list.Insert(index, node);
                return;
            }
            list.Add(node);
        }

        private static Stack<Vector3> CreatePath(AStarNode goalNode, AStarNode startNode)
        {
            var finalPath = new Stack<Vector3>();
            var curNode = goalNode;
            while (curNode != startNode)
            {
                finalPath.Push(curNode.position);
                curNode = curNode.parent;
            }
            return finalPath;
        }
        
        /* * * * * * * * * *
         * Vector3 Methods *
         * * * * * * * * * */
                 
        /// <summary> Return the given position rounded to the nearest integers </summary>
        private static Vector3 RoundPosition(Vector3 pos) { return new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)); }
        
        
        /* * * * * * * * * *
         * Queue Methods *
         * * * * * * * * * */
        
        /// <summary> Remove a value from the front of a list, and return it </summary>
        /// <typeparam name="T"> The data type of the List </typeparam>
        private static T Pop<T>(List<T> list)
        {
            var node = list[0];
            list.RemoveAt(0);
            return node;
        }
    }
}

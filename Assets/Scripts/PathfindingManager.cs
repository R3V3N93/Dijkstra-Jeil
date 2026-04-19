using UnityEngine;
using System.Collections.Generic;

public class PathfindingManager : MonoBehaviour
{
    enum Algorhithm
    {
        BreadthFirstSearch,
        Dijkstra,
        Astar
    };
    [Header("Global Algorhithm thingys")]
    [SerializeField] JeilNode startNode; 
    [SerializeField] JeilNode destinationNode;
    [SerializeField] Algorhithm selectedAlgorhithm = Algorhithm.BreadthFirstSearch;

    [Header("Debug")]
    [SerializeField] List<JeilNode> shortestPath = new List<JeilNode>();
    
    public void LeftClick()
    {   
    }

    public void RightClick()
    {
        
    }

    public void StartPathFinding()
    {
        switch (selectedAlgorhithm)
        {
            case Algorhithm.BreadthFirstSearch:
                BreadthFirstSearch();
                break;
            case Algorhithm.Dijkstra:
                Dijkstra();
                break;
            case Algorhithm.Astar:
                Astar();
                break;
        }

        if (shortestPath.Count == 0)
        {
            Debug.LogError("Something's wrong with Algorhithm execution. Check log.");
            return;
        }
        
        // Pathfinding is over. Trim the list
        shortestPath.Clear();
    }

    public void BreadthFirstSearch()
    {
        
    }

    public void Dijkstra()
    {
        
    }

    public void Astar()
    {
        
    }
}

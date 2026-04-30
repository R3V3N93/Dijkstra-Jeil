using UnityEngine;
using System.Collections.Generic;

public class PathfindingManager : MonoBehaviour
{
    [Header("UI")] 
    public GameObject ui;
    enum Algorhithm
    {
        BreadthFirstSearch,
        Dijkstra,
        Astar
    };
    [Header("Global Algorhithm thingys")]
    public JeilNode startNode; 
    public JeilNode destinationNode;
    [SerializeField] Algorhithm selectedAlgorhithm = Algorhithm.BreadthFirstSearch;

    [Header("Debug")]
    [SerializeField] List<JeilNode> shortestPath = new List<JeilNode>();
    
    private void OnDisable()
    {
        GameManager.obj.pinput.eventRightClick -= RightClick;
        GameManager.obj.pinput.eventClick      -= LeftClick;
        
        ui.SetActive(false);
    }
    
    private void OnEnable()
    {
        GameManager.obj.pinput.eventRightClick += RightClick;
        GameManager.obj.pinput.eventClick      += LeftClick;
        
        GameManager.obj.state = GameManager.GameState.PathFinding;
        
        ui.SetActive(true);
    }
    
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

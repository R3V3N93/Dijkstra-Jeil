using UnityEngine;
using System.Collections.Generic;

public class PathfindingManager : MonoBehaviour
{
    [Header("UI")] 
    public GameObject ui;
    enum Algorithm
    {
        BreadthFirstSearch,
        Dijkstra,
        Astar
    };
    [Header("Global Algorhithm thingys")]
    public JeilNode startNode; 
    public JeilNode destinationNode;
    [SerializeField] Algorithm selectedAlgorhithm = Algorithm.BreadthFirstSearch;

    [Header("Debug")]
    [SerializeField] List<JeilNode> shortestPath = new List<JeilNode>();

    [SerializeField] private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }
    
    private void OnDisable()
    {
        GameManager.obj.pinput.eventRightClick -= RightClick;
        GameManager.obj.pinput.eventClickOn      -= LeftClickOn;
        GameManager.obj.pinput.eventClickOff      -= LeftClickOff;
        
        ui.SetActive(false);
    }
    
    private void OnEnable()
    {
        GameManager.obj.pinput.eventRightClick += RightClick;
        GameManager.obj.pinput.eventClickOn      += LeftClickOn;
        GameManager.obj.pinput.eventClickOff      += LeftClickOff;
        
        GameManager.obj.state = GameManager.GameState.PathFinding;
        
        ui.SetActive(true);
    }
    
    public void SetAlgorithm(int to)
    {
        Debug.Log(to);
        selectedAlgorhithm = (Algorithm)to;
    }
    
    public void LeftClickOn()
    {   
    }

    public void LeftClickOff()
    {   
    }

    public void RightClick()
    {
        
    }

    public void StartPathFinding()
    {
        line.positionCount = 0;
        switch (selectedAlgorhithm)
        {
            case Algorithm.BreadthFirstSearch:
                BreadthFirstSearch();
                break;
            case Algorithm.Dijkstra:
                Dijkstra();
                break;
            case Algorithm.Astar:
                Astar();
                break;
        }

        if (shortestPath.Count == 0)
        {
            Debug.LogError("Something's wrong with Algorhithm execution. Check log.");
            return;
        }

        line.positionCount = shortestPath.Count;
        for (int i = 0; i < shortestPath.Count; i++)
        {
            line.SetPosition(i, shortestPath[i].transform.position);
        }
        
        // Pathfinding is over. Trim the list
        shortestPath.Clear();
    }

    public void BreadthFirstSearch()
    {
        
    }

    public void Dijkstra()
    {
        if (startNode == null && destinationNode == null)
        {
            Debug.unityLogger.Log("Dijkstra: startNode or destinationNode is null");
            return;
        }

        List<JeilNode> allNodes = GameManager.GetNodes();
        List<JeilNode> unvisited = new List<JeilNode>();

        Dictionary<JeilNode, int> distance = new Dictionary<JeilNode, int>();
        Dictionary<JeilNode, JeilNode> previousNode = new Dictionary<JeilNode, JeilNode>();
        
        foreach (JeilNode node in allNodes)
        {
            distance[node] = int.MaxValue;
            previousNode[node] = null;
            unvisited.Add(node);
        }

        distance[startNode] = 0;

        while (unvisited.Count > 0)
        {
            JeilNode current = null;
            foreach (JeilNode node in unvisited)
            {
                if (current == null || distance[node] < distance[current])
                {
                    current = node;
                }
            }
        }
    }

    public void Astar()
    {
        
    }
}

using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

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
        if (startNode == null && destinationNode == null)
        {
            Debug.Log("Dijkstra: startNode or destinationNode is null");
            return; 
        }
        //시작 노드랑 끝 노드 없을때 오류 방지

        List<JeilNode> allNodes = GameManager.GetNodes();
        Queue<JeilNode> allNodeQueue = new Queue<JeilNode>();
        allNodeQueue.Enqueue((startNode));
        foreach (JeilNode node in allNodes)
        {
            if (node == startNode)
            {
                continue;
            }
            allNodeQueue.Enqueue(node);
        }
        //고리 큐(allNodeQueue) 할당 완료
        //Dictionary<JeilNode, JeilNode> distance
        

    }

    public void Dijkstra()
    {
        
    }

    public void Astar()
    {
        
    }
}

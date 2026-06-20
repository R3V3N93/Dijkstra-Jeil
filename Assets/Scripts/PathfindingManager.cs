using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using System.Diagnostics;
using UnityEngine.UIElements;

[Serializable]
public struct TimerInfo
{
    public float lastAlgorithm;
    public float estimatedRealTime;
}

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

    [Header("UnityEngine.Debug")]
    [SerializeField] List<JeilNode> shortestPath = new List<JeilNode>();

    public LineRenderer linePrefab;
    public List<LineRenderer> activeLines;

    void Awake()
    {
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
        
        GameManager.obj.state = GameState.PathFinding;

        foreach (JeilEdge edge in GameManager.GetEdges())
        {
            edge.graphics.SetActive(false);
        }

        foreach (JeilNode node in GameManager.GetNodes())
        {
            node.SetOutline(false);
            if (node.visibleInPathfinding || node.IsDestinationNode() || node.IsStartNode()) continue;

            node.sprite.enabled = false;
        }

        ui.SetActive(true);
    }
    
    public void SetAlgorithm(int to)
    {
        UnityEngine.Debug.Log(to);
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

    public void ClearPath()
    {
        shortestPath.Clear();
        foreach(LineRenderer i in activeLines) DestroyImmediate(i.gameObject);
        activeLines.Clear();
    }

    public void StartPathFinding()
    {
        ClearPath();
        Stopwatch watch = new Stopwatch();
        watch.Start();
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
            UnityEngine.Debug.LogError("Something's wrong with Algorhithm execution. Check log.");
            return;
        }

        activeLines.Add(Instantiate(linePrefab, this.transform));  
        for (int i = 0; i < shortestPath.Count; i++)
        {
            int lineIndex = activeLines.Count - 1;
            activeLines[lineIndex].positionCount++;
            activeLines[lineIndex].SetPosition(activeLines[lineIndex].positionCount-1, shortestPath[i].transform.position);
            if(i < shortestPath.Count-1 && !shortestPath[i].neighborEdges[shortestPath[i+1]].visibleInPathfinding)
            {
                lineIndex++;
                activeLines.Add(Instantiate(linePrefab, this.transform));  
            }
        }

        watch.Stop();
        UnityEngine.Debug.Log("Algorithm Execution complete. Elapsed Time : " + watch.ElapsedMilliseconds);
    }

    public void BreadthFirstSearch()
    {
        if (startNode == null || destinationNode == null)
        {
            UnityEngine.Debug.Log("BreadthFirstSearch: startNode or destinationNode is null");
            return; 
        }
        //시작 노드랑 끝 노드 없을때 오류 방지
        
        Queue<JeilNode> allNodeQueue = new Queue<JeilNode>();
        allNodeQueue.Enqueue((startNode));
        //고리 큐(allNodeQueue) 할당 완료
        
        Dictionary<JeilNode, JeilNode> came_from = new Dictionary<JeilNode, JeilNode>();
        came_from[startNode] = null;
        //경로 설정
        
        while (allNodeQueue.Count > 0)
        {
            JeilNode currentNode = allNodeQueue.Dequeue(); // 고리 큐의 첫번째 원소를 고름
            foreach (JeilNode neighbor in  currentNode.neighbors)// 고른 노드의 이웃 노드 중에서
            {
                if (!came_from.ContainsKey(neighbor)) // 도착하지 않은 노드가 있다면
                {
                    allNodeQueue.Enqueue(neighbor); // 그 노드에서 부터 다시 이웃 탐색 하기 위해 고리에 추가
                    came_from[neighbor] = currentNode;  // 그 노드에 도착! 경로를 저장함
                }	
            }		
        }

        JeilNode sizak = destinationNode;
        shortestPath.Add(destinationNode);
        while (came_from[sizak] != null)
        {
            shortestPath.Add(came_from[sizak]);
            sizak = came_from[sizak];
        }
        shortestPath.Reverse();
    }

    public void Dijkstra()
    {
        if (startNode == null || destinationNode == null)
        {
            UnityEngine.Debug.Log("Dijkstra: startNode or destinationNode is null");
            return; 
        }
        //시작 노드랑 끝 노드 없을때 오류 방지
        
        PriorityQueue<JeilNode, int> frontier = new PriorityQueue<JeilNode, int>();
        frontier.Enqueue(startNode, 0);
        
        // cost_so_far은 특정 노드까지 도달하는데 거친 비용을 합한 값을 저장한다.
        // 예를 들어 시작 노드에서 노드B까지 가는데 비용이 각각 1, 3, 5가 소요된다면,
        // cost_so_far[노드B]는 1+3+5=9가 된다.
        Dictionary<JeilNode, int> cost_so_far = new Dictionary<JeilNode, int>();
        cost_so_far[startNode] = 0;
        Dictionary<JeilNode, JeilNode> came_from = new Dictionary<JeilNode, JeilNode>();
        came_from[startNode] = null;

        while (frontier.Count > 0)
        {
            JeilNode currentNode = frontier.Peek(); // 고리 큐의 첫번째 원소를 고름
            frontier.Dequeue(out currentNode, out _); //첫 원소 뺌

            if (currentNode == destinationNode) // 목적지를 진작에 찾았다면
            {
                break; // 조기 이탈!
            }
            
            foreach(JeilNode neighbor in currentNode.neighbors) // 고른 노드의 이웃 노드 중에서
            {
                int tempCost = cost_so_far[currentNode] + currentNode.neighborEdges[neighbor].cost;
                if (!cost_so_far.ContainsKey((neighbor)) || tempCost < cost_so_far[neighbor]) // 해당 노드까지의 비용이 계산되지 않았거나, 현재 경로가 저장된 경로보다 비용이 적다면
                {
                    cost_so_far[neighbor] = tempCost;
                    frontier.Enqueue(neighbor, tempCost); // tempCost는 고리에서 탐색되는 새로운 노드의 '우선순위'를 나타내어, 큰 값이 들어가므로 PriorityQueue인 frontier에서 순서가 뒤로 밀려남.
                    came_from[neighbor] = currentNode;  // 그 노드에 도착! 경로를 저장함
                }
            }
            
        }
        
        JeilNode sizak = destinationNode;
        shortestPath.Add(destinationNode);
        while (came_from[sizak] != null)
        {
            shortestPath.Add(came_from[sizak]);
            sizak = came_from[sizak];
        }
        shortestPath.Reverse();
    }

    public void Astar()
    {
        if (startNode == null || destinationNode == null)
        {
            UnityEngine.Debug.Log("Dijkstra: startNode or destinationNode is null");
            return; 
        }
        //시작 노드랑 끝 노드 없을때 오류 방지
        
        PriorityQueue<JeilNode, int> frontier = new PriorityQueue<JeilNode, int>();
        frontier.Enqueue(startNode, 0);
        
        // cost_so_far은 특정 노드까지 도달하는데 거친 비용을 합한 값을 저장한다.
        // 예를 들어 시작 노드에서 노드B까지 가는데 비용이 각각 1, 3, 5가 소요된다면,
        // cost_so_far[노드B]는 1+3+5=9가 된다.
        Dictionary<JeilNode, int> cost_so_far = new Dictionary<JeilNode, int>();
        cost_so_far[startNode] = 0;
        Dictionary<JeilNode, JeilNode> came_from = new Dictionary<JeilNode, JeilNode>();
        came_from[startNode] = null;

        while (frontier.Count > 0)
        {
            JeilNode currentNode = frontier.Peek(); // 고리 큐의 첫번째 원소를 고름
            frontier.Dequeue(out currentNode, out _); //첫 원소 뺌

            if (currentNode == destinationNode) // 목적지를 진작에 찾았다면
            {
                break; // 조기 이탈!
            }
            
            foreach(JeilNode neighbor in currentNode.neighbors) // 고른 노드의 이웃 노드 중에서
            {
                int tempCost = cost_so_far[currentNode] + currentNode.neighborEdges[neighbor].cost;
                if (!cost_so_far.ContainsKey((neighbor)) || tempCost < cost_so_far[neighbor]) // 해당 노드까지의 비용이 계산되지 않았거나, 현재 경로가 저장된 경로보다 비용이 적다면
                {
                    cost_so_far[neighbor] = tempCost;
                    
                    //한줄 추가!!
                    int priority = tempCost + Heuristic(neighbor.transform.position, destinationNode.transform.position, neighbor.layer, destinationNode.layer);
                    
                    frontier.Enqueue(neighbor, priority); // tempCost는 고리에서 탐색되는 새로운 노드의 '우선순위'를 나타내어, 큰 값이 들어가므로 PriorityQueue인 frontier에서 순서가 뒤로 밀려남.
                    came_from[neighbor] = currentNode;  // 그 노드에 도착! 경로를 저장함
                }
            }
            
        }
        
        JeilNode sizak = destinationNode;
        shortestPath.Add(destinationNode);
        while (came_from[sizak] != null)
        {
            shortestPath.Add(came_from[sizak]);
            sizak = came_from[sizak];
        }
        shortestPath.Reverse();
    }
    
    private int Heuristic(Vector2 neighborNode, Vector2 destinationNode, uint neighborLayer, uint destinationLayer)
    {
        int temp = (int)(Mathf.Abs(neighborNode.x-destinationNode.x)+Mathf.Abs(neighborNode.y-destinationNode.y) + 2*Mathf.Abs((int)destinationLayer-(int)neighborLayer));
        return temp;
    }
}

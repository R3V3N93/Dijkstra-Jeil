using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    private readonly List<(TElement element, TPriority priority)> _heap = new List<(TElement element, TPriority priority)>();

    public int Count => _heap.Count;
    
    public void Enqueue(TElement element, TPriority priority)
    {
        // 마지막 위치에 원소 추가
        _heap.Add((element, priority));
        var index = _heap.Count - 1; //추가된 원소의 초기 인덱스

        while(index > 0)  //root가 아닐때
        {
            // 부모 노드의 인덱스 구해오기
            var parent = (index - 1) / 2;

            // 현재 노드의 우선순위가 부모보다 크거나 같으면 (더 이상 위로 올릴 필요 없음)
            if(_heap[index].priority.CompareTo(_heap[parent].priority) >= 0)
                return;

            // 아니라면 부모와 자식을 스왑
            (_heap[parent], _heap[index]) = (_heap[index], _heap[parent]);
            //C#은 튜플 바꿀 때 (a, b) = (b, a);을 지원함
            index = parent;
        }
        //void 형태는 함수 끝에 오면 자동 반환(왜 회색인가 했네)
    }
    
    public void Dequeue(out TElement element, out TPriority priority)
    {
        if(_heap.Count <= 0)
        {
            element = default;
            priority = default;
            Debug.Log("Already empty");
            return;
        }

        /*if (_heap.Count == 1)
        {
            _heap.RemoveAt(0);
            element = default;
            priority = default;
            return;
        }*/

        // 루트 요소 반환
        element = _heap[0].element;
        priority = _heap[0].priority;

        // 마지막 요소를 루트에 위치시키고, 힙 크기를 줄임
        var lastElement = _heap[^1];  //^: 햇, Hat  ^n은 Length-n를 뜻함
                                      ////그니깐 heap[^1]는 heap(PriorityQueue), 그니깐 이진 트리 구조의 마지막 원소
        _heap[0] = lastElement; //root에 마지막 원소 넣기
        _heap.RemoveAt(_heap.Count - 1); //마지막 원소를 root로 옮겼으니 기존 마지막 원소 제거

        //PriorityQueue 구조 특징이 (부모요소<=자식요소)만 만족하면 꺼낼때 문제가 없음 그래서 이런식으로 대충 정렬해도 됨
        //대충이라는 말을 보강하자면 전체정렬 할 필요 없이 (부모요소<=자식요소)만 만족하면 된다는 거
        // 정렬 시작
        var index = 0;
        var count = _heap.Count;

        while(true)
        {
            // 자식의 인덱스를 구함
            var left = index * 2 + 1;
            var right = index * 2 + 2;
            var current = index;

            // 좌측 자식의 우선순위가 현재 우선순위보다 낮다면
            //left < count 는 이진 트리 구조에서 존재하지 않는 부분일 경우를 예외처리하는 조건
            //-원소 1개거나 2개일때 가정해서 따라가다보면 알게 될 거임 이거 보고 존나 소름돋음
            if(left < count && _heap[current].priority.CompareTo(_heap[left].priority) > 0)
            {
                current = left;
            }
            // 우측 자식의 우선순위가 현재 우선순위보다 낮다면
            if(right < count && _heap[current].priority.CompareTo(_heap[right].priority) > 0)
            {
                current = right;
            }
            // 두 조건 다 만족하지 못한다면
            if(current == index)
            {
                return;
            }

            // 스왑 진행
            (_heap[current], _heap[index]) = (_heap[index], _heap[current]);
            index = current; //바뀐 부분만 다시 진행하면 됨
        }
    }
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
        shortestPath.Clear();
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
        
       
    }

    public void BreadthFirstSearch()
    {
        if (startNode == null && destinationNode == null)
        {
            Debug.Log("BreadthFirstSearch: startNode or destinationNode is null");
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
                if (!came_from.ContainsKey(neighbor)) // 도착하지 않은 노드가 있다면, 여기 고쳐야됨!!!!!!!!!!!!!!!!!!!!!!!!!! JeilNode에 약간의 변화 필요 변수 하나 isVisited 정도?
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
        shortestPath.Add(startNode);
        shortestPath.Reverse();
    }

    public void Dijkstra()
    {
        if (startNode == null && destinationNode == null)
        {
            Debug.Log("Dijkstra: startNode or destinationNode is null");
            return; 
        }
        //시작 노드랑 끝 노드 없을때 오류 방지
        
        //******************************이 이하로 아직 안만듦************************************
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
                if (!came_from.ContainsKey(neighbor)) // 도착하지 않은 노드가 있다면, 여기 고쳐야됨!!!!!!!!!!!!!!!!!!!!!!!!!! JeilNode에 약간의 변화 필요 변수 하나 isVisited 정도?
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
        shortestPath.Add(startNode);
        shortestPath.Reverse();
    }

    public void Astar()
    {
        
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    [Header("UI")] 
    public GameObject ui;
    public GameObject costInputField;
    public GameObject nodeSubMenu;
    
    [Header("Debug")]
    [SerializeField] private JeilNode holdingStartNode;
    [SerializeField] private JeilNode holdingEndNode;
    [SerializeField] private JeilEdge selectedEdge;
    [SerializeField] private JeilNode selectedNode;
    [SerializeField] private JeilNode movingNode;
    
    // Node를 Holding하고 있다는건 우클릭 통해서 노드 생성 후
    // 목적지 노드를 '잡고' 있다는 뜻임. Moving과 혼동 주의!
    bool IsHoldingNode()
    {
        return (holdingEndNode != null);
    }
    
    // 노드를 Moving 하고 있다는건 우클릭으로 노드를 클릭하고
    // 노르를 옮기고 있다는 뜻임. Holding과 혼동 주의!
    bool IsMovingNode()
    {
        return (movingNode != null);
    }
    
    private void Update()
    {
        HoldingMovingNode();
    }

    private void OnDisable()
    {
        GameManager.obj.pinput.eventRightClick -= RightClick;
        GameManager.obj.pinput.eventClick      -= LeftClick;
        
        GameManager.obj.pinput.eventDelete     -= Delete;
        
        GameManager.obj.pinput.eventCancel     -= Cancel;
        
        ui.SetActive(false);
    }
    
    private void OnEnable()
    {
        GameManager.obj.pinput.eventRightClick += RightClick;
        GameManager.obj.pinput.eventClick      += LeftClick;
        
        GameManager.obj.pinput.eventDelete     += Delete;
        
        GameManager.obj.pinput.eventCancel     += Cancel;
        
        GameManager.obj.state = GameManager.GameState.Editing;
        
        ui.SetActive(true);
    }

    void HoldingMovingNode()
    {
        if (IsHoldingNode())
        {
            holdingEndNode.transform.position = GameManager.MousePosition();
        }
        if (IsMovingNode())
        {
            movingNode.transform.position = GameManager.MousePosition();
        }
    }
    
    // eventDelete에 할당되는 함수.
    public void Delete()
    {
        // 노드에 마우스 갖다댔으면
        JeilNode mouseOnNode = GameManager.NodeOnMouse();
        if (mouseOnNode != null)
        {
            DeleteNode(mouseOnNode); // 그 노드 지워버림
        }
    }
    
    // eventRightClick에 할당되는 함수.
    public void RightClick()
    {
        // Moving 중일때는 노드를 그 자리에 둠.
        if (IsMovingNode())
        {
            movingNode = null;
            return;
        }
        JeilNode clickedNode = GameManager.NodeOnMouse();
        // Holding 하고 있을때 우클릭 누르면
        // 기존에 있는 노드나 에지에 우클릭 했으면 그 노드에 연결하고 끝
        // 허공에 우클릭 했으면 노드 하나를 그 자리에 둔 후 그자리에서 부터 새 노드 이음
        if (IsHoldingNode())
        {
            // 노드 위에 있으면
            if (clickedNode != null)
            {
                Destroy(holdingEndNode.gameObject);
                ConnectNodes(holdingStartNode, clickedNode);
                holdingStartNode = null;
                Debug.Log("Finished connecting to existing Node in right click");
            }
            else // 허공이면
            {
                ConnectNodes(holdingStartNode, holdingEndNode);
                holdingStartNode = holdingEndNode;
                holdingEndNode = CreateNode(GameManager.MousePosition());
                Debug.Log("Continuing connecting node from right click");
            }
            
            return;
        }

        if (clickedNode != null)
        {
            if (GameManager.obj.pinput.ctrl)
            {
                holdingStartNode = clickedNode;
                holdingEndNode = CreateNode(GameManager.MousePosition());
            }
            else
            {
                movingNode = clickedNode;
                Debug.Log("Selected Node to move");
            }
            return;
        }

        // Holding, Moving 다 아니면 노드 새로 시작
        Debug.Log("Created New start and end node");
        holdingStartNode = CreateNode(GameManager.MousePosition());
        holdingEndNode = CreateNode(GameManager.MousePosition());
    }
    
    // eventClick에 할당되는 함수
    public void LeftClick()
    {
        // Holding 상태면 마우스 위치에 노드 고정하고, 연결하고 Holding 종료함.
        if (IsHoldingNode())
        {
            JeilNode clickedNode = GameManager.NodeOnMouse();
            if (clickedNode != null)
            {
                Destroy(holdingEndNode.gameObject);
                ConnectNodes(holdingStartNode, clickedNode);
                holdingStartNode = null;
                Debug.Log("Finished connecting to existing Node on left click");
                return;
            }
            ConnectNodes(holdingStartNode, holdingEndNode);
            holdingEndNode = null;
            holdingStartNode = null;
            Debug.Log("Finished connecting node from Left Click");
            return;
        }
        
        // 엣지 클릭했을 떄 그 엣지의 비용 수정
        Collider2D raycasted = Physics2D.OverlapPoint(GameManager.MousePosition(), GameManager.obj.layerEdge|GameManager.obj.layerNode);
        if (raycasted != null)
        {
            if (raycasted.gameObject.layer == GameManager.GetRealLayer(GameManager.obj.layerEdge))
            {
                selectedEdge = raycasted.gameObject.GetComponentInParent<JeilEdge>();
                ActivateEdgeSubmenu();
            }
            else
            {
                selectedNode = raycasted.gameObject.GetComponentInParent<JeilNode>();
                ActivateNodeSubmenu();
            }
        }
    }
    
    public void Cancel()
    {
        DeselectNodeSubmenu();
        DeselectInputField();
    }

    public JeilNode CreateNode(Vector2 pos)
    {
        return Instantiate(GameManager.obj.prefabNode, pos, Quaternion.identity, GameManager.obj.poolNode.transform).GetComponent<JeilNode>();
    }

    public void DeleteNode(JeilNode what)
    {
        foreach (JeilNode neighbor in what.neighbors)
        {
            if (neighbor == null)
                break;
            Destroy(what.neighborEdges[neighbor].gameObject);
        }
        Destroy(what.gameObject);
    }
    
    public void ConnectNodes(JeilNode what1, JeilNode what2)
    {
        Debug.Log("Connecting from what1 to what2 ");
        what1.neighbors.Add(what2);
        what2.neighbors.Add(what1);
        
        what1.gameObject.layer = GameManager.GetRealLayer(GameManager.obj.layerNode);
        what2.gameObject.layer = GameManager.GetRealLayer(GameManager.obj.layerNode);

        GameObject _edge = Instantiate(GameManager.obj.prefabEdge, (what1.transform.position + what2.transform.position) / 2, Quaternion.identity, GameManager.obj.poolEdge.transform);

        JeilEdge edge = _edge.GetComponent<JeilEdge>();
        edge.ConnectNodes(what1, what2);
        what1.neighborEdges[what2] = edge;
        what2.neighborEdges[what1] = edge;
    }

    public void ActivateEdgeSubmenu()
    {
        costInputField.transform.position = GameManager.obj.pinput.mousePosition;
        costInputField.SetActive(true);
        
        GameManager.obj.pinput.enabled = false;
        
        TMP_InputField inputField = costInputField.GetComponentInChildren<TMP_InputField>();
        inputField.ActivateInputField();
    }

    public void UpdateSelectedEdge()
    {
        selectedEdge.SetCost(int.Parse(costInputField.GetComponentInChildren<TMP_InputField>().text));
    }

    public void DeselectInputField()
    {
        costInputField.GetComponentInChildren<TMP_InputField>().DeactivateInputField();
        costInputField.SetActive(false);
        selectedEdge = null;
        GameManager.obj.pinput.enabled = true;
    }
    
    public void ActivateNodeSubmenu()
    {
        nodeSubMenu.transform.position = GameManager.obj.pinput.mousePosition;
        nodeSubMenu.SetActive(true);
        GameManager.obj.pinput.enabled = false;
    }
    
    public void UpdateSelectedNode()
    {
        selectedNode.visibleInPathfinding = nodeSubMenu.GetComponentInChildren<Toggle>().isOn;
    }

    public void DeselectNodeSubmenu()
    {
        nodeSubMenu.SetActive(false);
        selectedNode = null;
        GameManager.obj.pinput.enabled = true;
    }

    public void SetToStartNode()
    {
        GameManager.obj.managerPathfinding.startNode = selectedNode;
    }
    
    public void SetToDestinationNode()
    {
        GameManager.obj.managerPathfinding.destinationNode = selectedNode;
    }
}

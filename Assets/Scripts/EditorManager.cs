using TMPro;
using UnityEngine;

public class EditorManager : MonoBehaviour
{
    [Header("UI")] 
    public GameObject costInputField;
    [Header("Debug")]
    [SerializeField] private JeilNode holdingStartNode;
    [SerializeField] private JeilNode holdingEndNode;
    [SerializeField] private JeilEdge selectedEdge;
    [SerializeField] private JeilNode movingNode;
    
    bool IsHoldingNode()
    {
        return (holdingEndNode != null);
    }
    
    bool IsMovingNode()
    {
        return (movingNode != null);
    }
    
    private void Update()
    {
        HoldingMovingNode();
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
    
    
    
    public void RightClick()
    {
        if (IsMovingNode())
        {
            movingNode = null;
            return;
        }
        if (IsHoldingNode())
        {
            JeilNode clickedNode = GameManager.NodeOnMouse();
            if (clickedNode != null)
            {
                Destroy(holdingEndNode.gameObject);
                GameManager.ConnectNodes(holdingStartNode, clickedNode);
                holdingStartNode = null;
                Debug.Log("Finished connecting to existing Node in right click");
            }
            else
            {
                GameManager.ConnectNodes(holdingStartNode, holdingEndNode);
                holdingStartNode = holdingEndNode;
                holdingEndNode = Instantiate(GameManager.obj.prefabNode, GameManager.MousePosition(), Quaternion.identity, GameManager.obj.poolNode.transform).GetComponent<JeilNode>();
                Debug.Log("Continuing connecting node from right click");
            }
            
            return;
        }
        else
        {
            JeilNode targetNode = GameManager.NodeOnMouse();
            if (targetNode != null)
            {
                movingNode = targetNode;
                Debug.Log("Selected Node to move");
                return;
            }
        }
        
        Debug.Log("Created New start and end node");
        holdingStartNode = Instantiate(GameManager.obj.prefabNode, GameManager.MousePosition(), Quaternion.identity, GameManager.obj.poolNode.transform).GetComponent<JeilNode>();
        holdingEndNode = Instantiate(GameManager.obj.prefabNode, GameManager.MousePosition(), Quaternion.identity, GameManager.obj.poolNode.transform).GetComponent<JeilNode>();
    }
    
    public void LeftClick()
    {
        if (IsHoldingNode())
        {
            JeilNode clickedNode = GameManager.NodeOnMouse();
            if (clickedNode != null)
            {
                Destroy(holdingEndNode.gameObject);
                GameManager.ConnectNodes(holdingStartNode, clickedNode);
                holdingStartNode = null;
                Debug.Log("Finished connecting to existing Node on left click");
                return;
            }
            GameManager.ConnectNodes(holdingStartNode, holdingEndNode);
            holdingEndNode = null;
            holdingStartNode = null;
            Debug.Log("Finished connecting node from Left Click");
            return;
        }
        
        Collider2D raycasted = Physics2D.OverlapPoint(GameManager.MousePosition(), GameManager.obj.layerEdge|GameManager.obj.layerNode);
        if (raycasted != null)
        {
            if (raycasted.gameObject.layer == GameManager.GetRealLayer(GameManager.obj.layerNode))
            {
                
            }
            else
            {
                selectedEdge = raycasted.gameObject.GetComponentInParent<JeilEdge>();
                costInputField.transform.position = GameManager.obj.pinput.mousePosition;
                costInputField.SetActive(true);
                TMP_InputField inputField = raycasted.GetComponentInChildren<TMP_InputField>();
                inputField.ActivateInputField();
            }
        }
    }

    public void UpdateSelectedEdge()
    {
        selectedEdge.SetCost(int.Parse(costInputField.GetComponentInChildren<TMP_InputField>().text));
    }

    public void DeselectInputField()
    {
        costInputField.SetActive(false);
        selectedEdge = null;
    }
}

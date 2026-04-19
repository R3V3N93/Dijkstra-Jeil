using System;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    static public GameManager obj;
    public InputSO pinput;
    public Camera playerCamera;
    
    enum GameState
    {
        PathFinding,
        Editing
    };
    [Header("Program")] 
    [SerializeField] GameState state = GameState.PathFinding;

    [Header("Prefabs")] public GameObject prefabNode;
    public GameObject prefabEdge;
    
    [Header("Pools")]
    public GameObject poolNode;
    public GameObject poolEdge;
    
    [Header("Settings")] 
    [Tooltip("Max amount to scroll in orthographic")]
    [SerializeField] private uint maxScroll = 50;
    [SerializeField] private float panningSpeed = 0.05f;
    
    [Header("Layers")]
    public LayerMask layerNode;
    public LayerMask layerNodeHeld;
    public LayerMask layerEdge;

    [Header("Managers")] 
    [SerializeField] private EditorManager managerEditor;
    [SerializeField] private PathfindingManager managerPathfinding;

    [Header("Debug")] 
    [SerializeField] private List<GameObject> undoBuffer;

    public void ToggleState()
    {
        if (state == GameState.PathFinding)
        {
            state = GameState.Editing;
            pinput.eventRightClick += managerEditor.RightClick;
            pinput.eventClick      += managerEditor.LeftClick;
            
            pinput.eventRightClick -= managerPathfinding.RightClick;
            pinput.eventClick      -= managerPathfinding.LeftClick;

            managerEditor.gameObject.SetActive(false);
            managerPathfinding.gameObject.SetActive(true);
        }
        else
        {
            state = GameState.PathFinding;
            pinput.eventRightClick -= managerEditor.RightClick;
            pinput.eventClick      -= managerEditor.LeftClick;
                
            pinput.eventRightClick += managerPathfinding.RightClick;
            pinput.eventClick      += managerPathfinding.LeftClick;
            
            managerPathfinding.gameObject.SetActive(false);
            managerEditor.gameObject.SetActive(true);
        }
        Debug.Log("Toggled state");
    }
    
    void Awake()
    {
        if(obj == null) obj = this;
        else Destroy(this);
        
        pinput.eventRightClick += managerPathfinding.RightClick;
        pinput.eventClick += managerPathfinding.LeftClick;
    }

    void Update()
    {
        Scroll();
        Panning();
    }
    
    void Scroll()
    {
        if (pinput.scroll.sqrMagnitude > 0)
        {
            playerCamera.orthographicSize -= pinput.scroll.y;
            playerCamera.orthographicSize = Mathf.Clamp(playerCamera.orthographicSize, 1, maxScroll);
        }
    }

    void Panning()
    {
        if (pinput.middleClicked)
        {
            playerCamera.transform.position += -new Vector3(pinput.mouseDelta.x, pinput.mouseDelta.y, 0) * panningSpeed;
        }
    }

    static public Vector3 MousePosition()
    {
        return obj.playerCamera.ScreenToWorldPoint(new Vector3(obj.pinput.mousePosition.x, obj.pinput.mousePosition.y, obj.playerCamera.nearClipPlane));
    }

    static public JeilNode NodeOnMouse()
    {
        Collider2D raycasted = Physics2D.OverlapPoint(MousePosition(), obj.layerNode);
        if (raycasted != null)
        {
            JeilNode node = raycasted.gameObject.GetComponent<JeilNode>();
            if (node != null)
            {
                return node;
            }
        }
        
        return null;
    }


    

    static public int GetRealLayer(LayerMask from)
    {
        return (int)Math.Log(from.value, 2);
    }
    
    static public void ConnectNodes(JeilNode what1, JeilNode what2)
    {
        Debug.Log("Connecting from what1 to what2 ");
        what1.neighbors.Add(what2);
        what2.neighbors.Add(what1);
        
        what1.gameObject.layer = GetRealLayer(obj.layerNode);
        what2.gameObject.layer = GetRealLayer(obj.layerNode);

        GameObject _edge = Instantiate(obj.prefabEdge, (what1.transform.position + what2.transform.position) / 2, Quaternion.identity, obj.poolEdge.transform);

        JeilEdge edge = _edge.GetComponent<JeilEdge>();
        edge.ConnectNodes(what1, what2);
        what1.neighborEdges[what2] = edge;
        what2.neighborEdges[what1] = edge;
    }
}

using System;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    static public GameManager obj;
    public InputSO pinput;
    public Camera playerCamera;
    
    public enum GameState
    {
        PathFinding,
        Editing
    };
    [Header("Program")] 
    public GameState state = GameState.PathFinding;

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
    public EditorManager managerEditor;
    public PathfindingManager managerPathfinding;

    [Header("Debug")] 
    [SerializeField] private List<GameObject> undoBuffer;

    public void ToggleState()
    {
        if (state == GameState.PathFinding)
        {
            managerEditor.gameObject.SetActive(true);
            managerPathfinding.gameObject.SetActive(false);
        }
        else
        {
            managerPathfinding.gameObject.SetActive(true);
            managerEditor.gameObject.SetActive(false);
        }
        Debug.Log("Toggled state");
    }
    
    void Awake()
    {
        if(obj == null) obj = this;
        else Destroy(this);
        
        pinput.eventRightClick += managerPathfinding.RightClick;
        pinput.eventClick += managerPathfinding.LeftClick;
        
        managerPathfinding.gameObject.SetActive(true);
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

    static public Vector2 MousePosition()
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
}

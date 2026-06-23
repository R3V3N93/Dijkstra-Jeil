using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum GameState
{
    PathFinding,
    Editing
};

[Serializable]
public struct PoolsT
{
    public Transform node;
    public Transform edge;
}
[Serializable]
public struct ManagerT
{
    public EditorManager editor;
    public PathfindingManager pathFinding;
}

public class GameManager : MonoBehaviour
{
    static public GameManager obj;
    public InputSO pinput;
    public Camera playerCamera;

    [Header("Data")]
    public LayerSO layers;
    public PrefabSO prefabs;
    
    [Header("Program")] 
    public GameState state = GameState.PathFinding;
    
    [Header("Pointers")]
    public PoolsT pools;
    public ManagerT managers;
    
    [Header("Settings")] 
    [Tooltip("Max amount to scroll in orthographic")]
    [SerializeField] private uint maxScroll = 50;
    [SerializeField] private float panningSpeed = 0.05f;

    [Header("Debug")] 
    [SerializeField] private List<GameObject> undoBuffer;
    public Vector2 mousePositionOld;

    public void ToggleState()
    {
        if (state == GameState.PathFinding)
        {
            managers.editor.gameObject.SetActive(true);
            managers.pathFinding.ClearPath();
            managers.pathFinding.gameObject.SetActive(false);
        }
        else
        {
            managers.pathFinding.gameObject.SetActive(true);
            managers.editor.gameObject.SetActive(false);
        }
        Debug.Log("Toggled state");
    }
    
    void Awake()
    {
        if(obj == null) obj = this;
        else Destroy(this);
        
        managers.pathFinding.gameObject.SetActive(true);
    }

    void Update()
    {
        Scroll();
        Panning();

        if (!pinput.shift)
        {
            mousePositionOld = pinput.mousePosition;
        }
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

    static public Vector2 Screen2World(Vector2 from)
    {
        return obj.playerCamera.ScreenToWorldPoint(from);
    }

    static public Vector2 MousePosition()
    {
        Vector3 screenPosition = new Vector3(obj.pinput.mousePosition.x, obj.pinput.mousePosition.y, obj.playerCamera.nearClipPlane);
        if (obj.pinput.shift && obj.pinput.mouseDelta.x != obj.pinput.mouseDelta.y)
        {
            screenPosition.x -= obj.pinput.mouseDelta.x;
            screenPosition.y -= obj.pinput.mouseDelta.y;

            if (obj.pinput.mouseDelta.x > obj.pinput.mouseDelta.y)
                screenPosition.x += obj.pinput.mouseDelta.x;
            else screenPosition.y += obj.pinput.mouseDelta.y;
        }
        return Screen2World(screenPosition);
    }

    static public JeilElement GetElementOnMouse()
    {
        Collider2D raycasted = Physics2D.OverlapPoint(MousePosition(), obj.layers.node|obj.layers.edge);
        if (raycasted != null)
        {
            JeilElement element = raycasted.gameObject.GetComponent<JeilElement>();
            return element;
        }
        return null;
    }

    static public int GetRealLayer(LayerMask from)
    {
        return (int)Math.Log(from.value, 2);
    }

    static public List<JeilNode> GetNodes()
    {
        List<JeilNode> temp = new List<JeilNode>();
        for (int i = 0; i < GameManager.obj.pools.node.childCount; i++)
        {
            Transform child = GameManager.obj.pools.node.GetChild(i);
            JeilNode nodeFromChild = child.GetComponent<JeilNode>();
            temp.Add(nodeFromChild);
        }

        return temp;
    }
    
    static public List<JeilEdge> GetEdges()
    {
        List<JeilEdge> temp = new List<JeilEdge>();
        for (int i = 0; i < GameManager.obj.pools.edge.childCount; i++)
        {
            Transform child = GameManager.obj.pools.edge.GetChild(i);
            JeilEdge edgeFromChild = child.GetComponent<JeilEdge>();
            temp.Add(edgeFromChild);
        }

        return temp;
    }
}

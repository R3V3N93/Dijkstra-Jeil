using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum SelectionModeT
{
    Node,
    Edge
};

[Serializable]
public class PropertyMenu
{
    public GameObject bg; // Also is a root
    public NodePropertyMenu node;
    public EdgePropertyMenu edge;

    public void CloseAll()
    {
        if(bg) bg.SetActive(false);
        node.Disable();
        edge.Disable();
    }
    
    

    public void Activate(SelectionModeT mode, List<JeilElement> selections)
    {
        CloseAll();
        if(selections.Count == 0) return;

        bg.SetActive(true);
        
        switch (mode)
        {
            case SelectionModeT.Node:
                node.Activate(selections);
                break;
            case SelectionModeT.Edge:
                edge.Activate(selections);
                break;
        }
    }
}

public class EditorManager : MonoBehaviour
{
    enum StatesT
    {
        Selecting,
        Connecting,
        Moving
    };
    
    [SerializeField] private SelectionModeT selectionMode = SelectionModeT.Node;
    [SerializeField] private StatesT state = StatesT.Selecting;
    public List<JeilElement> selections = new List<JeilElement>();
    [Header("UI")] 
    public GameObject ui;
    public PropertyMenu menu;

    [Header("Connecting")] 
    [SerializeField] private JeilNode connectStart;
    [SerializeField] private JeilNode connectEnd;
    
    [Header("Selecting")]
    [SerializeField] private Rect dragRect = Rect.zero;
    [SerializeField] private bool isDragging;
    [SerializeField] private Texture2D dragImage;
    [SerializeField] private Texture2D testImage;
    
    [Header("Moving")]
    [SerializeField] private Vector2 oldMouse = Vector2.zero;
    
    private void Update()
    {
        switch (state)
        {
            case StatesT.Selecting:
                break;
            case StatesT.Connecting:
                if(connectEnd != null) connectEnd.transform.position = GameManager.MousePosition();
                break;
            case StatesT.Moving:
                if (selections.Count > 0)
                {
                    Vector3 dMove = GameManager.MousePosition() - oldMouse;
                    foreach (JeilElement sel in selections)
                    {
                        sel.transform.position += dMove;
                    }
                    oldMouse = GameManager.MousePosition();
                }
                break;
        }
    }

    private void OnGUI()
    {
        UpdateDragging();
    }

    private void OnDisable()
    {
        GameManager.obj.pinput.eventRightClick -= RightClick;
        GameManager.obj.pinput.eventClickOn      -= LeftClickOn;
        GameManager.obj.pinput.eventClickOff      -= LeftClickOff;
        
        GameManager.obj.pinput.eventDelete     -= Delete;
        
        GameManager.obj.pinput.eventCancel     -= Cancel;
        
        ClosePropertyMenu(true);

        Deselect();
        
        ui.SetActive(false);
    }
    
    private void OnEnable()
    {
        GameManager.obj.pinput.eventRightClick += RightClick;
        GameManager.obj.pinput.eventClickOn      += LeftClickOn;
        GameManager.obj.pinput.eventClickOff      += LeftClickOff;
        
        GameManager.obj.pinput.eventDelete     += Delete;
        
        GameManager.obj.pinput.eventCancel     += Cancel;
        
        GameManager.obj.state = GameState.Editing;
        
        foreach (JeilEdge edge in GameManager.GetEdges())
        {
            edge.graphics.SetActive(true);
        }

        foreach (JeilNode node in GameManager.GetNodes())
        {
            node.sprite.enabled = true;
        }
        
        ui.SetActive(true);
    }
    
    public void Delete()
    {
        JeilElement elem = GameManager.GetElementOnMouse();
        if(elem is JeilNode)
            DeleteNode(elem as JeilNode);
    }

    public void Deselect()
    {
        ClosePropertyMenu(); // setting it to true will cause infinite regressions btw. I don't wanna try it.
        foreach (JeilElement elem in selections)
        {
            elem.SetOutline(false);
            //if(elem is JeilNode) ((JeilNode)elem).SetOutline(false);
            //if(elem is JeilEdge) ((JeilEdge)elem).SetOutline(false);
        }

        selections.Clear();
    }
    
    public void LeftClickOn()
    {
        switch (state)
        {
            case StatesT.Selecting:
                if(!GameManager.obj.pinput.ctrl)
                    StartDragging();
                break;
            case StatesT.Connecting:
                break;
            case StatesT.Moving:
                break;
        }
        
    }
    public void LeftClickOff()
    {
        JeilElement elem = GameManager.GetElementOnMouse();
        switch (state)
        {
            case StatesT.Selecting:
                StopDragging(elem);
                if (GameManager.obj.pinput.ctrl && elem)
                {
                    ClosePropertyMenu();
                    if (selections.Contains(elem))
                    {
                        selections.Remove(elem);
                        elem.SetOutline(false);
                    }
                    else
                    {
                        if (elem.gameObject.layer != GameManager.GetRealLayer(GetSelectedModeLayer())) break;
                        selections.Add(elem);
                        elem.SetOutline(true);
                    }
                }
                break;
            case StatesT.Connecting:
                if (elem == null || elem is JeilEdge)
                {
                    connectEnd.Unhold();
                    ConnectNodes(connectStart, connectEnd);
                    connectStart = null;
                    connectEnd = null;
                    state = StatesT.Selecting;
                    break;
                }

                if (elem is JeilNode)
                {
                    DeleteNode(connectEnd);
                    ConnectNodes(connectStart, (JeilNode)elem);
                    connectStart = null;
                    connectEnd = null;
                    state = StatesT.Selecting;
                    break;
                }
                break;
            case StatesT.Moving:
                break;
        }
        
    }
    
    public void RightClick()
    {
        JeilElement elem = GameManager.GetElementOnMouse();
        switch (state)
        {
            case StatesT.Selecting:
                if (selections.Count > 0)
                {
                    if (GameManager.obj.pinput.ctrl)
                    {
                        state = StatesT.Moving;
                        oldMouse = GameManager.MousePosition();
                    }
                    else
                    {
                        OpenPropertyMenu();
                    }
                    break;
                }
                
                if (elem == null)
                {
                    state = StatesT.Connecting;
                    connectStart = CreateNode(GameManager.MousePosition());
                    connectEnd = CreateNode(GameManager.MousePosition());
                    connectEnd.Hold();
                    break;
                }
                if (elem is JeilNode)
                {
                    if (GameManager.obj.pinput.ctrl)
                    {
                        state = StatesT.Connecting;
                        connectStart = (JeilNode)elem;
                        connectEnd = CreateNode(GameManager.MousePosition());
                        connectEnd.Hold();
                    }
                }
                break;
            case StatesT.Connecting:
                if (elem == null || elem is JeilEdge)
                {
                    ConnectNodes(connectStart, connectEnd);
                    connectEnd.Unhold();
                    connectStart = connectEnd;
                    connectEnd = CreateNode(GameManager.MousePosition());
                    connectEnd.Hold();
                    break;
                }

                if (elem is JeilNode)
                {
                    DeleteNode(connectEnd);
                    ConnectNodes(connectStart, (JeilNode)elem);
                    
                    connectStart = null;
                    connectEnd = null;
                    state = StatesT.Selecting;
                    break;
                }
                break;
            case StatesT.Moving:
                state = StatesT.Selecting;
                break;
        }
    }

    private void StartDragging()
    {
        dragRect.Set(GameManager.obj.pinput.mousePosition.x, Screen.height - GameManager.obj.pinput.mousePosition.y, 0, 0);
        isDragging = true;
    }

    private void UpdateDragging()
    {
        if (!isDragging) return;
        float width = GameManager.obj.pinput.mousePosition.x - dragRect.x;
        float height = (Screen.height - GameManager.obj.pinput.mousePosition.y) - dragRect.y;
        dragRect.Set(dragRect.x, dragRect.y, width, height);
        GUI.DrawTexture(dragRect, dragImage, ScaleMode.StretchToFill, true);
    }
    
    private void StopDragging(JeilElement ElemOnMouse)
    {
        if (!isDragging) return;
        
        if (Mathf.Abs(dragRect.width * dragRect.height) < 10)
        {
            if (ElemOnMouse)
            {
                Deselect();
                if (ElemOnMouse.gameObject.layer == GameManager.GetRealLayer(GetSelectedModeLayer()))
                {
                    selections.Add(ElemOnMouse);
                    ElemOnMouse.SetOutline(true);
                }
            }
            isDragging = false;
            dragRect = Rect.zero;
            return;
        }
        
        Deselect();

        Vector2 min = dragRect.min;
        Vector2 max = dragRect.max;

        min.y = Screen.height-min.y;
        max.y = Screen.height-max.y;
        
        min = GameManager.Screen2World(min);
        max = GameManager.Screen2World(max);
        
        Debug.DrawLine(min, max,  Color.red, 1f);
        Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max, layerMask: GetSelectedModeLayer());
        
        isDragging = false;
        dragRect = Rect.zero;

        if (colliders.Length == 0) return;

        foreach (Collider2D col in colliders)
        {
            JeilElement element =  col.gameObject.GetComponent<JeilElement>();
            element.SetOutline(true);
            selections.Add(element);
        }
    }

    public void Cancel()
    {
        ClosePropertyMenu(false);
    }

    public JeilNode CreateNode(Vector2 pos, int index = -1, bool landmark = false, int layer = 0)
    {   
        JeilNode product = Instantiate(GameManager.obj.prefabs.node, pos, Quaternion.identity, GameManager.obj.pools.node.transform).GetComponent<JeilNode>();
        if(index != -1 && index >= 0)
            product.index = index;
        product.visibleInPathfinding = landmark;
        product.layer = (uint)layer;
        return product;
    }

    public void DeleteNode(JeilNode what)
    {
        foreach (JeilNode neighbor in what.neighbors)
        {
            if (neighbor == null)
                continue;
            // 후에 반드시 Destroy()으로 바뀌어야함 !!!!!!!!!!!!!!!!!!!!
            DestroyImmediate(what.neighborEdges[neighbor].gameObject);
            neighbor.neighbors.Remove(what);
        }
        DestroyImmediate(what.gameObject);
    }
    
    public void ConnectNodes(JeilNode what1, JeilNode what2, int cost = 1, bool visible = true)
    {
        Debug.Log("Connecting from what1 to what2 ");
        if(!what1 || !what2)
            return;

        if(what1.neighbors.Contains(what2))
            return;

        what1.neighbors.Add(what2);
        what2.neighbors.Add(what1);
        
        what1.gameObject.layer = GameManager.GetRealLayer(GameManager.obj.layers.node);
        what2.gameObject.layer = GameManager.GetRealLayer(GameManager.obj.layers.node);

        GameObject _edge = Instantiate(GameManager.obj.prefabs.edge, (what1.transform.position + what2.transform.position) / 2, Quaternion.identity, GameManager.obj.pools.edge.transform);

        JeilEdge edge = _edge.GetComponent<JeilEdge>();
        edge.SetCost(cost);
        edge.ConnectNodes(what1, what2);
        edge.visibleInPathfinding = visible;
        what1.neighborEdges[what2] = edge;
        what2.neighborEdges[what1] = edge;
    }

    public void OpenPropertyMenu()
    {
        menu.Activate(selectionMode, selections);
    }

    public void ClosePropertyMenu(bool clearSelected = false)
    {
        if (clearSelected) Deselect();
        menu.CloseAll();
    }

    public void SetToStartNode()
    {
        if (selections.Count != 1) return;
        JeilElement selected = selections[0];
        
        if (selected is not JeilNode) return;
        
        if ((JeilNode)selected == GameManager.obj.managers.pathFinding.destinationNode) return;
        
        if(GameManager.obj.managers.pathFinding.startNode != null) 
            GameManager.obj.managers.pathFinding.startNode.SetColour(Color.red);
        
        GameManager.obj.managers.pathFinding.startNode = (JeilNode)selected;
        
        ((JeilNode)selected).SetColour(Color.lawnGreen);
    }
    
    public void SetToDestinationNode()
    {
        if (selections.Count != 1) return;
        JeilElement selected = selections[0];
        
        if (selected is not JeilNode) return;
        
        if ((JeilNode)selected == GameManager.obj.managers.pathFinding.startNode) return;
        
        if(GameManager.obj.managers.pathFinding.destinationNode != null) 
            GameManager.obj.managers.pathFinding.destinationNode.SetColour(Color.red);
        
        GameManager.obj.managers.pathFinding.destinationNode = (JeilNode)selected;
        
        ((JeilNode)selected).SetColour(Color.blue);
    }

    public void ToggleLandmark(bool toggle)
    {
        foreach (JeilElement selected in selections)
        {
            if (selected is not JeilNode) return;
            JeilNode sel =  selected as JeilNode;
            sel.visibleInPathfinding = toggle;
        }
    }

    public void ToggleEdgeVisibility(bool toggle)
    {
        foreach (JeilElement selected in selections)
        {
            if (selected is not JeilEdge) return;
            JeilEdge sel =  selected as JeilEdge;
            sel.visibleInPathfinding = toggle;
        }
    }
    
    public void SetEdgeCost(string to) // Stupid ngl. I just solely want int input.
    {
        foreach (JeilElement selected in selections)
        {
            if (selected is not JeilEdge) return;
            JeilEdge sel = selected as JeilEdge;
            int parsed = 0;
            if (int.TryParse(to, out parsed))
                sel.SetCost(parsed);
        }
    }
    
    public void SetNodeLayer(string to)
    {
        foreach (JeilElement selected in selections)
        {
            if (selected is not JeilNode) return;
            JeilNode sel = selected as JeilNode;
            int parsed = 0;
            if (int.TryParse(to, out parsed))
                sel.layer = (uint)parsed;
        }
    }

    public void SetSelectionMode(int to)
    {
        Deselect();
        ClosePropertyMenu();
        selectionMode = (SelectionModeT)to;
    }

    public LayerMask GetSelectedModeLayer()
    {
        return selectionMode == SelectionModeT.Node ? GameManager.obj.layers.node : GameManager.obj.layers.edge;
    }
}

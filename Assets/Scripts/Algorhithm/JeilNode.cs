using UnityEngine;
using System.Collections.Generic;

public class JeilNode : JeilElement
{
    public int index = -1;
    public List<JeilNode> neighbors = new List<JeilNode>();
    public Dictionary<JeilNode, JeilEdge> neighborEdges = new Dictionary<JeilNode, JeilEdge>();
    public bool visibleInPathfinding = false;
    public GameObject outline;
    public SpriteRenderer sprite;

    public bool IsStartNode()
    {
        return GameManager.obj.managers.pathFinding.startNode == this;
    }
    
    public bool IsDestinationNode()
    {
        return GameManager.obj.managers.pathFinding.destinationNode == this;
    }
    
    public void SetColour(Color to)
    {
        if(!sprite) Debug.LogError("SpriteRenderer doesn't exist for this!");
        sprite.color = to;
    }

    public void SetOutline(bool to)
    {
        outline.SetActive(to);
    }
    
    public void Hold()
    {
        gameObject.layer = GameManager.GetRealLayer(GameManager.obj.layers.nodeHeld);
        SetOutline(true);
    }
    
    public void Unhold()
    {
        gameObject.layer = GameManager.GetRealLayer(GameManager.obj.layers.node);
        SetOutline(false);
    }
}
using UnityEngine;
using System.Collections.Generic;

public class JeilNode : JeilElement
{
    public int index = -1;
    public List<JeilNode> neighbors = new List<JeilNode>();
    public Dictionary<JeilNode, JeilEdge> neighborEdges = new Dictionary<JeilNode, JeilEdge>();
    public bool visibleInPathfinding = false;
}
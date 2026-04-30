using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

class SaveManager : MonoBehaviour
{
    public string fileName = "config.json";
    public string path = Application.dataPath + "/";
    public void Save()
    {
        // Set index for each node for easier classification from JSON
        for (int i = 0; i < GameManager.obj.poolNode.transform.childCount; i++)
        {
            Transform child = GameManager.obj.poolNode.transform.GetChild(i);
            
            JeilNode nodeFromChild = child.GetComponent<JeilNode>();

            nodeFromChild.index = i;
        }
        
        NodeSaveDataArray dataArray =  new NodeSaveDataArray();
        // Now as we have indexes, we can save them into Json
        foreach (Transform child in GameManager.obj.poolNode.transform)
        {
            JeilNode nodeFromChild = child.GetComponent<JeilNode>();
            
            NodeSaveData datum  = NodeSaveData.Create(nodeFromChild);
            dataArray.data.Add(datum);
        }

        File.WriteAllText(path + fileName, JsonUtility.ToJson(dataArray));
        Debug.Log("Saved config to " + path);
    }
    
    public void Load()
    {
        NodeSaveDataArray dataArray = new NodeSaveDataArray();
        string JsonRawString = File.ReadAllText(path + fileName);
        
        JsonUtility.FromJsonOverwrite(JsonRawString, dataArray);

        EditorManager editor = GameManager.obj.managerEditor;

        foreach(Transform purged in GameManager.obj.poolNode.transform)
        {
            JeilNode purgedNode = purged.GetComponent<JeilNode>();
            editor.DeleteNode(purgedNode);
        }
            
        int curIndex = 0;
        foreach (NodeSaveData datum in dataArray.data)
        {
            JeilNode createdNode = editor.CreateNode(datum.pos, curIndex);
            for(int i = 0; i < datum.neighborIndexes.Count; i++)
            {
                editor.ConnectNodes(createdNode, FindNodeByIndex(datum.neighborIndexes[i]), datum.costBetweenNeighbors[i]);
            }

            curIndex++;
        }

        JeilNode FindNodeByIndex(int index)
        {
            foreach (Transform nodeObj in GameManager.obj.poolNode.transform)
            {
                JeilNode node = nodeObj.GetComponent<JeilNode>();
                if (node.index == index)
                {
                    return node;
                }
            }
            return null;
        }
    }
}

[Serializable]
public class NodeSaveDataArray
{
    public List<NodeSaveData> data = new List<NodeSaveData>();
}

[Serializable]
public class NodeSaveData
{
    [SerializeField] public Vector2 pos;
    [SerializeField] public List<int> neighborIndexes = new List<int>();
    [SerializeField] public List<int> costBetweenNeighbors =  new List<int>();
    [SerializeField] public bool isVisible;
    [SerializeField] public bool isStart;
    [SerializeField] public bool isDestination;

    static public NodeSaveData Create(JeilNode from)
    {
        NodeSaveData data = new NodeSaveData();
        data.pos = from.transform.position;
        foreach (JeilNode neighbor in from.neighbors)
        {
            data.neighborIndexes.Add(neighbor.index);
            data.costBetweenNeighbors.Add(from.neighborEdges[neighbor].cost);
        }
        
        data.isVisible = from.visibleInPathfinding;
        data.isStart = (GameManager.obj.managerPathfinding.startNode == from);
        data.isDestination = (GameManager.obj.managerPathfinding.destinationNode == from);
        
        return data;
    }
	
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}

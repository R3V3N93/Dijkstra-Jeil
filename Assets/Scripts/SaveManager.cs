using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

class SaveManager : MonoBehaviour
{
    void Save()
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
        
        string fileName = "config.json";
        string path = Application.dataPath + "/" + fileName;

        File.WriteAllText(path, JsonUtility.ToJson(dataArray));
        Debug.Log("Saved config to " + path);
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
    [SerializeField] public List<int> neighborIndexes;
    [SerializeField] public List<int> costBetweenNeighbors;
    [SerializeField] public bool isVisible;

    static public NodeSaveData Create(JeilNode from)
    {
        NodeSaveData data = new NodeSaveData();
        data.pos = from.transform.position;
        data.neighborIndexes = new List<int>();
        data.costBetweenNeighbors = new List<int>();
        data.isVisible = from.visibleInPathfinding;
        
        return data;
    }
	
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "PrefabSO", menuName = "SO/PrefabSO")]
public class PrefabSO : ScriptableObject
{
    [Header("Prefabs")]
    public GameObject node;
    public GameObject edge;
}

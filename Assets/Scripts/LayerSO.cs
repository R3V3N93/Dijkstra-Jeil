using UnityEngine;

[CreateAssetMenu(fileName = "Layer", menuName = "SO/Layer")]
public class LayerSO : ScriptableObject
{
    [Header("Layers")]
    public LayerMask node;
    public LayerMask nodeHeld;
    public LayerMask edge;
}

using UnityEngine;
using TMPro;

public class PathFindingInfo : MonoBehaviour
{
    public TMP_Text textAlgorithm;
    public TMP_Text textReal;

    public void UpdateFrom(TimerInfo from)
    {
        textAlgorithm.text = from.lastAlgorithm.ToString();
        textReal.text = from.estimatedRealTime.ToString();
    }
}

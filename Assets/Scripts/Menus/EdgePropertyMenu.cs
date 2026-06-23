using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EdgePropertyMenu : MonoBehaviour
{
    public TMP_InputField costInput;
    public Toggle visibilityToggle;

    public void Activate(List<JeilElement> selections)
    {
        gameObject.SetActive(true);
        if(selections.Count > 1)
        {
            costInput.text = "";
            visibilityToggle.isOn = false;
        }
        else
        {
            JeilEdge edge = selections[0] as JeilEdge;
            costInput.text = edge.cost.ToString();
            visibilityToggle.isOn = edge.visibleInPathfinding;
        }
    }

    
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}

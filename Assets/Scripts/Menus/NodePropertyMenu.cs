using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodePropertyMenu : MonoBehaviour
{
    public Button setStartNodeButton;
    public Button setDestinationNodeButton;
    public TMP_InputField layerInput;
    public Toggle landmarkToggle;

    public void Activate(List<JeilElement> selections)
    {
        gameObject.SetActive(true);
        if(selections.Count > 1)
        {
            setStartNodeButton.interactable = false;
            setDestinationNodeButton.interactable = false;

            layerInput.text = "";
            landmarkToggle.isOn = false;
        }
        else
        {
            setStartNodeButton.interactable = true;
            setDestinationNodeButton.interactable = true;
            
            JeilNode node = selections[0] as JeilNode;
            layerInput.text = node.layer.ToString();
            landmarkToggle.isOn = node.visibleInPathfinding;
        }
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}

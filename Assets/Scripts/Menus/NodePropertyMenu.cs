using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodePropertyMenu : MonoBehaviour
{
    public Button setStartNodeButton;
    public Button setDestinationNodeButton;
    public TMP_InputField layerInput;
    public Toggle landmarkToggle;

    public void Activate(bool multipleSelected = false)
    {
        gameObject.SetActive(true);
        if(multipleSelected)
        {
            // ... Disable button gameobjects
        }
        else
        {
            // ... Enable buttons
        }
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}

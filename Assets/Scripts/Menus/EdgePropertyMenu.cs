using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EdgePropertyMenu : MonoBehaviour
{
    public TMP_InputField costInput;
    public Toggle visibilityToggle;

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}

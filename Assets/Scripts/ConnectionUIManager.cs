using TMPro;
using UnityEngine;

public class ConnectionUIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipField;
    [SerializeField] private TMP_InputField nameField;
    
    public string GetIP()
    {
        return ipField.text;
    }

    public string GetName()
    {
        return nameField.text;
    }
}

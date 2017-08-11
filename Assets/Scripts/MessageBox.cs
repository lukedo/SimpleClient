using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private Text buttonText;
    [SerializeField] private Button button;

    public Button Button
    {
        get { return button; }
    }

    public void SetBoxText(string message, string button)
    {
        messageText.text = message;
        buttonText.text = button;
    }
}

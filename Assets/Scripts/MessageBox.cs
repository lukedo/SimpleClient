using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private Text buttonText;
    [SerializeField] private Button button;

    public static void CreateMessageBox(GameObject prefab, Transform canvas, string messageText, string buttonText, UnityAction buttonAction)
    {
        MessageBox messageBox = Instantiate(prefab, canvas).GetComponent<MessageBox>();
        messageBox.SetBoxText(messageText, buttonText);
        
        messageBox.Button.onClick.AddListener(buttonAction);
    }
    
    public void Awake()
    {
        button.onClick.AddListener(delegate { Destroy(gameObject); });
    }

    public Button Button
    {
        get { return button; }
    }

    public void SetBoxText(string message, string buttonMessage)
    {
        messageText.text = message;
        buttonText.text = buttonMessage;
    }
}
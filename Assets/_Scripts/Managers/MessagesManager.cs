using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessagesManager : MonoBehaviour
{
    public static MessagesManager instance;

    public struct Message
    {
        public string title;
        public string body;
    }

    List<Message> messages = new List<Message>();
    public static bool visible = false;
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _bodyText;
    [SerializeField] TextMeshProUGUI _buttonText;


    private void Awake()
    {
        instance = this;
        instance.gameObject.SetActive(false); // hide on startup
    }

    public static void Show(Message msg)
    {
        instance.gameObject.SetActive(true);
        instance.messages.Add(msg);
        instance.UpdateMessageText();
        instance.UpdateButtonText();
        visible = true;
    }

    public static void OnDone()
    {
        if (instance.messages.Count == 1)
        {
            // hide
            instance.gameObject.SetActive(false);
            visible = false;
            instance.messages.Clear();
        } else if (instance.messages.Count > 1)
        {
            instance.messages.Remove(instance.messages[0]);
            instance.UpdateMessageText();
            instance.UpdateButtonText();
        }
    }

    private void UpdateMessageText()
    {
        if (instance.messages.Count == 0) return;
        Message headOfQueue = instance.messages[0];
        _titleText.text = headOfQueue.title;
        _bodyText.text = headOfQueue.body;
    }

    private void UpdateButtonText()
    {
        if (messages.Count > 1)
        {
            _buttonText.text = "Next";
        } else
        {
            _buttonText.text = "Done";
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

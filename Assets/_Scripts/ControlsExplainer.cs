using UnityEngine;
using TMPro;

public class ControlsExplainer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textBox;


    static ControlsExplainer instance;

    public static void Show(string text)
    {
        instance.textBox.text = text;
        instance.gameObject.SetActive(true);

    }

    public static void Hide()
    {
        instance.gameObject.SetActive(false);
    }

    private void Awake()
    {
        instance = this;
    }
}

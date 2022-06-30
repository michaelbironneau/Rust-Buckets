using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    static TooltipManager instance;
    [SerializeField] TextMeshProUGUI tooltipText;
    private bool _active = true;
    private string _tooltip;

    Camera _cam;
    Vector3 _min, _max;
    RectTransform _rect;
    float offset = 10f;

    private void Awake()
    {
        instance = this;
    }

    public static void Show(string text)
    {
        instance.tooltipText.text = text;
        instance._active = true;
        instance.gameObject.SetActive(true);
    }

    public static bool Visible()
    {
        return instance._active;
    }

    public static void Hide()
    {
        instance._active = false;
        instance.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        _active = false;
        _cam = Camera.main;
        _rect = GetComponent<RectTransform>();
        _min = new Vector3(0, 0, 0);
        _max = new Vector3(_cam.pixelWidth, _cam.pixelHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            //get the tooltip position with offset
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y - (_rect.rect.height / 2 + offset), 0f); // + _rect.rect.width
            //clamp it to the screen size so it doesn't go outside
            transform.position = new Vector3(Mathf.Clamp(position.x, _min.x + _rect.rect.width / 2, _max.x - _rect.rect.width / 2), Mathf.Clamp(position.y, _min.y + _rect.rect.height / 2, _max.y - _rect.rect.height / 2), transform.position.z);
        }

    }
}

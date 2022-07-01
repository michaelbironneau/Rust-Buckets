using UnityEngine;

public class GroundSelectionController : MonoBehaviour
{
    static GroundSelectionController instance;
    Collider _collider;
    [SerializeField] float MaxMoveDistance = 1000f;

    public static Vector3 mousePosition = Vector3.zero;
    public static bool enableMouseTracking = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
            _collider = GetComponent<Collider>(); // must have a collider for any of this to work
            mousePosition = Vector3.zero;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            // left click
            SelectionManager.DeselectAll();
            ControlsExplainer.Hide();
        } else if (Input.GetMouseButton(1))
        {
            // right click - get intersection
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (_collider.Raycast(ray, out hit, MaxMoveDistance))
            {
                mousePosition = hit.point;
                SelectionManager.Move(hit.point);
                //Debug.Log(hit.point);
            }
        } else if (enableMouseTracking)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (_collider.Raycast(ray, out hit, MaxMoveDistance))
            {
                mousePosition = hit.point;
            }
        }
    }

}

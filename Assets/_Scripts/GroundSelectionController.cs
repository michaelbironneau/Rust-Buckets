using UnityEngine;

public class GroundSelectionController : MonoBehaviour
{
    Collider _collider;
    [SerializeField] float MaxMoveDistance = 1000f;

    private void Start()
    {
            _collider = GetComponent<Collider>(); // must have a collider for any of this to work
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            // left click
            SelectionManager.DeselectAll();
        } else if (Input.GetMouseButton(1))
        {
            // right click - get intersection
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (_collider.Raycast(ray, out hit, MaxMoveDistance))
            {
                SelectionManager.Move(hit.point);
                //Debug.Log(hit.point);
            }
        }
    }

}

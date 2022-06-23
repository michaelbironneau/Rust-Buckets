using UnityEngine;

public class SelectionController : MonoBehaviour, ISelectable
{
    [SerializeField] GameObject _selectionCirclePrefab;

    void Start()
    {
        _selectionCirclePrefab.SetActive(false);
        SelectionManager.Register(this);
    }

    private void OnMouseDown()
    {
        Debug.Log("Selected object");
        SelectionManager.OnSelected(this);
        Select();
    }
    void OnDestroy()
    {
        SelectionManager.Unregister(this);
    }

    public void Select()
    {
        Debug.Log("Showing circle");
        _selectionCirclePrefab.SetActive(true);
    }

    public void Deselect()
    {
        _selectionCirclePrefab.SetActive(false);
    }

    public void MoveTo(Vector3 worldPos)
    {
        //TODO Hook up with movement controller
    }
}

using UnityEngine;

public class SelectionController : MonoBehaviour, ISelectable
{
    [SerializeField] GameObject _selectionCirclePrefab;
    [SerializeField] float deadbandDistance = 10f;
    [SerializeField] float breakDistance = 100f;
    [SerializeField] float minBreakSpeed = 10f;
    [SerializeField] float maxReverseDistance = 50f;
    [SerializeField] float minTurnAngle = 1f;
    [SerializeField] private bool _selectionCircleFixedHeight = false;
    
    Vector3 _target;
    float _scHeight = 0;
    IVehicleController _vehicleController;


    void Awake()
    {
        _scHeight = _selectionCirclePrefab.transform.position.y;
        _vehicleController = GetComponent<IVehicleController>();
        if (_vehicleController == null)
        {
            _vehicleController = transform.GetComponentInChildren<IVehicleController>(true);
        }
        
    }

    void Start()
    {
        _selectionCirclePrefab.SetActive(false);
        SelectionManager.Register(this);
    }

    private void OnMouseDown()
    {
        //Debug.Log("Selected object");
        SelectionManager.OnSelected(this);
        Select();
    }
    void OnDestroy()
    {
        SelectionManager.Unregister(this);
    }

    public void MoveTo(Vector3 worldPos)
    {
        _target = worldPos;
    }

    public void Select()
    {
        //Debug.Log("Showing circle");
        _selectionCirclePrefab.SetActive(true);
    }

    public void Deselect()
    {
        _selectionCirclePrefab.SetActive(false);
    }

    void Update()
    {
        if (_target == null || _vehicleController == null) return;
        MoveToTarget();
        FixHeightOfSelectionCircle();
    }

    public void FixHeightOfSelectionCircle()
    {
        if (!_selectionCircleFixedHeight) return;
        Vector3 pos = _selectionCirclePrefab.transform.position;
        pos.y = _scHeight;
        _selectionCirclePrefab.transform.position = pos;
    }

    public void MoveToTarget()
    {


        float forwardAmount = 0;
        float turnAmount = 0;
        float distance = Vector3.Distance(transform.position, _target);

      

        // 1) If we're within deadband distance, do nothing
        if (distance < deadbandDistance)
        {
            _vehicleController.SetInputs(0f, 0f);
            return;
        }



        // 2) If we're not within breaking distance, move towards target
        Vector3 directionToMove = (_target - transform.position).normalized;
        float dot = Vector3.Dot(directionToMove, transform.forward);
        if (dot >= 0)
        {
            // in front
            forwardAmount = 1f;
        }
        else
        {
            //behind
            if (distance > maxReverseDistance)
            {
                forwardAmount = 1f;
            } else
            {
                forwardAmount = -1f;
            }
            
        }

        float turnAngle = Vector3.SignedAngle(transform.forward, directionToMove, transform.up);
        if (Mathf.Abs(turnAngle) >= minTurnAngle) // reduce jitter
        {
            if (turnAngle >= 0)
            {
                turnAmount = 1f;
            }
            else
            {
                turnAmount = -1f;
            }
        }
       

        // 3) If we're within breaking distance, break
        if (distance < breakDistance && _vehicleController.GetSpeed() > minBreakSpeed)
        {
            _vehicleController.SetInputs(-1, turnAmount); // break
            return;
        } else if (distance < breakDistance && _vehicleController.GetSpeed() < -1*minBreakSpeed)
        {
            _vehicleController.SetInputs(1, turnAmount); // break reverse
            return;
        }

            _vehicleController.SetInputs(forwardAmount, turnAmount);

    }
}

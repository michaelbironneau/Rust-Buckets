using UnityEngine;

public class SelectionController : MonoBehaviour, ISelectable
{
    [SerializeField] GameObject _selectionCirclePrefab;
    [SerializeField] float deadbandDistance = 10f;
    [SerializeField] float brakeDistance = 100f;
    [SerializeField] float minbrakeSpeed = 10f;
    [SerializeField] float maxReverseDistance = 50f;
    [SerializeField] private bool _selectionCircleFixedHeight = false;
    
    Vector3 _target;
    bool _haveTarget = false;
    bool _selected = false;
    float _scHeight = 0;
    IVehicleController _vehicleController;
    private VehiclePowerController _powerController;

    public enum MovementMode
    {
        Forward,
        Reverse,
        Both
    };

    public MovementMode mode = MovementMode.Both;

    void Awake()
    {
        _scHeight = _selectionCirclePrefab.transform.position.y;
        _vehicleController = GetComponent<IVehicleController>();
        _powerController = GetComponent<VehiclePowerController>();
        if (_powerController == null)
        {
            _powerController = transform.GetComponentInChildren<VehiclePowerController>(true);
        }
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
        _haveTarget = true;
    }

    public void Select()
    {
        //Debug.Log("Showing circle");
        _selected = true;
        _selectionCirclePrefab.SetActive(true);
        if (_powerController != null)
        {
            _powerController.ShowUI();
        }
    }

    public bool IsSelected()
    {
        return _selected;
    }

    public void Deselect()
    {
        _selected = false;
        _selectionCirclePrefab.SetActive(false);
        if (_powerController != null)
        {
            _powerController.HideUI();
        }
    }

    void Update()
    {
        if (!_haveTarget || _vehicleController == null) return;
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

    float getForwardMixedMode(Vector3 directionToMove, float distance)
    {
        float dot = Vector3.Dot(directionToMove, transform.forward);
        if (dot >= 0)
        {
            // in front
            return 1f;
        }
        else
        {
            //behind
            if (distance > maxReverseDistance)
            {
                return 1f;
            }
            else
            {
                return -1f;
            }
        }
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
            _haveTarget = false;
            return;
        }



        // 2) If we're not within brakeing distance, move towards target
        Vector3 directionToMove = (_target - transform.position).normalized;
        switch (mode)
        {
            case MovementMode.Forward:
                forwardAmount = 1f;
                break;
            case MovementMode.Reverse:
                forwardAmount = -1f;
                break;
            case MovementMode.Both:
            default:
                forwardAmount = getForwardMixedMode(directionToMove, distance);
                break;
        }
        

        float turnAngle = Vector3.SignedAngle(transform.forward, directionToMove, transform.up);
       
            if (turnAngle >= 0)
            {
                turnAmount = 1f;
            }
            else
            {
                turnAmount = -1f;
            }
      

        // 3) If we're within brakeing distance, brake
        if (distance < brakeDistance && _vehicleController.GetSpeed() > minbrakeSpeed)
        {
            _vehicleController.SetInputs(-1, turnAmount); // brake
            return;
        } else if (distance < brakeDistance && _vehicleController.GetSpeed() < -1*minbrakeSpeed)
        {
            _vehicleController.SetInputs(1, turnAmount); // brake reverse
            return;
        }

            _vehicleController.SetInputs(forwardAmount, turnAmount);

    }
}

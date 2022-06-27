using UnityEngine;

public class SelectionController : MonoBehaviour, ISelectable
{
    [SerializeField] GameObject _selectionCirclePrefab;
    [SerializeField] float deadbandDistance = 10f;
    [SerializeField] float breakDistance = 100f;
    [SerializeField] float minBreakSpeed = 10f;
    [SerializeField] float maxReverseDistance = 50f;
    [SerializeField] private bool _selectionCircleFixedHeight = false;
    
    Vector3 _target;
    bool _haveTarget = false;
    bool _selected = false;
    float _scHeight = 0;
    IVehicleController _vehicleController;
    private VehiclePowerController _powerController;

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
       
            if (turnAngle >= 0)
            {
                turnAmount = 1f;
            }
            else
            {
                turnAmount = -1f;
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

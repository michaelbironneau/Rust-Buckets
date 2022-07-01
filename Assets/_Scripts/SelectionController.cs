using UnityEngine;

public class SelectionController : MonoBehaviour, ISelectable
{
    [SerializeField] GameObject selectionCirclePrefab;
    [SerializeField] Component relatedActionHandler;
    [SerializeField] public float deadbandDistance = 10f;
    [SerializeField] float brakeDistance = 100f;
    [SerializeField] float minbrakeSpeed = 10f;
    [SerializeField] float maxReverseDistance = 50f;
    [SerializeField] private bool selectionCircleFixedHeight = false;
    [SerializeField] Transform front;
    [SerializeField] Transform rear;
    
    Vector3 _target;
    bool _haveTarget = false;
    bool _selected = false;
    float _scHeight = 0;
    IVehicleController _vehicleController;
    IRelatedActionHandler _relatedActionHandler;
    private VehiclePowerController _powerController;

    // Tutorial params
    [SerializeField] string tutorialText;
    [TextArea(1, 4)][SerializeField] string controls;
    bool _doneIntro;


    public enum MovementMode
    {
        Forward,
        Reverse,
        Both
    };

    public MovementMode mode = MovementMode.Both;

    public void RelatedAction(GameObject obj)
    {
        // user right clicked on other object while the current object was selected
        if (relatedActionHandler != null) _relatedActionHandler.OnRelatedAction(obj);
    }

    protected void OnValidate()
    {
        if (!(relatedActionHandler is IRelatedActionHandler))
            relatedActionHandler = null;
    }

    void Awake()
    {
        if (relatedActionHandler != null)
        {
            _relatedActionHandler = (IRelatedActionHandler)relatedActionHandler;
        }
        if (selectionCirclePrefab != null)
        {
            _scHeight = selectionCirclePrefab.transform.position.y;
        }
        
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
        if (selectionCirclePrefab != null)
        {
            selectionCirclePrefab.SetActive(false);
        }
        
        SelectionManager.Register(this);
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
        if (selectionCirclePrefab != null)
        {
            selectionCirclePrefab.SetActive(true);
        }
        
        if (_powerController != null)
        {
            _powerController.ShowUI();
        }

        if (tutorialText.Length > 0 && !_doneIntro)
        {
            MessagesManager.Message msg = new MessagesManager.Message();
            msg.title = "Tutorial";
            msg.body = tutorialText;
            MessagesManager.Show(msg);
            _doneIntro = true;
        }

        if (controls.Length > 0)
        {
            ControlsExplainer.Show(controls);
        } else
        {
            ControlsExplainer.Hide();
        }
    }

    public bool IsSelected()
    {
        return _selected;
    }

    public void Deselect()
    {
        _selected = false;
        if (selectionCirclePrefab != null)
        {
            selectionCirclePrefab.SetActive(false);
        }
     
        if (_powerController != null)
        {
            _powerController.HideUI();
        }
    }

    private void OnMouseOver()
    {
        
        if (Input.GetMouseButton(0))
        {
            // left click
            SelectionManager.OnSelected(this);
            Select();
        }
        else if (Input.GetMouseButton(1))
        {
            // right click - report related action
            SelectionManager.OnRelatedAction(this.gameObject);
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
        if (selectionCirclePrefab == null) return;
        if (!selectionCircleFixedHeight) return;
        Vector3 pos = selectionCirclePrefab.transform.position;
        pos.y = _scHeight;
        selectionCirclePrefab.transform.position = pos;
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

    float DistanceToTarget()
    {
        switch (mode)
        {
            case MovementMode.Forward:
                if (front != null)
                {
                    return Vector3.Distance(front.position, _target);
                } else
                {
                    return Vector3.Distance(transform.position, _target);
                }
            case MovementMode.Reverse:
                if (rear != null)
                {
                    return Vector3.Distance(rear.position, _target);
                }
                else
                {
                    return Vector3.Distance(transform.position, _target);
                }
            case MovementMode.Both:
            default:
                return Vector3.Distance(transform.position, _target);
        }
    }

    public bool CloseToTarget()
    {
        return !_haveTarget;
    }
 
    public void MoveToTarget()
    {


        float forwardAmount = 0;
        float turnAmount = 0;
        float distance = DistanceToTarget() ;

      

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

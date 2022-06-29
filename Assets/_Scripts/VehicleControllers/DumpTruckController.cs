using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumpTruckController : MonoBehaviour, IVehicleController, IRelatedActionHandler
{
    Rigidbody _rb;
    float _forward = 0f;
    float _turn = 0f;
    float _currentBreakForce = 0f;
    float _previousTurn = 0f;
    private float _dustEmission = 0f;
    private VehiclePowerController _vehiclePowerController;
    private SelectionController _selectionController;

    // Wheels
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider backLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider backRightWheelCollider;
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;

    // Driving params
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField, Range(1, 100)] private float smoothing;
    [SerializeField] private float maxAngularVelocityDegrees;
    [SerializeField, Range(0, 100)] private float tyreSlipDustThreshold = 10f;

    // Particle systems
    [SerializeField] ParticleSystem DustLeft;
    [SerializeField] ParticleSystem DustRight;
    [SerializeField] ParticleSystem DustCloud;

    // Mining params
    [SerializeField] DumpTruckBucketController bucketController;
    Collider _nearestRock;
    GameObject _nextRock;
    List<GameObject> _rocks = new List<GameObject>();
    bool _hadRock;

    // Dumping params
    [SerializeField] DumpTruckBinController binController;
    SmeltingPlantController _smelter;

    enum State
    {
        Mining,
        Dumping,
        Idle
    };

    State _state = State.Idle;

    public void OnRelatedAction(GameObject obj)
    {
        if (obj.tag == "Rock")
        {
            //Debug.Log("Mining mode");
            _nearestRock = obj.GetComponent<Collider>();
            _nextRock = obj;
            PutIntoMiningMode();
        } else if (obj.tag == "Smelter")
        {
            //Debug.Log("Dumping mode");
            _smelter = obj.GetComponent<SmeltingPlantController>();
            Vector3 nearest;
            Collider other = obj.GetComponent<Collider>();
            nearest = other.ClosestPoint(transform.position);
            _selectionController.MoveTo(nearest);
            PutIntoDumpingMode();
        }
    }

    

    void Start()
    {
        DustLeft.Stop();
        DustLeft.Stop();
        _dustEmission = DustCloud.emissionRate;
        _rb = GetComponent<Rigidbody>();  
        _vehiclePowerController = GetComponent<VehiclePowerController>();
        _selectionController = GetComponent<SelectionController>();
    }

    void GoToNearestRock()
    {
        if (_nearestRock == null)
        {
            _nextRock = bucketController.FindNearestRock();
            if (_nextRock == null) return;
            _nearestRock = _nextRock.GetComponent<Collider>();
            if (_nearestRock == null)
            {
                return; // TODO: Provide user feedback on the mining radius etc
            }
            //Debug.Log(_nearestRock.gameObject.name);
        }
        _selectionController.MoveTo(_nearestRock.ClosestPoint(transform.position));
    }

    void PutIntoMiningMode()
    {
        if (_state == State.Mining) return;
        _state = State.Mining;
        bucketController.miningMode = true;
        _selectionController.mode = SelectionController.MovementMode.Forward;
        binController.RotateToInitialPosition();
    }

    void PutIntoDumpingMode()
    {
        if (_state == State.Dumping) return;
        _state = State.Dumping;
        bucketController.miningMode = false;
        _selectionController.mode = SelectionController.MovementMode.Reverse;
        StartCoroutine(WaitAndSmelt());
    }

    IEnumerator WaitAndSmelt()
    {
        while (!_selectionController.CloseToTarget())
        {
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(1f);
        //Debug.Log("Near smelter");
        binController.RotateToDumpPosition();
        yield return Smelt();
    }

    IEnumerator Smelt()
    {
        while (binController.Rotating())
        {
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(1f);
        //Debug.Log("Done rotating");
        for (var i = 0; i < _rocks.Count; i++)
        {
            _smelter.Smelt(_rocks[i]);
            yield return new WaitForSeconds(2f);
        }
        _rocks.Clear();
        PutIntoIdleMode();
    }

    void PutIntoIdleMode()
    {
        if (_state == State.Idle) return;
        _state = State.Idle;
        bucketController.miningMode = false;
        _selectionController.mode = SelectionController.MovementMode.Both;
        binController.RotateToInitialPosition();
    }

    void Update()
    {
        if (!_selectionController.IsSelected()) return;
        if (Input.GetKey(KeyCode.Space))
        {
            PutIntoMiningMode();
        }
        else if (Input.GetKey(KeyCode.X))
        {
            PutIntoIdleMode();   
        } else if (Input.GetKey(KeyCode.B))
        {
            PutIntoDumpingMode();
        }
    }

    public void SetInputs(float forward, float turn)
    {
        _forward = forward;
        _turn = turn;
    }
    public float GetSpeed()
    {
        return _rb.velocity.magnitude;
    }

    void FixedUpdate()
    {
        //Debug.Log("[DT] Forward: " + _forward.ToString() + " Turn: " + _turn.ToString());
        if (_hadRock && !bucketController.HaveRock())
        {
            // reset nearest rock, which is now in the trailer, so that
            // we don't keep trying to move towards it (we've already captured this one).
            Debug.Log("Added rock " + _nextRock.name);
            _nearestRock = null;
            _hadRock = false;
            _rocks.Add(_nextRock);
            _nextRock = null;
        }
        
        if (_state == State.Mining && !bucketController.HaveRock())
        {
            GoToNearestRock();
        } 
        if (bucketController.HaveRock())
        {
            _hadRock = true;
            //return; // Don't move while we're trying to capture the rock, otherwise it may not end up in the trailer!
        }
        HandleMotor();
        HandleSteering();
        UpdateWheelVisuals();
        if (AreWheelsSpinning())
        {
            DustLeft.Play();
            DustRight.Play();
        } else
        {
            DustLeft.Stop();
            DustRight.Stop();
        }
        if (_rb.velocity.magnitude > 0.05f)
        {
            DustCloud.emissionRate = _dustEmission;
        } else
        {
            DustCloud.emissionRate = 0f;
        }
    }

  
    void HandleMotor()
    {
        bool exceedingSafeAngularVelocity = _rb.angularVelocity.magnitude >= Mathf.Deg2Rad * maxAngularVelocityDegrees;
        if (_rb.angularVelocity.magnitude >= Mathf.Deg2Rad * maxAngularVelocityDegrees) 
        {
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
            _vehiclePowerController.SetPowerPercent(0);
        } else
        {
            if (_forward == 1)
            {
                frontLeftWheelCollider.motorTorque = _forward * motorForce;
                frontRightWheelCollider.motorTorque = _forward * motorForce;
                _vehiclePowerController.SetPowerPercent(-100);
            } else
            {
                frontLeftWheelCollider.motorTorque = _forward * motorForce * 0.5f;
                frontRightWheelCollider.motorTorque = _forward * motorForce * 0.5f;
                _vehiclePowerController.SetPowerPercent(-50);
            }
            
        }

        if (!_vehiclePowerController.CanDischarge())
        {
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
        }

        float directionDot = Vector3.Dot(transform.forward, _rb.velocity);
        bool isBreaking = Mathf.Sign(directionDot) != _forward || _forward == 0 ? true : false; // if we're going in the opposite direction to _forward we should break
        if (isBreaking || exceedingSafeAngularVelocity)
        {
            _currentBreakForce = breakForce;
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
            _vehiclePowerController.SetPowerPercent(0);
        } else
        {
            _currentBreakForce = 0;
        }
        ApplyBreaking();

    }

    bool AreWheelsSpinning()
    {
        //gauge slip based on left front wheel (arbitrary - could be either, but not rear wheels as less likely to slip)
        WheelHit hitData;
        frontLeftWheelCollider.GetGroundHit(out hitData);
        if (Mathf.Abs(hitData.forwardSlip) >= tyreSlipDustThreshold * 0.01)
        {
            return true;
            
        } else
        {
            return false;
        }

    }

    void ApplyBreaking()
    {
        frontLeftWheelCollider.brakeTorque = _currentBreakForce;
        frontRightWheelCollider.brakeTorque = _currentBreakForce;
        backLeftWheelCollider.brakeTorque = _currentBreakForce;
        backRightWheelCollider.brakeTorque = _currentBreakForce;
    }

    void HandleSteering()
    {
        float steerAngle = maxSteerAngle * (_turn + (smoothing-1)*_previousTurn)/smoothing; // average current and previous turn to smooth out
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
        _previousTurn = (_turn + (smoothing - 1) * _previousTurn) / smoothing;


    }

    void UpdateWheelVisuals()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(backLeftWheelCollider, backLeftWheelTransform);
        UpdateSingleWheel(backRightWheelCollider, backRightWheelTransform);
    }

    void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

   
}

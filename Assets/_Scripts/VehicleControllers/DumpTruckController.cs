using UnityEngine;

public class DumpTruckController : MonoBehaviour, IVehicleController
{
    Rigidbody _rb;
    float _forward = 0f;
    float _turn = 0f;
    float _currentBreakForce = 0f;
    float _previousTurn = 0f;
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider backLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider backRightWheelCollider;
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField, Range(1, 100)] private float smoothing;
    [SerializeField] private float maxAngularVelocityDegrees;
    [SerializeField, Range(0, 100)] private float tyreSlipDustThreshold = 10f;
    [SerializeField] ParticleSystem DustLeft;
    [SerializeField] ParticleSystem DustRight;
    [SerializeField] ParticleSystem DustCloud;

    private VehiclePowerController _vehiclePowerController;

    void Start()
    {
        DustLeft.Stop();
        DustLeft.Stop();
        DustCloud.Stop();
        _rb = GetComponent<Rigidbody>();  
        _vehiclePowerController = GetComponent<VehiclePowerController>();
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
        if (_rb.velocity.magnitude > 0.01f)
        {
            DustCloud.Play();
        } else
        {
            DustCloud.Stop();
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
            frontLeftWheelCollider.motorTorque = _forward * motorForce;
            frontRightWheelCollider.motorTorque = _forward * motorForce;
            _vehiclePowerController.SetPowerPercent(-100);
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

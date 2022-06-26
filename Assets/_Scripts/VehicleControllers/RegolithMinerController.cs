using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegolithMinerController : MonoBehaviour, IVehicleController
{
    Rigidbody _rb;
    float _forward = 0f;

    float _turn = 0f;
    float _currentBreakForce = 0f;
    float _previousTurn = 0f;
    
    //Cabin wheels
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider backLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider backRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;


    //Trailer Wheels
    [SerializeField] private WheelCollider trailerFrontLeftWheelCollider;
    [SerializeField] private WheelCollider trailerBackLeftWheelCollider;
    [SerializeField] private WheelCollider trailerFrontRightWheelCollider;
    [SerializeField] private WheelCollider trailerBackRightWheelCollider;

    [SerializeField] private Transform trailerFrontLeftWheelTransform;
    [SerializeField] private Transform trailerFrontRightWheelTransform;
    [SerializeField] private Transform trailerBackLeftWheelTransform;
    [SerializeField] private Transform trailerBackRightWheelTransform;

    // Driving params
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField, Range(1, 100)] private float smoothing;
    [SerializeField] private float maxAngularVelocityDegrees;
    [SerializeField, Range(0, 100)] private float tyreSlipDustThreshold = 10f;
    [SerializeField] ParticleSystem DustLeft;
    [SerializeField] ParticleSystem DustRight;
    [SerializeField] ParticleSystem DustCloud;
    [SerializeField] float dustCloudSpeedThreshold = 0.4f;


    // Excavation params
    [SerializeField] private float bucketWheelAngularVelocityDegrees = 10;
    [SerializeField] Transform bucketWheel;
    [SerializeField] Transform bucketArm;
    [SerializeField] float bucketArmMinHeight;
    [SerializeField] float bucketArmMaxHeight;
    [SerializeField] float bucketArmRaiseSeconds = 3f;
    [SerializeField] float regolithMiningRate = 1f;
    [SerializeField] float regolithPowerPercent = 1f;
    [SerializeField] ParticleSystem ArmDustCloud;
    [SerializeField] ParticleSystem CollectorDustCloud;
    private bool _mining = false;
    private bool _bucketWheelGrounded = false;
    private bool _excavating = false;
    private float _dustEmission = 0f;
    private float _armDustEmission = 0f;
    private float _collectorDustEmission;
    private VehiclePowerController _vehiclePowerController;

    void Start()
    {
        DustLeft.Stop();
        DustLeft.Stop();
        _dustEmission = DustCloud.emissionRate;
        _armDustEmission = ArmDustCloud.emissionRate;
        _collectorDustEmission = CollectorDustCloud.emissionRate;
        _rb = GetComponent<Rigidbody>();
        _vehiclePowerController = GetComponent<VehiclePowerController>();
    }

    public void OnBucketWheelGrounded()
    {
        _bucketWheelGrounded = true;
    }

    public void OnBucketWheelAirborne()
    {
        _bucketWheelGrounded = false;
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

    void UpdateBucketWheel()
    {
        Vector3 armPos = bucketArm.transform.localPosition;
        if (!_mining)
        {
            _excavating = false;
            ArmDustCloud.emissionRate = 0f;
            CollectorDustCloud.emissionRate = 0f;
            // Driving mode
            // raise arm and return
            if (armPos.y < bucketArmMaxHeight)
            {
                armPos.y += (1 / bucketArmRaiseSeconds) * Time.deltaTime;
                armPos.y = Mathf.Clamp(armPos.y, bucketArmMinHeight, bucketArmMaxHeight);
                bucketArm.transform.localPosition = armPos;
            }
            return;
                
        }

        // Mining mode
        //1) Are we making contact with the ground?
        if (!_bucketWheelGrounded)
        {
            _excavating = false;
            ArmDustCloud.emissionRate = 0f;
            CollectorDustCloud.emissionRate = 0f;
            if (armPos.y > bucketArmMinHeight)
                // a) If not then lower the arm
            {
                armPos.y -= (1 / bucketArmRaiseSeconds) * Time.deltaTime;
                armPos.y = Mathf.Clamp(armPos.y, bucketArmMinHeight, bucketArmMaxHeight);
                bucketArm.transform.localPosition = armPos;
            }
            return;
        } else
        {
            //b) If yes then mine
            if (!_vehiclePowerController.CanDischarge()) return; // no power left!
            _excavating = true;
            StatsManager.Stats mined = new StatsManager.Stats();
            mined.silicates = Time.deltaTime * regolithMiningRate;
            StatsManager.ApplyUpdate(mined);
            ArmDustCloud.emissionRate = _armDustEmission;
            CollectorDustCloud.emissionRate = _collectorDustEmission;
            _vehiclePowerController.SetPowerPercent(-1 * regolithPowerPercent);
            bucketWheel.transform.RotateAround(bucketWheel.transform.right, (1 / bucketWheelAngularVelocityDegrees) * Time.deltaTime);
        }

        
       
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _mining = true;
        } else
        {
            _mining = false;
        }
        
        UpdateBucketWheel();
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
        }
        else
        {
            DustLeft.Stop();
            DustRight.Stop();
        }
        if (_rb.velocity.magnitude > 0.01f)
        {
            DustCloud.emissionRate = _dustEmission;
        } else
        {
            DustCloud.emissionRate = 0;
        
        }
    }

    void HandleMotor()
    {
        bool exceedingSafeAngularVelocity = _rb.angularVelocity.magnitude >= Mathf.Deg2Rad * maxAngularVelocityDegrees;
        if (_rb.angularVelocity.magnitude >= Mathf.Deg2Rad * maxAngularVelocityDegrees)
        {
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
            backLeftWheelCollider.motorTorque = 0;
            backRightWheelCollider.motorTorque = 0;
            
            _vehiclePowerController.SetPowerPercent(0);
        }
        else
        {
            frontLeftWheelCollider.motorTorque = _forward * motorForce;
            frontRightWheelCollider.motorTorque = _forward * motorForce;
            backLeftWheelCollider.motorTorque = _forward * motorForce * 0.25f;
            backRightWheelCollider.motorTorque = _forward * motorForce * 0.25f;
            if (_forward != 0) _vehiclePowerController.SetPowerPercent(-100);

        }

        float directionDot = Vector3.Dot(transform.forward, _rb.velocity);
        bool isBreaking = Mathf.Sign(directionDot) != _forward || _forward == 0 ? true : false; // if we're going in the opposite direction to _forward we should break
        if (_rb.velocity.magnitude > 0.001f && (isBreaking || exceedingSafeAngularVelocity))
        {
            //Debug.Log("Braking. Dot: " + directionDot.ToString() + " Fwd: " + _forward + " Spd: " + _rb.velocity + " Fwd: " + transform.forward);
            _currentBreakForce = breakForce;
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
            backLeftWheelCollider.motorTorque = 0;
            backRightWheelCollider.motorTorque = 0;
            _vehiclePowerController.SetPowerPercent(0);
        }
        else
        {
            _currentBreakForce = 0;
        }
        if (!_vehiclePowerController.CanDischarge())
        {
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
            backLeftWheelCollider.motorTorque = 0;
            backRightWheelCollider.motorTorque = 0;
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

        }
        else
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
        trailerFrontLeftWheelCollider.brakeTorque = _currentBreakForce;
        trailerFrontRightWheelCollider.brakeTorque = _currentBreakForce;
        trailerBackLeftWheelCollider.brakeTorque = _currentBreakForce;
        trailerBackRightWheelCollider.brakeTorque = _currentBreakForce;
    }

    void HandleSteering()
    {
        float steerAngle = maxSteerAngle * (_turn + (smoothing - 1) * _previousTurn) / smoothing; // average current and previous turn to smooth out
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
        backLeftWheelCollider.steerAngle = steerAngle*0.25f; // 4 wheel turning
        backRightWheelCollider.steerAngle = steerAngle*0.25f; // 4 wheel turning
        _previousTurn = (_turn + (smoothing - 1) * _previousTurn) / smoothing;


    }

    void UpdateWheelVisuals()
    {
        //Digger
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(backLeftWheelCollider, backLeftWheelTransform);
        UpdateSingleWheel(backRightWheelCollider, backRightWheelTransform);
        //Trailer
        UpdateSingleWheel(trailerFrontLeftWheelCollider, trailerFrontLeftWheelTransform);
        UpdateSingleWheel(trailerFrontRightWheelCollider, trailerFrontRightWheelTransform);
        UpdateSingleWheel(trailerBackLeftWheelCollider, trailerBackLeftWheelTransform);
        UpdateSingleWheel(trailerBackRightWheelCollider, trailerBackRightWheelTransform);
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
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

    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField, Range(1, 100)] private float smoothing;
    [SerializeField] private float maxAngularVelocityDegrees;
    [SerializeField, Range(0, 100)] private float tyreSlipDustThreshold = 10f;
    [SerializeField] ParticleSystem DustLeft;
    [SerializeField] ParticleSystem DustRight;

    void Start()
    {
        DustLeft.Stop();
        DustLeft.Stop();
        _rb = GetComponent<Rigidbody>();
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
        }
        else
        {
            DustLeft.Stop();
            DustRight.Stop();
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
        }
        else
        {
            frontLeftWheelCollider.motorTorque = _forward * motorForce;
            frontRightWheelCollider.motorTorque = _forward * motorForce;
            backLeftWheelCollider.motorTorque = _forward * motorForce * 0.25f;
            backRightWheelCollider.motorTorque = _forward * motorForce * 0.25f;
        }

        float directionDot = Vector3.Dot(transform.forward, _rb.velocity);
        bool isBreaking = Mathf.Sign(directionDot) != _forward || _forward == 0 ? true : false; // if we're going in the opposite direction to _forward we should break
        if (_rb.velocity.magnitude > 0.001f && (isBreaking || exceedingSafeAngularVelocity))
        {
            //Debug.Log("Braking. Dot: " + directionDot.ToString() + " Fwd: " + _forward + " Spd: " + _rb.velocity + " Fwd: " + transform.forward);
            _currentBreakForce = breakForce;
        }
        else
        {
            Debug.Log("Moving");
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
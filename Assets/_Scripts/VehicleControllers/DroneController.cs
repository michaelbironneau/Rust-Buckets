using System.Collections;
using UnityEngine;

public class DroneController : MonoBehaviour, IVehicleController
{
    [SerializeField] private float CruisingHeight = 5f;
    [SerializeField, Range(1f, 2f)] private float AutomaticThrustingForce = 1.2f;
    [SerializeField, Range(0f, 1f)] private float SoftLandingForce = 0.9f;
    [SerializeField, Range(0f, 1f)] private float AltitudeDeadbandProportion = 0.05f;
    [SerializeField, Range(0f, 1f)] private float ClimbBrakingForce = 0.5f;
    [SerializeField, Range(0f, 0.1f)] private float StabilizationJitter = 0.07f;
    [SerializeField, Range(0f, 1f)] private float CruisingForwardForce = 0.5f;
    [SerializeField] private float MaxSpeed = 5f;
    [SerializeField] private float MaxTurnSpeedDegreesPerSecond = 5f;

    private float _forward = 0;
    private float _turn = 0;
    private bool _flying = false;
    private bool _cruising = false;
    private Rigidbody _rb;
    private SelectionController _mySelectionController;

    // Start is called before the first frame update
    void Start()
    {
        _mySelectionController = GetComponent<SelectionController>();
        _rb = GetComponent<Rigidbody>();
    }

    public float GetSpeed()
    {
        return _rb.velocity.magnitude;
    }

    public void SetInputs(float forward, float turn)
    {
        _forward = forward;
        _turn = turn;
    }

    void FixedUpdate()
    {
        if (!_flying)
        {
            Land();

        } else
        {
            AchieveControlHeight();
        }
        ApplyInputs();

    }

 
    void ApplyInputs()
    {
        //Debug.Log("Forward: " + _forward.ToString() + " Turn: " + _turn.ToString());
        if (_forward == 0)
        {
            _flying = false;
        } else
        {
            _flying = true;
        }
        if (transform.position.y < 0.5 * CruisingHeight)
        {
            return; // wait until we reach half our cruising height before we start to navigate towards target
        }


        if (_forward != 0 && GetSpeed() < MaxSpeed)
        {
            _rb.AddForce(_forward * transform.forward * MaxSpeed / 5);
        } 
        var m_EulerAngleVelocity = _rb.transform.up * _turn * MaxTurnSpeedDegreesPerSecond;
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
        _rb.MoveRotation(_rb.rotation * deltaRotation);

    }

    void BrakeNonVertical()
    {
        this._rb.AddForce(-0.75f * this._rb.velocity.x, 0, -0.75f * this._rb.velocity.z);
    }
   
    

    void Land()
    {
        BrakeNonVertical();
        if (transform.position.y < 0.5f)
        {
            return; //inert
        }
        if (transform.position.y > 1.5f)
        {
            // soft descent
            this._rb.AddForce(Vector3.up * Physics.gravity.magnitude * SoftLandingForce);
        } else if (transform.position.y > 0.75f)
        {
            //add a bit of a shake to the landing
            this._rb.AddForce(Vector3.up * Physics.gravity.magnitude * 0.9f);
        } else
        {
            this._rb.AddForce(Vector3.up * Physics.gravity.magnitude * 1.5f);
        }
        
    }

    float GetRelativeOffsetFromSetpoint()
    {
        if (Mathf.Abs(transform.position.y - CruisingHeight) < AltitudeDeadbandProportion * CruisingHeight)
        {
            return 0f;
        }
        return (transform.position.y - CruisingHeight)/CruisingHeight;
    }

    void AchieveControlHeight()
    {
        float relativeOffset = GetRelativeOffsetFromSetpoint();
        float mag;
        if (relativeOffset == 0)
        {
            mag = Random.Range(1 - StabilizationJitter, 1 + StabilizationJitter) + _rb.velocity.y*-1*ClimbBrakingForce;
            _cruising = true;
        } else if (relativeOffset < 0)
        {
            // relativeOffset is beetween 0 and -1
            _cruising = false;
            mag = (1 - relativeOffset * (AutomaticThrustingForce - 1));
            //linearly interpolate this to AutomaticThrustingForce when relativeOffset is smallest, and gravity when it reaches control height
        } else
        {
            // relativeOffset is between 0 and inf, linearly interpolate so that 1 maps to SoftLandingForce
            if (relativeOffset > 1)
            {
                _cruising = true;
                mag =  SoftLandingForce;
            } else
            {
                mag = SoftLandingForce + (1 - relativeOffset) * (1 - SoftLandingForce);
                _cruising = true;
            }
           

        }
        this._rb.AddForce(Vector3.up * mag * Physics.gravity.magnitude);

    }
}

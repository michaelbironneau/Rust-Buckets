using System.Collections;
using UnityEngine;

public class DroneController : MonoBehaviour, ISelectable
{
    [SerializeField] private float CruisingHeight = 5f;
    [SerializeField, Range(1f, 2f)] private float AutomaticThrustingForce = 1.2f;
    [SerializeField, Range(0f, 1f)] private float SoftLandingForce = 0.9f;
    [SerializeField, Range(0f, 1f)] private float AltitudeDeadbandProportion = 0.05f;
    [SerializeField, Range(0f, 1f)] private float ClimbBrakingForce = 0.5f;
    [SerializeField, Range(0f, 0.1f)] private float StabilizationJitter = 0.07f;
    [SerializeField, Range(0f, 1f)] private float CruisingForwardForce = 0.5f;

    private Vector3 direction;
    private bool _flying = false;
    private bool _cruising = false;
    private bool _selected = false;
    private Rigidbody _rb;


    public void Select()
    {
        _selected = true;
        SelectionManager.OnSelected(this);
    }

    public void Deselect()
    {
        _selected = false;
    }

    void OnDestroy()
    {
        SelectionManager.Unregister(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        SelectionManager.Register(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_selected) return;
        if (!_flying && Input.GetKeyDown(KeyCode.Space))
        {
            _flying = true;
            Debug.Log("Taking off");
        }
        Camera cam = Camera.main;
        if (_flying)
        {
            direction = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                direction += cam.transform.forward;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                direction += -1 * cam.transform.forward;

            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                direction += -1 * cam.transform.right;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                direction += cam.transform.right;
            }
            // normalize just in case we had multiple key presses
            direction = Vector3.Normalize(direction);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            _flying = false;
            _cruising = false;
        }
    }

    void FixedUpdate()
    {
        if (!_flying || !_selected)
        {
            Land();

        } else
        {
            AchieveControlHeight();
            Turn();
            
        }

    }

    void OnMouseDown()
    {
        _selected = true;
        //StartCoroutine(DeselectMe()); //Testing only!
    }

    IEnumerator DeselectMe()
    {
       yield return new WaitForSeconds(10f);
        _selected = false;

    }

    void Turn()
    {
        if (!_cruising) return;
        this._rb.AddForce(direction * CruisingForwardForce);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(transform.forward, direction), 3f);

    }

    void Land()
    {
        if (transform.position.y < 0.5f && _rb.velocity.y == 0)
        {
            return; //inert
        }
        if (transform.position.y > 1.5f)
        {
            // soft descent
            this._rb.AddForce(Vector3.up * Physics.gravity.magnitude * SoftLandingForce);
        } else
        {
            //add a bit of a shake to the landing
            this._rb.AddForce(Vector3.up * Physics.gravity.magnitude * (1 - transform.position.y));
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

using UnityEngine;

public class DumpTruckBinController : MonoBehaviour
{
    private float _rotationDeadbandDegrees = 2.5f;
    private Quaternion _initialRotation;
    private Vector3 _shaftInitialPosition;

    [SerializeField] Transform dumpRockPosition;  // empty whose transform matches the rotation we want to achieve to dump rock
    [SerializeField] Transform shaftDumpPosition;
    [SerializeField] float rotationAngularVelocity = 20f;

    [SerializeField] GameObject shoulder; // The parent we should rotate
    [SerializeField] GameObject shaft;

    private bool _dumping = false;
    private bool _rotating = false;

    public void RotateToDumpPosition()
    {
        _dumping = true;
        shaft.transform.localPosition = shaftDumpPosition.transform.localPosition;
    }

    public void RotateToInitialPosition()
    {
        _dumping = false;
    }

    private void Start()
    {
        _initialRotation = shoulder.transform.localRotation;
        _shaftInitialPosition = shaft.transform.localPosition;
    }

    public bool Rotating()
    {
        return _rotating;
    }

    void Update()
    {
        Quaternion rot = shoulder.transform.localRotation;
        float step = rotationAngularVelocity * Time.deltaTime;
        if (_dumping && Mathf.Abs(Quaternion.Angle(rot, dumpRockPosition.localRotation)) > _rotationDeadbandDegrees)
        {
            shoulder.transform.localRotation = Quaternion.RotateTowards(rot, dumpRockPosition.localRotation, step);
            _rotating = true;
        }
        else if (!_dumping && Mathf.Abs(Quaternion.Angle(rot, _initialRotation)) > _rotationDeadbandDegrees)
        {
            shoulder.transform.localRotation = Quaternion.RotateTowards(rot, _initialRotation, step);
            _rotating = true;

        } else if (!_dumping)
        {
            shaft.transform.localPosition = _shaftInitialPosition;
            _rotating = false;
        } else if (_dumping)
        {
            // dumping but no need to rotate
            _rotating = false;
        }
    }
}

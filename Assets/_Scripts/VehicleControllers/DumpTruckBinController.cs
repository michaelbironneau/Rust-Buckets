using UnityEngine;

public class DumpTruckBinController : MonoBehaviour
{
    private float _rotationDeadbandDegrees = 2.5f;
    private Quaternion _initialRotation;

    [SerializeField] Transform dumpRockPosition;  // empty whose transform matches the rotation we want to achieve to dump rock
    [SerializeField] float rotationAngularVelocity = 20f;

    [SerializeField] GameObject shoulder; // The parent we should rotate

    private bool _dumping = false;

    public void RotateToDumpPosition()
    {
        _dumping = true;
    }

    public void RotateToInitialPosition()
    {
        _dumping = false;
    }
    
    void Update()
    {
        Quaternion rot = shoulder.transform.localRotation;
        float step = rotationAngularVelocity * Time.deltaTime;
        if (_dumping && Mathf.Abs(Quaternion.Angle(rot, dumpRockPosition.localRotation)) > _rotationDeadbandDegrees)
        {
            shoulder.transform.localRotation = Quaternion.RotateTowards(rot, dumpRockPosition.localRotation, step);
        }
        else if (!_dumping && Mathf.Abs(Quaternion.Angle(rot, _initialRotation)) > _rotationDeadbandDegrees)
        {
            shoulder.transform.localRotation = Quaternion.RotateTowards(rot, _initialRotation, step);
        }
    }
}

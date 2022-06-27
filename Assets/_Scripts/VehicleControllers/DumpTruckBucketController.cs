using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumpTruckBucketController : MonoBehaviour
{
    private bool _mining = false;
    private GameObject _rock;
    private float _rotationDeadbandDegrees = 2.5f;
    private Quaternion _initialShoulderRotation;

    [SerializeField] Transform dumpRockPosition;  // empty whose transform matches the rotation we want to achieve to dump rock
    [SerializeField] float rotationAngularVelocity = 20f;

    [SerializeField] GameObject shoulder; // The point at which the arm attaches to the cabin, i.e. the rotation point for the entire arm

    private void Start()
    {
        _initialShoulderRotation = shoulder.transform.localRotation;
    }

    public bool miningMode
    {
        get { return _mining; }
        set { _mining = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_mining) return;
        if (_rock != null) return; // can only carry one rock at a time
        if (other.gameObject.tag == "Rock")
        {
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.position = transform.position; // TODO: Offset?
            other.transform.parent = transform;
            _rock = other.gameObject;
            StartCoroutine(CaptureRock());
        }
    }


    IEnumerator CaptureRock()
    {
        yield return DumpRock();
        yield return WaitForArmReturnToInitialPos();
    }
    IEnumerator DumpRock()
    {
        yield return WaitForArmRotationToDumpPos();
        Debug.Log("DONE WAITING");
        //_rock.GetComponent<Rigidbody>().isKinematic = false; 
        _rock = null;
    }


    private void Update()
    {
        Quaternion rot = shoulder.transform.localRotation;
        float step = rotationAngularVelocity * Time.deltaTime;
        if (_rock != null && Mathf.Abs(Quaternion.Angle(rot, dumpRockPosition.localRotation)) > _rotationDeadbandDegrees)
        {
            shoulder.transform.localRotation = Quaternion.RotateTowards(rot, dumpRockPosition.localRotation, step);
        } else if (_rock == null && Mathf.Abs(Quaternion.Angle(rot, _initialShoulderRotation)) > _rotationDeadbandDegrees)
        {
            shoulder.transform.localRotation = Quaternion.RotateTowards(rot, _initialShoulderRotation, step);
        } 
    }

    IEnumerator WaitForArmReturnToInitialPos()
    {
        while (Mathf.Abs(Quaternion.Angle(shoulder.transform.localRotation, _initialShoulderRotation)) > _rotationDeadbandDegrees)
        {
            yield return new WaitForSeconds(.1f);
        }
        
    }

    IEnumerator WaitForArmRotationToDumpPos()
    {
        while (Mathf.Abs(Quaternion.Angle(shoulder.transform.rotation, dumpRockPosition.rotation)) > _rotationDeadbandDegrees)
        {
            yield return new WaitForSeconds(.1f);
        }

    }


}

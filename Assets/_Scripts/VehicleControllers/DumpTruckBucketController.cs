using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumpTruckBucketController : MonoBehaviour
{
    private bool _mining = false;
    private GameObject _rock;

    private float _initialShoulderRotation;

    [SerializeField] float dumpShoulderZRotation;
    [SerializeField] float rotationAngularVelocity = 20f;

    [SerializeField] GameObject shoulder; // The point at which the arm attaches to the cabin, i.e. the rotation point for the entire arm

    private void Start()
    {
        _initialShoulderRotation = shoulder.transform.localRotation.eulerAngles.z;
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
        _rock.GetComponent<Rigidbody>().isKinematic = false; // TODO: Will this work as the rock will be inside the bucket collider?
        _rock = null;
    }

    private void Update()
    {
        Vector3 rot = shoulder.transform.localRotation.eulerAngles;
        if (_rock != null && rot.z > dumpShoulderZRotation)
        {
            shoulder.transform.Rotate(shoulder.transform.right, -Time.deltaTime * rotationAngularVelocity);
            //rot.z -= Time.deltaTime*rotationAngularVelocity;
            //shoulder.transform.rotation = rot;
        } else if (_rock == null && rot.z < _initialShoulderRotation)
        {
            shoulder.transform.Rotate(shoulder.transform.right, Time.deltaTime * rotationAngularVelocity);
        }
    }

    IEnumerator WaitForArmReturnToInitialPos()
    {
        while (shoulder.transform.rotation.z < _initialShoulderRotation)
        {
            yield return new WaitForSeconds(.1f);
        }
        
    }

    IEnumerator WaitForArmRotationToDumpPos()
    {
        while (shoulder.transform.rotation.z >= dumpShoulderZRotation)
        {
            yield return new WaitForSeconds(.1f);
        }

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumpTruckBucketController : MonoBehaviour
{
    private bool _mining = false;
    private GameObject _rock;

    [SerializeField] GameObject shoulder; // the point at which the arm attaches to the cabin, i.e. the rotation point for the entire arm

    public bool miningMode
    {
        get { return _mining; }
        set { _mining = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
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
        yield return LiftRock();
        yield return DumpRock();
        yield return ReturnArmToInitialPosition();
    }

    IEnumerator LiftRock()
    {
        yield return ReturnArmToInitialPosition(); // in case the arm isn't already there
        yield return new WaitForSeconds(1f);
    }

    IEnumerator DumpRock()
    {
        yield return new WaitForSeconds(1f);
        _rock = null;
    }

    IEnumerator ReturnArmToInitialPosition()
    {
        yield return new WaitForSeconds(1f);
    }


}

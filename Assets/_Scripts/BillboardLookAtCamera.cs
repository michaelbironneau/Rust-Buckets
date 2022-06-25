using UnityEngine;

public class BillboardLookAtCamera : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
       
    }
}

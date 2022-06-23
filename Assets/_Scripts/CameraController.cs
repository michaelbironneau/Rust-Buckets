using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera _mycam;
    [SerializeField] float _sensitivity = 0.05f;
    [SerializeField] float _screenEdgeRatio = 0.05f;
    void Start()
    {
        _mycam = GetComponent<Camera>();
    }
    
    void UpdatePan()
    {
        Vector3 newPos = transform.position;
        if (Input.mousePosition.x < _screenEdgeRatio*Screen.width)
        {
            newPos = newPos - _sensitivity*transform.right;
        }
        if (Input.mousePosition.x > (1-_screenEdgeRatio)* Screen.width)
        {
            newPos = newPos + _sensitivity * transform.right;
        }
        if (Input.mousePosition.y < _screenEdgeRatio * Screen.height)
        {
            newPos = newPos - _sensitivity * transform.forward;
        }
        if (Input.mousePosition.y > (1 - _screenEdgeRatio) * Screen.height)
        {
            newPos = newPos + _sensitivity * transform.forward;
        }
        transform.position = newPos;
    }
    void UpdateRotation()
    {

        Vector3 vp = _mycam.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mycam.nearClipPlane));
        vp.x -= 0.5f;
        vp.y -= 0.5f;
        vp.x *= _sensitivity;
        vp.y *= _sensitivity;
        vp.x += 0.5f;
        vp.y += 0.5f;
        Vector3 sp = _mycam.ViewportToScreenPoint(vp);

        Vector3 v = _mycam.ScreenToWorldPoint(sp);
        transform.LookAt(v, Vector3.up);
    }

    void UpdateZoom()
    {
        transform.Translate(_sensitivity * Vector3.forward * Input.GetAxis("Mouse ScrollWheel"));
    }
    void Update()
    {
        if (MessagesManager.visible)
        {
            return; // don't allow camera moves while messages are displayed
        }
        UpdatePan();
        //UpdateRotation();
        //UpdateZoom();
    }
}

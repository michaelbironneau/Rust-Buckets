using UnityEngine;
using UnityEngine.EventSystems;

public class CameraPanDisabler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        CameraController.Disable();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CameraController.Enable();
    }
}

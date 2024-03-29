using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Show(text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }
}

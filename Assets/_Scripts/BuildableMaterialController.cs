using UnityEngine;

public class BuildableMaterialController : MonoBehaviour
{
    [SerializeField] Material placeholder;
    [SerializeField] Material final;

    public void ShowPlaceholder()
    {
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            if (childRenderer.material == final)
            {
                childRenderer.material = placeholder;
            }
        }
    }

    public void ShowFinal()
    {
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            if (childRenderer.material == placeholder)
            {
                childRenderer.material = final;
            }
        }
    }

}

using UnityEngine;
using System.Collections.Generic;

public class BuildableMaterialController : MonoBehaviour
{
    [SerializeField] Material placeholder;
    [SerializeField] Material illegal;

    Dictionary<MeshRenderer, Material> _originalMaterials;

    private void Start()
    {
        _originalMaterials = new Dictionary<MeshRenderer, Material>();
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            _originalMaterials.Add(childRenderer, childRenderer.material);  
        }
    }

    public void ShowPlaceholder()
    {
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            childRenderer.material = placeholder;
        }
    }

    public void ShowFinal()
    {
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            Material originalMaterial;
            if (_originalMaterials.TryGetValue(childRenderer, out originalMaterial))
            {
                childRenderer.material = originalMaterial;
            } else
            {
                Debug.LogWarning("Could not find original material for " + childRenderer.gameObject.name);
            }
        }
    }

    public void ShowIllegal()
    {
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            if (childRenderer.material == placeholder)
            {
                childRenderer.material = illegal;
            }
        }
    }

}

using UnityEngine;
using System.Collections.Generic;

public class BuildableMaterialController : MonoBehaviour
{
    [SerializeField] Material placeholder;
    [SerializeField] Material illegal;

    private enum State
    {
        Placeholder,
        Illegal,
        Original
    };

    private State state = State.Original;

    Dictionary<MeshRenderer, Material> _originalMaterials;
    void CacheMaterials()
    {
        _originalMaterials = new Dictionary<MeshRenderer, Material>();
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            _originalMaterials.Add(childRenderer, childRenderer.material); 
            Debug.Log(childRenderer.material.name);
        }
    }
    public void ShowPlaceholder()
    {
        if (state == State.Placeholder) return;
        if (_originalMaterials == null) CacheMaterials();
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            childRenderer.material = placeholder;
        }
        state = State.Placeholder;
    }

    public void ShowFinal()
    {
        if (state == State.Original) return;
        if (_originalMaterials == null) CacheMaterials();
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            Material originalMaterial;
            if (_originalMaterials.TryGetValue(childRenderer, out originalMaterial))
            {
                childRenderer.material = originalMaterial;
                //Debug.Log(originalMaterial.name);
            } else
            {
                Debug.LogWarning("Could not find original material for " + childRenderer.gameObject.name);
            }
        }
        state = State.Original;
    }

    public void ShowIllegal()
    {
        if (state == State.Illegal) return;
        if (_originalMaterials == null) CacheMaterials();
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
                childRenderer.material = illegal;
        }
        state = State.Illegal;
    }

}

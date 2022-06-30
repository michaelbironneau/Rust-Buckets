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

    private State state;

    Dictionary<MeshRenderer, Material> _originalMaterials;

    private void Start()
    {
        _originalMaterials = new Dictionary<MeshRenderer, Material>();
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            _originalMaterials.Add(childRenderer, childRenderer.material);  
        }
        state = State.Original;
    }

    public void ShowPlaceholder()
    {
        if (state == State.Placeholder) return;
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
        state = State.Original;
    }

    public void ShowIllegal()
    {
        if (state == State.Illegal) return;
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            if (childRenderer.material == placeholder)
            {
                childRenderer.material = illegal;
            }
        }
        state = State.Illegal;
    }

}

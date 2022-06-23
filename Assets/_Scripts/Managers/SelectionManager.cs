using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private static SelectionManager instance;
    private List<ISelectable> objects = new List<ISelectable> ();
    private ISelectable _selected;

    private void Awake()
    {
        instance = this;
    }

    public static void Register(ISelectable obj)
    {
        instance.objects.Add(obj);
    }

    public static void Unregister(ISelectable obj)
    {
        instance.objects.Remove(obj);
        if (instance._selected == obj) instance._selected = null;
    }

    public static void OnSelected(ISelectable obj)
    {
        instance.objects.ForEach(obj_i =>
        {
            if (obj != obj_i) obj_i.Deselect();
        });
        instance._selected = obj;
    }

    public static void OnDeselectAll()
    {
        instance.objects.ForEach(obj_i =>
        {
            obj_i.Deselect();
        });
        instance._selected = null;
    }

    public static void OnMove(Vector3 worldPos)
    {
        if (instance._selected == null) return;
        instance._selected.MoveTo(worldPos);
    }

    
}

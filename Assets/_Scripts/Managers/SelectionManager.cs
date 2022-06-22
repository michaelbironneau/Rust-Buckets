using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private static SelectionManager instance;
    private List<ISelectable> objects = new List<ISelectable> ();

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
    }

    public static void OnSelected(ISelectable obj)
    {
        instance.objects.ForEach(obj_i =>
        {
            if (obj != obj_i) obj_i.Deselect();
        });
    }

    
}

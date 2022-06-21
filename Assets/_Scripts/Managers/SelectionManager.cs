using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private static SelectionManager SM;
    private List<ISelectable> objects = new List<ISelectable> ();

    private void Awake()
    {
        if (SM != null)
        {
            GameObject.Destroy(SM);
        }
        SM = new SelectionManager();
    }

    public static void Register(ISelectable obj)
    {
        SM.objects.Add(obj);
    }

    public static void Unregister(ISelectable obj)
    {
        SM.objects.Remove(obj);
    }

    public static void OnSelected(ISelectable obj)
    {
        SM.objects.ForEach(obj_i =>
        {
            if (obj != obj_i) obj_i.Deselect();
        });
    }

    
}

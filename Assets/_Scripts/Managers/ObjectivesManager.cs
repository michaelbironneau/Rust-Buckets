using System.Collections.Generic;
using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    static ObjectivesManager instance;
    private Dictionary<string, ObjectiveController> _objectives;
    [SerializeField] GameObject prefab;

    private void Awake()
    {
        instance = this;
        instance._objectives = new Dictionary<string, ObjectiveController>();
    }

    public static void AddObjective(string text)
    {
        if (instance._objectives.ContainsKey(text)) return;
        GameObject obj = Instantiate(instance.prefab, instance.transform);
        instance._objectives.Add(text, obj.GetComponent<ObjectiveController>());
    }

    public static void CompleteObjective(string text)
    {
        if (!instance._objectives.ContainsKey(text)) return;
        ObjectiveController obj;
        if (!instance._objectives.TryGetValue(text, out obj)) return;
        obj.Complete();
        instance._objectives.Remove(text);
    }
}

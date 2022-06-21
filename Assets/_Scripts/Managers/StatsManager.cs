using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private static StatsManager instance;

    public struct Stats
    {
        public float O2; // Oxygen, in liters
        public int humans; // Humans, in units
        public float H20; // Water, in liters
        public float silicates; // Silicates from regolith, in Kg
        public float copper; // Copper from ore, in Kg
        public float nickel; // Nicklel from ore, in Kg
    }

    private Stats _stats;

    public static void ApplyUpdate(Stats delta)
    {
        instance._stats.O2 = Mathf.Max(0, instance._stats.O2 + delta.O2);
        instance._stats.humans = Mathf.Max(0, instance._stats.humans + delta.humans);
        instance._stats.H20 = Mathf.Max(0, instance._stats.H20 + delta.H20);
        instance._stats.silicates = Mathf.Max(0, instance._stats.silicates + delta.silicates);
        instance._stats.copper = Mathf.Max(0, instance._stats.copper + delta.copper);
        instance._stats.nickel = Mathf.Max(0, instance._stats.nickel + delta.nickel);
    }

    public static Stats GetLatest()
    {
        return instance._stats;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

}
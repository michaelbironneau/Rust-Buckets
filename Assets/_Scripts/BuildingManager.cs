using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingStatsManager : MonoBehaviour
{
    public enum Type
    {
        HabitatPod = 0,
        Smelter = 1,
        Platform = 2,
        SolarPanel = 3,
    };

    private static HashSet<GameObject> habitatPods = new HashSet<GameObject>();
    private static HashSet<GameObject> smelters = new HashSet<GameObject>();
    private static HashSet<GameObject> platforms = new HashSet<GameObject>();
    private static HashSet<GameObject> solarPanels = new HashSet<GameObject>();


    public static AddBuilding(Type buildingType, GameObject obj)
    {
        switch (buildingType)
        {
            case Type.HabitatPod:
                habitatPods.Add(obj);
                break;
            case Type.Smelter:
                smelters.Add(obj);
                break;
            case Type.Platform:
                platforms.Add(obj);
                break;
            case Type.SolarPanel:
                solarPanels.Add(obj);
                break;
            default:
                Debug.LogWarning("Unknown building type: " + buildingType);
        }
    }

    public static RemoveBuilding(Type buildingType, GameObject obj)
    {
        switch (buildingType)
        {
            case Type.HabitatPod:
                habitatPods.Remove(obj);
                break;
            case Type.Smelter:
                smelters.Remove(obj);
                break;
            case Type.Platform:
                platforms.Remove(obj);
                break;
            case Type.SolarPanel:
                solarPanels.Remove(obj);
                break;
            default:
                Debug.LogWarning("Unknown building type: " + buildingType);
        }
    }

    public static int Count(Type buildingType)
    {
        switch (buildingType)
        {
            case Type.HabitatPod:
                return habitatPods.Count();
            case Type.Smelter:
                return smelters.Count();
            case Type.Platform:
                return platforms.Count();
            case Type.SolarPanel:
                return solarPanels.Count();
            default:
                Debug.LogWarning("Uknown building type: " + buildingType);
                return 0;
        }
    }

}

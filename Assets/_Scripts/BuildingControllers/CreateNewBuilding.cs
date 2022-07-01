using UnityEngine;

public class CreateNewBuilding : MonoBehaviour
{

    [SerializeField] GameObject prefab;

    public void Create()
    {
        NewBuildingManager.SpawnNew(BuildingStatsManager.Type.HabitatPod, prefab, 1.82f);
    }

}

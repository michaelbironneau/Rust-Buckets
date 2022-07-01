using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNewBuilding : MonoBehaviour
{

    [SerializeField] GameObject prefab;

    public void Create()
    {
        NewBuildingManager.SpawnNew(prefab, 1.82f);
        ObjectivesManager.CompleteObjective("Build a habitat pod");
    }

}

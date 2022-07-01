using UnityEngine;

public class Level1 : MonoBehaviour
{

    public const string ObjectiveCreateHabitatPod = "Create a habitat pod";
    public const string ObjectiveMineRegolith = "Extract 100Kg of silicates";
    public const string ObjectiveMineCopper = "Extract 5Kg of copper";

    float _timeSinceLastUpdate;
    float _updateFrequency = 2.5f;

    bool HaveHabitatPod()
    {
        return BuildingStatsManager.Count(BuildingStatsManager.Type.HabitatPod) > 0;
    }

    bool HaveEnoughSilicates()
    {
        return StatsManager.GetLatest().silicates > 100;
    }

    bool HaveEnoughCopper()
    {
        return StatsManager.GetLatest().copper > 5;
    }


    void Update()
    {
        _timeSinceLastUpdate += Time.deltaTime;
        if (_timeSinceLastUpdate < _updateFrequency) return;

        if (HaveHabitatPod()){
            ObjectivesManager.CompleteObjective(ObjectiveCreateHabitatPod);
        }
        if (HaveEnoughSilicates()){
            ObjectivesManager.CompleteObjective(ObjectiveMineRegolith);
        }
        if (HaveEnoughCopper())
        {
            ObjectivesManager.CompleteObjective(ObjectiveMineCopper);
        }
        
    }
}

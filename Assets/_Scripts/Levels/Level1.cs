using System;
using System.Collections;
using UnityEngine;

public class Level1 : MonoBehaviour
{

    public const string ObjectiveCreateHabitatPod = "Create a habitat pod";
    public const string ObjectiveMineRegolith = "Extract 50Kg of silicates";
    public const string ObjectiveMineCopper = "Extract 5Kg of copper";

    float _timeSinceLastUpdate;
    float _updateFrequency = 2.5f;
    bool _complete = false;

    bool HaveHabitatPod()
    {
        return BuildingStatsManager.Count(BuildingStatsManager.Type.HabitatPod) > 0;
    }

    bool HaveEnoughSilicates()
    {
        return StatsManager.GetLatest().silicates > 50;
    }

    bool HaveEnoughCopper()
    {
        return StatsManager.GetLatest().copper > 5;
    }


    void Update()
    {
        if (_complete) return;
        _timeSinceLastUpdate += Time.deltaTime;
        if (_timeSinceLastUpdate < _updateFrequency) return;
        int _completion = 0;
        if (HaveHabitatPod()){
            ObjectivesManager.CompleteObjective(ObjectiveCreateHabitatPod);
            _completion++;
        }
        if (HaveEnoughSilicates()){
            ObjectivesManager.CompleteObjective(ObjectiveMineRegolith);
            _completion++;
        }
        if (HaveEnoughCopper())
        {
            ObjectivesManager.CompleteObjective(ObjectiveMineCopper);
            _completion++;
        }
        if (_completion == 3)
        {
            _complete = true;
            MessagesManager.Message msg = new MessagesManager.Message();
            msg.title = "Congratulations!";
            msg.body = "You have successfully completed the tutorial and created a pod. Your humans will have enough oxygen to survive.";
            MessagesManager.Show(msg);
        }
        
    }

    void DisplayPodMessage()
    {
        MessagesManager.Message podMessage = new MessagesManager.Message();
        podMessage.title = "Instructions";
        podMessage.body = $"Your oxygen is running out. Habitat pods have life support systems that can produce oxygen.{Environment.NewLine}Create a habitat pod before your oxyen runs out.";
        MessagesManager.Show(podMessage);
    }

    void DisplayMiningMessage()
    {
        MessagesManager.Message podMessage = new MessagesManager.Message();
        podMessage.title = "Hint";
        podMessage.body = $"Before you can build a pod, you'll need to assemble the necessary raw materials.{Environment.NewLine}Pods require silicates and copper.";
        MessagesManager.Show(podMessage);
    }
    void Start()
    {
        StartCoroutine(DisplayStartingMessages());
    }

    IEnumerator DisplayStartingMessages()
    {
        yield return new WaitForSeconds(2f);
        DisplayPodMessage();
        DisplayMiningMessage();
        yield return new WaitForSeconds(2f);
        ObjectivesManager.AddObjective(Level1.ObjectiveMineRegolith);
        ObjectivesManager.AddObjective(Level1.ObjectiveMineCopper);
        ObjectivesManager.AddObjective(Level1.ObjectiveCreateHabitatPod);
    }
}

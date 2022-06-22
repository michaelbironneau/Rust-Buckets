using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntroController : MonoBehaviour
{
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
        podMessage.body = $"Before you can build a pod, you'll need to assemble the necessary raw materials.{Environment.NewLine}. Pods require silicates, copper and nickel.";
        MessagesManager.Show(podMessage);
    }
    void Start()
    {
        StartCoroutine(DisplayStartingMessages());
    }

    IEnumerator DisplayStartingMessages()
    {
        yield return new WaitForSeconds(5f);
        DisplayPodMessage();
        yield return new WaitForSeconds(2f);
        DisplayMiningMessage();
    }

}

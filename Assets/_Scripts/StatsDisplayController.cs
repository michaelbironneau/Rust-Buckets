using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsDisplayController : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI humans;
    [SerializeField] TextMeshProUGUI oxygen;
    [SerializeField] TextMeshProUGUI water;
    [SerializeField] TextMeshProUGUI silicates;
    [SerializeField] TextMeshProUGUI copper;
    [SerializeField] TextMeshProUGUI nickel;
    [SerializeField] float updateFreqSeconds = 0.5f;


    float _timeSinceLastUpdate = 0f;

    void Start()
    {
        DisplayStats(StatsManager.GetLatest());        
    }

    void DisplayStats(StatsManager.Stats stats)
    {
        humans.text = stats.humans.ToString();
        oxygen.text = Mathf.Round(stats.O2).ToString();
        water.text = Mathf.Round(stats.H20).ToString();
        silicates.text = Mathf.Round(stats.silicates).ToString();
        copper.text = Mathf.Round(stats.copper).ToString();
        nickel.text = Mathf.Round(stats.nickel).ToString();
    }

    void Update()
    {
        _timeSinceLastUpdate += Time.deltaTime;
        if (_timeSinceLastUpdate >= updateFreqSeconds)
        {
            DisplayStats(StatsManager.GetLatest());
            _timeSinceLastUpdate = 0;
        }
    }
}

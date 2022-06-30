using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HumanStatsController : MonoBehaviour
{
    [SerializeField] float O2ConsumptionPerHour = 23f; // in L
    [SerializeField] float H20ConsumptionPerHour = 83f; // in ml
    [SerializeField] float UpdateFrequencySeconds = 2;
    [SerializeField] int InitialHumans = 3;
    [SerializeField] Image FadeToBlackImage;
    [SerializeField] TextMeshProUGUI GameOverText;
    [SerializeField] GameObject GameOverCanvas;
    [SerializeField] float DoNothingMinutes = 10; // amount of time the player can initially sit around and do nothing without dying
    
    private bool _running = false;
    private float _TimeRemainingSeconds = 0f;
    private string _TimeRemainingReason = "";

    void Start()
    {
        _running = true;
        GameOverCanvas.SetActive(false);
        SeedStats();
        StartCoroutine(UpdateStats());
    }

    private void SeedStats()
    {
        StatsManager.Stats stats = new StatsManager.Stats();
        stats.humans = InitialHumans;
        stats.O2 = O2ConsumptionPerHour * (DoNothingMinutes/60) * InitialHumans; //10 minutes of gameplay if nothing happens
        stats.H20 = H20ConsumptionPerHour * (DoNothingMinutes / 60) * InitialHumans;
        StatsManager.ApplyUpdate(stats);
    }

    public float TimeRemaining()
    {
        return _TimeRemainingSeconds;
    }

    public string TimeRemainingReason()
    {
        return _TimeRemainingReason;
    }


    private void OnDestroy()
    {
        _running = false; // stop UpdateStats coroutine after next iteration
    }

    private IEnumerator GameOver()
    {
        GameOverCanvas.SetActive(true);
        for (float i = 0; i < 1; i += 0.02f)
        {
            Color tint = FadeToBlackImage.color;
            tint.a = i;
            FadeToBlackImage.color = tint;
            yield return new WaitForSeconds(0.05f);
        }
        for (float i = 0; i < 1; i += 0.1f)
        {
            Color tint = GameOverText.color;
            tint.a = i;
            GameOverText.color = tint;
            yield return new WaitForSeconds(0.05f);
        }

    }

    IEnumerator UpdateStats()
    {
        while (_running)
        {
            StatsManager.Stats current = StatsManager.GetLatest();
            StatsManager.Stats update = new StatsManager.Stats();
            float _O2TimeRemainingSeconds = (current.O2 / (current.humans*O2ConsumptionPerHour)) * 3600f;
            float _H2OTimeRemainingSeconds = (current.H20 / (current.humans*H20ConsumptionPerHour)) * 3600f;
            if (_H2OTimeRemainingSeconds> _O2TimeRemainingSeconds)
            {
                _TimeRemainingSeconds = _O2TimeRemainingSeconds;
                _TimeRemainingReason = "Oxygen";
            } else
            {
                _TimeRemainingSeconds = _H2OTimeRemainingSeconds;
                _TimeRemainingReason = "Water";
            }
            update.O2 = -1 * (UpdateFrequencySeconds / 3600) * O2ConsumptionPerHour * current.humans;
            update.H20 = -1 * (UpdateFrequencySeconds / 3600) * H20ConsumptionPerHour * current.humans;
            StatsManager.ApplyUpdate(update);
            if (_TimeRemainingSeconds <= 0)
            {
                StartCoroutine(GameOver());
                yield break;
            }
            yield return new WaitForSeconds(UpdateFrequencySeconds);
        }
        
    }
}

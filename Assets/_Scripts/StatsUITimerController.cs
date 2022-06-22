using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUITimerController : MonoBehaviour
{
    private bool _running = false;
    [SerializeField] HumanStatsController _humanStatsController;
    [SerializeField] TextMeshProUGUI _timeRemainingText;
    [SerializeField] TextMeshProUGUI _timeRemainingReasonText;

    // Start is called before the first frame update
    void Start()
    {
        _running = true;
        _timeRemainingText.enabled = false;
        _timeRemainingReasonText.enabled = false;
        StartCoroutine(UpdateCountdown());
    }

    private void OnDestroy()
    {
        _running = false;
    }

    private IEnumerator UpdateCountdown()
    {
        while (_running)
        {
            float timeRemaining = _humanStatsController.TimeRemaining();
            string timeRemainingReason = _humanStatsController.TimeRemainingReason();
            float minutesRemaining = Mathf.Floor(timeRemaining / 60);
            float secondsRemaining = Mathf.Floor(timeRemaining - 60 * minutesRemaining);
            _timeRemainingText.text = Mathf.Floor(timeRemaining/60).ToString() + " min " + secondsRemaining.ToString() + " sec ";
            _timeRemainingReasonText.text = "of " + timeRemainingReason + " remaining";
            _timeRemainingText.enabled = true;
            _timeRemainingReasonText.enabled = true;
            yield return new WaitForSeconds(1f);
        }
    }
}

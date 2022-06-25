using UnityEngine;
using UnityEngine.UI;
public class VehiclePowerController : MonoBehaviour
{
    [SerializeField] float maxEnergy = 100f;
    [SerializeField] Image energyBar;
    [SerializeField] GameObject energyUI;
    [SerializeField] float maxPower = 10f;
    float _currentEnergy = 0f;
    float _currentPower = 0f;

   
    private void OnValidate()
    {
        if (maxEnergy < 0f)
        {
            maxEnergy = 0.1f;
        }
    }

    public void HideUI()
    {
        energyUI.SetActive(false);
    }

    public void ShowUI()
    {
        energyUI.SetActive(true);
    }

    private void Start()
    {
        HideUI();
        _currentEnergy = maxEnergy;
    }

    public bool CanCharge()
    {
        return _currentEnergy < maxEnergy;
    }

    public bool CanDischarge()
    {
        return _currentEnergy > 0;
    }


    public void SetPower(float rate)
    {
        _currentPower = rate;
    }

    public void SetPowerPercent(float percent)
    {
        _currentPower = 0.01f*percent*maxPower;
    }


    void Update()
    {
        float _newCurrentEnergy = _currentEnergy + _currentPower * Time.deltaTime;
        _currentEnergy = Mathf.Clamp(_newCurrentEnergy, 0f, maxEnergy);
        energyBar.fillAmount = _currentEnergy / maxEnergy;
        
    }
}

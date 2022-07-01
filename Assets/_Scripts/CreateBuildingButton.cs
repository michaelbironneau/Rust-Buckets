using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreateBuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] BuildingStatsManager.Type buildingType;
    [SerializeField] float silicates;
    [SerializeField] float copper;
    [SerializeField] float nickel;
    [SerializeField] GameObject prefab;
    [SerializeField] Color active;
    [SerializeField] Color disabled;
    [SerializeField] Color clicked;
    [SerializeField] Color hover;
    [SerializeField] float verticalOffset = 0f;

    Image _img;
    float _timeSinceLastUpdate;
    float _updateFrequency = 1f;
    bool _canBuild;
    bool _hover;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _hover = true;
    }

    bool CanBuild()
    {
        StatsManager.Stats stats = StatsManager.GetLatest();
        return (stats.silicates >= silicates && stats.copper >= copper && stats.nickel >= nickel);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hover = false;
    }

    void SetIdleColor()
    {
        if (CanBuild())
        {
            _img.color = active;
        }
        else
        {
            _img.color = disabled;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _img = GetComponent<Image>();
    }

    void Build()
    {
        NewBuildingManager.SpawnNew(buildingType, prefab, verticalOffset);
        _timeSinceLastUpdate = _updateFrequency + 1; //force _canBuild update on next frame as resources will have gone down
    }

    // Update is called once per frame
    void Update()
    {
        _timeSinceLastUpdate += Time.deltaTime;
        if (_timeSinceLastUpdate > _updateFrequency)
        {
            _canBuild = CanBuild();
            _timeSinceLastUpdate = 0;
        }
        if (!_hover)
        {
            // most likely case
            if (_canBuild)
            {
                _img.color = active;
            } else
            {
                _img.color = disabled;
            }
            return;
        }
        // hovering
        if (_canBuild)
        {
            if (Input.GetMouseButton(0))
            {
                // clicked
                _img.color = clicked;
                Build();
            } else
            {
                _img.color = hover;
            }
        } // don't do anything if can't build

    }
}

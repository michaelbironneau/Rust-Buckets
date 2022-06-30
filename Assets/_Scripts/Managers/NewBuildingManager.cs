using UnityEngine;

public class NewBuildingManager : MonoBehaviour
{
    static NewBuildingManager instance;
    GameObject _spawn;
    BuildableMaterialController _bmc;
    Collider _spawnCollider;
    float _clearance = 5f;

    private void Awake()
    {
        instance = this;
    }

    public static void SpawnNew(GameObject prefab)
    {
        if (instance._spawn != null)
        {
            instance.CancelSpawn();
        }
        GroundSelectionController.enableMouseTracking = true;
        instance._bmc = prefab.GetComponent<BuildableMaterialController>();
        if (instance._bmc == null)
        {
            Debug.LogWarning("Could not find BuildableMaterialController in spawn");
            return;
        }

        instance._bmc.ShowPlaceholder();
    }

    void DoneSpawning()
    {
        if (instance._bmc != null) instance._bmc.ShowFinal();
        GroundSelectionController.enableMouseTracking = false;
        _spawn = null;
    }

    void CancelSpawn()
    {
        Destroy(_spawn);
        _spawn = null;
        GroundSelectionController.enableMouseTracking = false;
    }

    bool LegalLocation()
    {
        if (_spawnCollider == null) return true;
        Collider[] colliders = Physics.OverlapSphere(_spawn.transform.position, _clearance);
        foreach (Collider collider in colliders)
        {
            if (collider != _spawnCollider  && collider.gameObject.tag != "Terrain") return false;
        }
        return true;
    }

    void UpdatePosition()
    {
        if (_spawn != null) _spawn.transform.position = GroundSelectionController.mousePosition;
    }


    // Update is called once per frame
    void Update()
    {
        if (_spawn == null) return;

        if (Input.GetMouseButton(0))
        {
            // left click - if we're in a legal location, finalise the building. Otherwise, destroy it.
            DoneSpawning();
            //TODO: Add this to a list of buildings somewhere??

        }
        else if (Input.GetMouseButton(1))
        {
            // right click - destroy temporary building
            CancelSpawn();
        }
    }
}

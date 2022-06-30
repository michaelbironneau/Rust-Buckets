using UnityEngine;

public class NewBuildingManager : MonoBehaviour
{
    static NewBuildingManager instance;
    GameObject _spawn;
    BuildableMaterialController _bmc;
    Collider _spawnCollider;
    float _clearance = 3f;
    float _verticalOffset;

    private void Awake()
    {
        instance = this;
    }

    public static void SpawnNew(GameObject prefab, float verticalOffset)
    {
        if (instance._spawn != null)
        {
            instance.CancelSpawn();
        }
        instance._verticalOffset = verticalOffset;
        instance._spawn = Object.Instantiate(prefab, Vector3.up*verticalOffset, Quaternion.identity);
     
        instance._spawnCollider = instance._spawn.GetComponent<Collider>();
        GroundSelectionController.enableMouseTracking = true;
        instance._bmc = instance._spawn.GetComponent<BuildableMaterialController>();
        if (instance._bmc == null)
        {
            Debug.LogWarning("Could not find BuildableMaterialController in spawn");
            return;
        }
        //instance.DisableChildColliders();
        instance._bmc.ShowPlaceholder();
    }

    void DisableChildColliders()
    {
        Collider[] colliders = instance._spawn.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            if (collider != _spawnCollider) collider.enabled = false;
        }
    }

    void EnableChildColliders()
    {
        Collider[] colliders = instance._spawn.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
           collider.enabled = true;
        }
    }

    void DoneSpawning()
    {
        if (instance._bmc != null) instance._bmc.ShowFinal();
        GroundSelectionController.enableMouseTracking = false;
        //instance.EnableChildColliders();
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
            if (collider != _spawnCollider && collider.gameObject.tag != "Terrain")
            {
                //Debug.Log("Illegal: " + collider.gameObject.name);
                return false;
            }
        }
        return true;
    }

    void UpdatePosition()
    {
        if (_spawn != null) _spawn.transform.position = GroundSelectionController.mousePosition + Vector3.up*instance._verticalOffset;
    }


    // Update is called once per frame
    void Update()
    {
        if (_spawn == null) return;
        if (MessagesManager.visible) return; // don't do anything while this is visible
        UpdatePosition();
        bool legal = LegalLocation();
        
        if (legal)
        {
            instance._bmc.ShowPlaceholder();
        } else
        {
            instance._bmc.ShowIllegal();
        }
        if (legal && Input.GetMouseButton(0))
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

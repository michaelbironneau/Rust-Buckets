using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmeltingPlantController : MonoBehaviour
{

    public void Smelt(GameObject ore)
    {
        if (ore.tag != "Ore")
        {
            Debug.LogWarning("Non-ore object inserted into smelter!");
        }
        Debug.Log("Smelting " + ore.name);

        //TODO: Improve this!
        Rigidbody rb = ore.GetComponent<Rigidbody>();
        //rb.isKinematic = true;
        ore.transform.parent = transform;
        ore.transform.position = transform.position + Vector3.up*2.5f;

        StatsManager.Stats delta = new StatsManager.Stats();
        delta.copper += rb.mass * 0.05f;
        StatsManager.ApplyUpdate(delta);        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

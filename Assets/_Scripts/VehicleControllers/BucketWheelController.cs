using UnityEngine;

public class BucketWheelController : MonoBehaviour
{

    [SerializeField] RegolithMinerController regolithMinerController;
    Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        // TODO: Which collider have we collided with? Does it matter?
        // If we're not mining we won't do anything anyway -
        // and if we are, the regolith miner can't move, so the only possible collision is with the ground.
        regolithMinerController.OnBucketWheelGrounded(); 
    }

    private void OnTriggerExit(Collider other)
    {
        // TODO: Which collider have we collided with? Does it matter?
        // If we're not mining we won't do anything anyway -
        // and if we are, the regolith miner can't move, so the only possible collision is with the ground.

        regolithMinerController.OnBucketWheelAirborne();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereObjectiveTrigger : MonoBehaviour
{
    //public ObjectiveSystem objectiveSystem;
    // Start is called before the first frame update
    void Start()
    {
        // if (objectiveSystem == null) {
        //     Debug.Log("Trigger lacks objectivesystem reference!");
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        ObjectiveSystem objectiveSystem = other.GetComponent<ObjectiveSystem>();
        if (objectiveSystem) {
            objectiveSystem.destinationEvent(transform.gameObject);
            Debug.Log("Entered");
        }
    }
}

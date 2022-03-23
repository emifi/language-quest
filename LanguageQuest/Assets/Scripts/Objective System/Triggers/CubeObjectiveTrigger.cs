using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeObjectiveTrigger : MonoBehaviour
{
    public DestinationObject trigger;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        ObjectiveSystem objectiveSystem = other.GetComponent<ObjectiveSystem>();
        if (objectiveSystem) {
            objectiveSystem.destinationEvent(trigger);
        }
    }
}
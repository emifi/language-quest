using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSystem : MonoBehaviour
{
    public List<Objective> objectives = new List<Objective>();
    public GameObject objectiveUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pickupEvent(PickupObject item) {
        foreach (Objective objective in objectives) {
            if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.Collect) continue;
            if (objective.item == item) {
                objective.current += 1;
                if (objective.current == objective.required) objective.status = ObjectiveStatus.Complete;
            }
        }
    }

    public void triggerEvent(TriggerObject trigger) {
        foreach (Objective objective in objectives) {
            if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.Trigger) continue;
            if (objective.trigger == trigger) objective.status = ObjectiveStatus.Complete;
        }
    }

    public void destinationEvent(GameObject destination) {
        foreach (Objective objective in objectives) {
            if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.MoveTo) continue;
            if (objective.destination == destination) {
                objective.status = ObjectiveStatus.Complete;
            }
        }
    }
}

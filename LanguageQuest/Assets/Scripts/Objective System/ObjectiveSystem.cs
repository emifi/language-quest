using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveSystem : MonoBehaviour
{
    public List<Objective> objectives = new List<Objective>();
    public GameObject objectiveUI;
    public GameObject panelUI;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Objective obj in objectives) {
            addObjective(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            addObjective(new Objective(ObjectiveType.Collect, "Pick up bucket", Resources.Load<PickupObject>("Items/Bucket"), 0, 1));
        }
    }

    public void addObjective(Objective objective) {
        if (!objectives.Contains(objective)) objectives.Add(objective);
        objective.panelUI = Instantiate(panelUI);
        string objstr = objective.objectiveString;
        if (objective.type == ObjectiveType.Collect) {
            objective.panelUI.GetComponent<TMP_Text>().text = $" • {objstr} {objective.current}/{objective.required}";
        }
        else {
            objective.panelUI.GetComponent<TMP_Text>().text = $" • {objstr}";
        }
        objective.panelUI.transform.SetParent(objectiveUI.transform);
    }

    public void removeAllObjectives() {
        foreach (Objective objective in objectives) {
            Destroy(objective.panelUI);
        }
        objectives.Clear();
    }

    public void pickupEvent(PickupObject item) {
        foreach (Objective objective in objectives) {
            if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.Collect) continue;
            if (objective.item == item) {
                objective.current += 1;
                if (objective.current == objective.required) objective.status = ObjectiveStatus.Complete;
                updateUI(objective);
            }
        }
    }

    public void triggerEvent(TriggerObject trigger) {
        foreach (Objective objective in objectives) {
            if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.Trigger) continue;
            if (objective.trigger == trigger) {
                objective.status = ObjectiveStatus.Complete;
                updateUI(objective);
            }
        }
    }

    public void destinationEvent(DestinationObject destination) {
        foreach (Objective objective in objectives) {
            if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.MoveTo) continue;
            if (objective.destination == destination) {
                objective.status = ObjectiveStatus.Complete;
                updateUI(objective);
            }
        }
    }

    void updateUI(Objective objective) {
        string objstr = objective.objectiveString;
        if (objective.type == ObjectiveType.Collect) {
            if (objective.status == ObjectiveStatus.Complete) {
                objective.panelUI.GetComponent<TMP_Text>().text = $" • <s>{objstr} {objective.current}/{objective.required}</s>";
            }
            else {
                objective.panelUI.GetComponent<TMP_Text>().text = $" • {objstr} {objective.current}/{objective.required}";
            }
        }
        else {
            objective.panelUI.GetComponent<TMP_Text>().text = $" • <s>{objstr}</s>";
        }
     }
}

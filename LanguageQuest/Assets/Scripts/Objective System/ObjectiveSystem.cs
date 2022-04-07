using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveSystem : MonoBehaviour
{
    public List<Objective> objectives = new List<Objective>();
    public List<Objective> completeObjectives = new List<Objective>();
    public GameObject objectiveUI;
    public GameObject panelUI;

    Objective example;
    Objective example_2;
    Objective example_3;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Objective obj in objectives) {
            addObjective(obj);
        }
        // example of a 'collect' objective
        example = new Objective(ObjectiveType.Collect, "Pick up bucket", Resources.Load<PickupObject>("Items/Bucket"), 0, 1);
        // example of a 'moveto' objective
        example_2 = new Objective(ObjectiveType.MoveTo, "Move to the NW corner", Resources.Load<DestinationObject>("Destinations/NW"));
        // example of a 'trigger' objective
        example_3 = new Objective(ObjectiveType.Trigger, "Read the book", Resources.Load<TriggerObject>("Items/Book"));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0)) {
            addObjective(example);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            addObjective(example_2);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)) {
            addObjective(example_3);
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            removeAllObjectives();
        }
    }

    // Adds an Objective to the UI and to the current objectives container
    // The container object is only public for editor convenience! Never add directly to it!
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

    // Starts coroutine to fade out and remove the specified objective
    public void removeObjective(Objective objective) {
        StartCoroutine(fadeAndRemove(0f, objective));
    }

    IEnumerator fadeAndRemove(float t, Objective objective) {
        yield return new WaitForSeconds(2.0f);
        while (t < 2.0) {
            objective.panelUI.GetComponent<TMP_Text>().color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), t/2.0f);
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(objective.panelUI);
        objectives.Remove(objective);
        completeObjectives.Add(objective);
    }

    public void removeAllObjectives() {
        foreach (Objective objective in objectives) {
            removeObjective(objective);
        }
    }

    // Fires from PlayerInteraction.cs when a pickup interaction is detected
    public void pickupEvent(PickupObject item) {
        foreach (Objective objective in objectives) {
            if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.Collect) continue;
            if (objective.item == item) {
                objective.current += 1;
                if (objective.current == objective.required) {
                    objective.status = ObjectiveStatus.Complete;
                    StartCoroutine(fadeAndRemove(0.0f, objective));
                }
                updateUI(objective);
            }
        }
    }

    // Fires from PlayerInteraction.cs when an interaction with a trigger item is detected
    public void triggerEvent(TriggerObject trigger) {
        foreach (Objective objective in objectives) {
            if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.Trigger) continue;
            if (objective.trigger == trigger) {
                objective.status = ObjectiveStatus.Complete;
                StartCoroutine(fadeAndRemove(0.0f, objective));
                updateUI(objective);
            }
        }
    }

    // Fires from PlayerInteraction.cs when the player enters a collision trigger
    public void destinationEvent(DestinationObject destination) {
        foreach (Objective objective in objectives) {
            if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.MoveTo) continue;
            if (objective.destination == destination) {
                objective.status = ObjectiveStatus.Complete;
                StartCoroutine(fadeAndRemove(0.0f, objective));
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

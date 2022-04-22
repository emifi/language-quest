using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveSystem : MonoBehaviour
{
    // Completed objectives are still rendered on screen
    public List<Objective> objectives = new List<Objective>();
    public List<Objective> completeObjectives = new List<Objective>();
    public List<Objective> hiddenObjectives = new List<Objective>();
    public GameObject panelUI;
    public GameObject objectiveUI;
    public InventoryObject inventory;
    DialogueUI dialogueUI;
    Color[] pallete = new Color[] {
        new Color(
            186f/256f,
            220f/256f, 
            88f/256f
        ),
        new Color(
            255f/256f, 
            121f/256f, 
            121f/256f
        ),
        new Color(
            255f/256f, 
            190f/256f, 
            118f/256f
        ),
        new Color( //Sky Blue
            126f/256f,
            237f/256f, 
            219f/256f
        ),
        new Color( //Purple
            141f/256f,
            126f/256f, 
            237f/256f
        ),
        new Color( //Lavender
            200f/256f,
            126f/256f, 
            237f/256f
        ),
        new Color( //Pink
            237f/256f,
            126f/256f, 
            235f/256f
        )
    };
    // Start is called before the first frame update
    void Start()
    {
        //objectiveUI = GameObject.Find("ObjectiveUI");
        dialogueUI = GameObject.Find("DialogueUI").GetComponent<DialogueUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Adds an Objective to the UI and to the current objectives container
    // The container object is only public for editor convenience! Never add directly to it!
    public void addObjective(Objective objective, int color_num) {
        Color color = pallete[color_num % pallete.Length];
        if (!objectives.Contains(objective)) objectives.Add(objective);
        objective.panelUI = Instantiate(panelUI);
        objective.panelUI.GetComponent<TMP_Text>().color = color;
        string objstr = objective.objectiveString;
        if (objective.type == ObjectiveType.Collect) {
            if (inventory.GetItem((objective as ObjectiveCollect).item) > (objective as ObjectiveCollect).current) {
                (objective as ObjectiveCollect).current = (objective as ObjectiveCollect).required;
            } else {
                (objective as ObjectiveCollect).current = inventory.GetItem((objective as ObjectiveCollect).item);
            }
            if ((objective as ObjectiveCollect).current >= (objective as ObjectiveCollect).required) {
                (objective as ObjectiveCollect).status = ObjectiveStatus.Complete;
                (objective as ObjectiveCollect).panelUI.GetComponent<TMP_Text>().text = $" • <s>{objstr} {(objective as ObjectiveCollect).current}/{(objective as ObjectiveCollect).required}</s>";
            }
            else {
                (objective as ObjectiveCollect).panelUI.GetComponent<TMP_Text>().text = $" • {objstr} {(objective as ObjectiveCollect).current}/{(objective as ObjectiveCollect).required}";
            }
        }
        else {
            objective.panelUI.GetComponent<TMP_Text>().text = $" • {objstr}";
        }
        objective.panelUI.transform.SetParent(objectiveUI.transform);
    }

    public void addObjectiveList(List<Objective> objectives, int color_num) {
        foreach (Objective objective in objectives) {
            addObjective(objective, color_num);
        }
    }

    // Starts coroutine to fade out and remove the specified objective
    public void removeObjective(Objective objective) {
        StartCoroutine(fadeAndRemove(0f, objective));
    }

    IEnumerator fadeAndRemove(float t, Objective objective) {
        yield return new WaitForSeconds(2.0f);
        while (t < 2.0) {
            objective.panelUI.GetComponent<TMP_Text>().color = Color.Lerp(objective.panelUI.GetComponent<TMP_Text>().color, new Color(1f, 1f, 1f, 0f), t/2.0f);
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(objective.panelUI);
    }

    public void removeCompletedObjectives() {
        foreach (Objective objective in completeObjectives) {
            removeObjective(objective);
        }
    }

    public void removeCompletedObjectives(List<Objective> objs) {
        foreach (Objective obj in objs) {
            removeObjective(obj);
        }
    }
    // Fires from PlayerInteraction.cs when a pickup interaction is detected
    public void pickupEvent(PickupObject item) {
        List<Objective> objectivesToRemove = new List<Objective>();
        foreach (Objective objective in objectives) {
            //if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.Collect) continue;
            if ((objective as ObjectiveCollect).item == item) {
                (objective as ObjectiveCollect).current = inventory.GetItem(item);
                if ((objective as ObjectiveCollect).current >= (objective as ObjectiveCollect).required) {
                    objective.status = ObjectiveStatus.Complete;
                    objectivesToRemove.Add(objective);
                }
                updateUI(objective);
            }
        }
        foreach (Objective objective in objectivesToRemove) {
            objectives.Remove(objective);
            completeObjectives.Add(objective);
        }
        objectivesToRemove = new List<Objective>();
        foreach (Objective objective in hiddenObjectives) {
            //if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.Collect) continue;
            if ((objective as ObjectiveCollect).item == item) {
                (objective as ObjectiveCollect).current = inventory.GetItem(item);
                if ((objective as ObjectiveCollect).current >= (objective as ObjectiveCollect).required) {
                    objective.status = ObjectiveStatus.Complete;
                }
            }
        }
        foreach (Objective objective in objectivesToRemove) {
            hiddenObjectives.Remove(objective);
        }
    }

    // Fires from PlayerInteraction.cs when an interaction with a trigger item is detected
    public void triggerEvent(TriggerObject trigger) {
        List<Objective> objectivesToRemove = new List<Objective>();
        foreach (Objective objective in objectives) {
            //if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.Trigger) continue;
            if ((objective as ObjectiveTrigger).trigger == trigger) {
                objective.status = ObjectiveStatus.Complete;
                objectivesToRemove.Add(objective);
                updateUI(objective);
            }
        }
        foreach (Objective objective in objectivesToRemove) {
            objectives.Remove(objective);
            completeObjectives.Add(objective);
        }
        objectivesToRemove = new List<Objective>();
        foreach (Objective objective in hiddenObjectives) {
            //if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.Trigger) continue;
            if ((objective as ObjectiveTrigger).trigger == trigger) {
                objective.status = ObjectiveStatus.Complete;
                objectivesToRemove.Add(objective);
            }
        }
        foreach (Objective objective in objectivesToRemove) {
            hiddenObjectives.Remove(objective);
        }
    }

    // Fires from PlayerInteraction.cs when the player enters a collision trigger
    public void destinationEvent(DestinationObject destination) {
        List<Objective> objectivesToRemove = new List<Objective>();
        foreach (Objective objective in objectives) {
            //if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.MoveTo) continue;
            if ((objective as ObjectiveMoveTo).destination == destination) {
                objective.status = ObjectiveStatus.Complete;
                objectivesToRemove.Add(objective);
                updateUI(objective);
            }
        }
        foreach (Objective objective in objectivesToRemove) {
            objectives.Remove(objective);
            completeObjectives.Add(objective);
        }
        objectivesToRemove = new List<Objective>();
        foreach (Objective objective in hiddenObjectives) {
            //if (objective.status == ObjectiveStatus.Complete) continue;
            if (objective.type != ObjectiveType.MoveTo) continue;
            if ((objective as ObjectiveMoveTo).destination == destination) {
                objective.status = ObjectiveStatus.Complete;
                objectivesToRemove.Add(objective);
            }
        }
        foreach (Objective objective in objectivesToRemove) {
            hiddenObjectives.Remove(objective);
        }
    }

    public void talkToEvent(NPCIdentifierObject npc) {
        List<Objective> objectivesToRemove = new List<Objective>();
        foreach (Objective objective in objectives) {
            if (objective.type != ObjectiveType.TalkTo) continue;
            if ((objective as ObjectiveTalkTo).npcID == npc) {
                objective.status = ObjectiveStatus.Complete;
                objectivesToRemove.Add(objective);
                updateUI(objective);
            }
        }
        foreach (Objective objective in objectivesToRemove) {
            objectives.Remove(objective);
            completeObjectives.Add(objective);
        }
        objectivesToRemove = new List<Objective>();
        foreach (Objective objective in hiddenObjectives) {
            if (objective.type != ObjectiveType.TalkTo) continue;
            if ((objective as ObjectiveTalkTo).npcID == npc) {
                objective.status = ObjectiveStatus.Complete;
                objectivesToRemove.Add(objective);
            }
        }
        foreach (Objective objective in objectivesToRemove) {
            hiddenObjectives.Remove(objective);
        }
    }

    public void miscEvent(ObjectiveMisc obj) {
        List<Objective> objectivesToRemove = new List<Objective>();
        foreach (Objective objective in objectives) {
            if (objective.type != ObjectiveType.Misc) continue;
            if (objective == obj) {
                objective.status = ObjectiveStatus.Complete;
                objectivesToRemove.Add(objective);
                updateUI(objective);
            }
        }
        foreach (Objective objective in objectivesToRemove) {
            objectives.Remove(objective);
            completeObjectives.Add(objective);
        }
        objectivesToRemove = new List<Objective>();
        foreach (Objective objective in hiddenObjectives) {
            if (objective.type != ObjectiveType.Misc) continue;
            if (objective == obj) {
                objective.status = ObjectiveStatus.Complete;
                objectivesToRemove.Add(objective);
            }
        }
        foreach (Objective objective in objectivesToRemove) {
            hiddenObjectives.Remove(objective);
        }
    }

    void updateUI(Objective objective) {
        string objstr = objective.objectiveString;
        if (objective.type == ObjectiveType.Collect) {
            if (objective.status == ObjectiveStatus.Complete) {
                objective.panelUI.GetComponent<TMP_Text>().text = $" • <s>{objstr} {(objective as ObjectiveCollect).current}/{(objective as ObjectiveCollect).required}</s>";
            }
            else {
                objective.panelUI.GetComponent<TMP_Text>().text = $" • {objstr} {(objective as ObjectiveCollect).current}/{(objective as ObjectiveCollect).required}";
            }
        }
        else {
            objective.panelUI.GetComponent<TMP_Text>().text = $" • <s>{objstr}</s>";
        }
     }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereObjectiveTrigger : MonoBehaviour
{
    public DestinationObject trigger;
    [Header("Optional: Wait for an NPC to enter destination first")]
    public NPCIdentifierObject npc;
    ObjectiveSystem objectiveSystem;

    bool player_entered = false;
    bool npc_entered = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (objectiveSystem) {
            if (npc == null && player_entered) {
                objectiveSystem.destinationEvent(trigger);
            } else if (player_entered && npc_entered) {
                objectiveSystem.destinationEvent(trigger);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<ObjectiveSystem>()) objectiveSystem = other.GetComponent<ObjectiveSystem>();
        if (other.GetComponent<ObjectiveSystem>()) {
            player_entered = true;
        }
        NPCInteractable npc_interactable = other.GetComponent<NPCInteractable>();
        if (npc_interactable && npc_interactable.id == npc) {
            npc_entered = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<ObjectiveSystem>()) objectiveSystem = other.GetComponent<ObjectiveSystem>();
        if (other.GetComponent<ObjectiveSystem>()) {
            player_entered = false;
        }
        NPCInteractable npc_interactable = other.GetComponent<NPCInteractable>();
        if (npc_interactable && npc_interactable.id == npc) {
            npc_entered = false;
        }
    }
}

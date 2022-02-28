using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 3f;
    public float highlightRange = 6f;
    List<Collider> interactionColliders;
    List<Collider> highlightColliders;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] currColliders = Physics.OverlapSphere(transform.position, highlightRange);
        foreach (Collider collision in highlightColliders) {
            if (collision.gameObject.GetComponent<Interactable>() != null) {
                if (!highlightColliders.Contains(collision)) {
                    highlightColliders.Add(collision);
                }
            }
        }
    }

    private Collider[] outOfRangeColliders() {
        return null;
    }
}

using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractableRaycast : MonoBehaviour
{
    private float raylength = 5;

    private KeyCode killEnemy = KeyCode.Mouse0;

    public Image crosshair;

    private const string interactableTag = "Enemy";

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        // Button Raycast
        if (Physics.Raycast(transform.position, forward, out hit, raylength))
        {
            // Make sure we only destroy objects with the Enemy tag
            if (hit.collider.CompareTag(interactableTag))
            {
                CrosshairChange(true);

                if (Input.GetKeyDown(killEnemy))
                {
                    // Check if the enemy is not the player or parented player
                    GameObject target = hit.collider.gameObject;

                    // If the enemy has the player as a child, detach player first
                    PlayerController playerController = target.GetComponentInChildren<PlayerController>();
                    if (playerController != null)
                    {
                        // Release the player if attached
                        playerController.transform.SetParent(null);
                        playerController.EnableMovement();
                    }

                    // Destroy the enemy
                    Destroy(target);
                    Debug.Log("Enemy destroyed while attached!");
                }
            }
        }
        else
        {
            CrosshairChange(false);
        }
    }


    void CrosshairChange(bool on)
    {
        if (on)
        {
            crosshair.color = Color.red;
        }
        else
        {
            crosshair.color = Color.white;
        }
    }
}
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractableRaycast : MonoBehaviour
{
    private float raylength = 5;

    public Image crosshair;

    public bool collectedPlank;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Debug.DrawLine(transform.position, transform.position + forward * 5, Color.blue);
        // Button Raycast
        if (Physics.Raycast(transform.position, forward, out hit, raylength))
        {
            print(hit.collider.gameObject.name);

            // Make sure we only destroy objects with the Enemy tag
            if (hit.collider.CompareTag("Enemy"))
            {
                CrosshairChange(true);

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    // Check if the enemy is not the player or parented player
                    GameObject target = hit.collider.gameObject;

                    // If the enemy has the player as a child, detach player first
                    PlayerMovement playerMovement = target.GetComponentInChildren<PlayerMovement>();
                    if (playerMovement != null)
                    {
                        // Release the player if attached
                        playerMovement.transform.SetParent(null);
                        playerMovement.EnableMovement();
                    }

                    // Destroy the enemy
                    Destroy(target);
                    Debug.Log("Enemy destroyed!");
                }

            }
        }
        else
        {
            CrosshairChange(false);
        }

        if (Physics.Raycast(transform.position, forward, out hit, raylength))
        {
            print(hit.collider.gameObject.name);

            if (hit.collider.gameObject.CompareTag("Wood"))
            {
                CrosshairChange(true);

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    GameObject material = hit.collider.gameObject;
                    collectedPlank = true;
                    Destroy(material);
                    Debug.Log("Collected Material");
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
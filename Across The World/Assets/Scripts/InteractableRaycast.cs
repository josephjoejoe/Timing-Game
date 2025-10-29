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

        //Button Raycast
        if (Physics.Raycast(transform.position, forward, out hit, raylength))
        {
            if (hit.collider.CompareTag(interactableTag))
            {
                CrosshairChange(true);
                if (Input.GetKeyDown(killEnemy))
                {
                    Destroy(gameObject);
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
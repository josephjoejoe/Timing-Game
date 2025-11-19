using UnityEngine;
using UnityEngine.Animations.Rigging;   // << REQUIRED for Animation Rigging

public class LedgeGrab : MonoBehaviour
{
    public Transform body;

    public Transform leftHandTarget; // IK target for the hand 
    public Transform rightHandTarget; // IK target for the hand

    public LayerMask ledgeLayer;

    public float grabDistance = 1f;
    public float grabHeight = 1.5f;
    public float handMoveSpeed = 5f;
    public float handLiftHeight = 0.15f;
    public float handSpacing = 0.25f;  // distance between left & right hand

    // -------------------------
    // IK Weight Control
    // -------------------------
    [Header("IK Weight Control")]
    public float ikWeight = 0f;                 // current weight
    public float ikBlendSpeed = 4f;             // speed of blend
    public bool ikActive = false;              // should IK be active?

    // *** NEW: Reference to Animation Rig ***
    [Header("Animation Rigging")]
    public Rig handRig;                          // << Drag your rig here!

    private Vector3 leftOldPos;
    private Vector3 rightOldPos;

    private Vector3 leftGrabPoint;
    private Vector3 rightGrabPoint;

    private float grabLerp = 1f;
    public bool grabbing = false;

    void Update()
    {
        DetectLedge();

        // Smooth IK weight blend
        float targetWeight = ikActive ? 1f : 0f;
        ikWeight = Mathf.MoveTowards(ikWeight, targetWeight, Time.deltaTime * ikBlendSpeed);

        // *** NEW: Apply IK weight to the Animation Rig ***
        if (handRig != null)
            handRig.weight = ikWeight;

        if (grabbing)
        {
            AnimateHandGrab();
        }
    }

    void DetectLedge()
    {
        bool ledgeDetected = false;   // *** NEW: track if we detect any ledge this frame ***

        Ray forwardRay = new Ray(body.position + Vector3.up * grabHeight, body.forward); // cast forward to detect a ledge

        Debug.DrawRay(forwardRay.origin, forwardRay.direction * grabDistance, Color.red);

        if (Physics.Raycast(forwardRay, out RaycastHit wallHit, grabDistance, ledgeLayer))
        {
            Debug.DrawLine(forwardRay.origin, wallHit.point, Color.green);

            Ray downRay = new Ray(wallHit.point + Vector3.up * 0.25f, Vector3.down); // cast downward from the top of the wall

            Debug.DrawRay(downRay.origin, Vector3.down * 1f, Color.yellow);

            if (Physics.Raycast(downRay, out RaycastHit ledgeHit, 1f, ledgeLayer))
            {
                Debug.DrawLine(downRay.origin, ledgeHit.point, Color.blue);

                ledgeDetected = true;   // *** NEW: we detected a ledge ***

                if (grabLerp >= 1f) // start grab if not already grabbing 
                {
                    grabbing = true;
                    grabLerp = 0f;

                    // Turn IK ON
                    ikActive = true;

                    leftOldPos = leftHandTarget.position; // save old hand position
                    rightOldPos = rightHandTarget.position;

                    Vector3 basePoint = ledgeHit.point; // create two seperate grab points

                    leftGrabPoint = basePoint - body.right * handSpacing;
                    rightGrabPoint = basePoint + body.right * handSpacing;
                }
            }
        }

        // ----------------------------------------------------
        // *** NEW: Only turn IK off when the ledge is truly gone ***
        // ----------------------------------------------------
        if (!ledgeDetected)
        {
            grabbing = false;     // stop animation only
            ikActive = false;     // fade IK weight back to 0
            grabLerp = 1f;        // allow future grabs
        }
    }

    void AnimateHandGrab()
    {
        if (grabLerp < 1f)
        {
            Vector3 leftPos = Vector3.Lerp(leftOldPos, leftGrabPoint, grabLerp);
            leftPos.y += Mathf.Sin(grabLerp * Mathf.PI) * handLiftHeight;

            Vector3 rightPos = Vector3.Lerp(rightOldPos, rightGrabPoint, grabLerp);
            rightPos.y += Mathf.Sin(grabLerp * Mathf.PI) * handLiftHeight;

            leftHandTarget.position = leftPos;
            rightHandTarget.position = rightPos;

            grabLerp += Time.deltaTime * handMoveSpeed;
        }
        else
        {
            // lock in place — BUT DO NOT TURN IK OFF
            leftHandTarget.position = leftGrabPoint;
            rightHandTarget.position = rightGrabPoint;

            grabbing = false;  // animation finished, but IK stays active
        }
    }

    // Call this when letting go of the ledge manually
    public void ReleaseLedge()
    {
        ikActive = false;
    }
}
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.GridLayoutGroup;

public class LedgeDetection : MonoBehaviour
{
    public UnityEvent<GameObject> OnGrabLedge = new UnityEvent<GameObject>();

    public HandChecker leftHand;
    public HandChecker rightHand;

    public bool leftHandOff;
    public bool rightHandOff;

    
    public void HandEnter(HandChecker hand, Collider other)
    {
        if (other.gameObject.CompareTag("Ledge"))
        {
            OnGrabLedge.Invoke(other.gameObject);

            if (hand == leftHand)
            {
                leftHandOff = false;
            }

            if (hand == rightHand)
            {
                rightHandOff = false;
            }
        }   
    }

    public void HandExit(HandChecker hand, Collider other)
    {
        if (other.gameObject.CompareTag("Ledge"))
        {
            if (hand == leftHand)
            {
                leftHandOff = true;
            }

            if (hand == rightHand)
            {
                rightHandOff = true;
            }
        }
    }    
}

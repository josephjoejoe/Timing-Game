using UnityEngine;

public class HandChecker : MonoBehaviour
{
    public LedgeDetection ledgeDetection;

    void OnTriggerEnter(Collider other)
    {
        ledgeDetection.HandEnter(this, other);
    }

    void OnTriggerExit(Collider other)
    {
        ledgeDetection.HandExit(this, other);
    }
}

using UnityEngine;

public class BaseState : MonoBehaviour
{
    public PlayerMovementStateMachine owner;
    public bool isActive = false;
    private void Start() {
        owner = GetComponent<PlayerMovementStateMachine>();
    }
    public void EnterState() { }
}

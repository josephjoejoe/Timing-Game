using UnityEngine;

public abstract class BaseState : MonoBehaviour
{
    public PlayerMovementStateMachine owner;
    public bool isActive = false;
    private void Start() {
        owner = GetComponent<PlayerMovementStateMachine>();
    }
    public abstract void EnterState();

    public abstract void ExitState();

}

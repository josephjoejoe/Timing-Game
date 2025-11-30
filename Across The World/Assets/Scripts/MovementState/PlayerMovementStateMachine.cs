using UnityEngine;

public class PlayerMovementStateMachine : MonoBehaviour
{
    public PlayerMovement walkState;
    public NewLedgeGrab ledgeState;
    BaseState currentState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        walkState = GetComponent<PlayerMovement>();
        ledgeState = GetComponent<NewLedgeGrab>();
        currentState = walkState;
        SwapState(walkState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwapState(BaseState newState) 
    {
        currentState.isActive = false;
        currentState = newState;
        currentState.isActive = true;
        currentState.EnterState();
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> State = new Dictionary<EState, BaseState<EState>>();
    public BaseState<EState> CurrentState;

    protected bool IsTransitioningState = false;

    void Start()
    {
        CurrentState.EnterState(); //EnterState is like void start they both get called one time
    }

    void Update() // this will be called once per frame
    {
        EState nextStateKey = CurrentState.GetNextState();

        if (!IsTransitioningState && nextStateKey.Equals(CurrentState.Statekey))
        {
            CurrentState.UpdateState();
        }
        else if(!IsTransitioningState)
        {
            TransitionToState(CurrentState.Statekey);
        }
    }

    public void TransitionToState(EState stateKey)
    {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = State[stateKey];
        CurrentState.EnterState();
        IsTransitioningState = false;
    }

    void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        CurrentState.OnTriggerStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        CurrentState.OnTriggerExit(other);
    }

}

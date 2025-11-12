using UnityEngine;

public abstract class EnvironmentInteractionState : BaseState<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    // this was added because of an error with the class dealing with key i think
    protected EnvironmentInteractionState(EnvironmentInteractionStateMachine.EEnvironmentInteractionState key) : base(key)
    {
    }
}

using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

public class EnvironmentInteractionStateMachine : StateManager<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    public enum EEnvironmentInteractionState
    {
        Search,
        Approach,
        Rise,
        Touch,
        Reset,
    }

    [SerializeField] private TwoBoneIKConstraint _leftIkConstraint;
    [SerializeField] private TwoBoneIKConstraint _rightIkConstraint;
    [SerializeField] private MultiRotationConstraint _leftMultiRotationConstraint;
    [SerializeField] private MultiRotationConstraint _rightMultiRotationConstraint;
    //[SerializeField] private Rigidbody _rigidbody;
    //[SerializeField] private CapsuleCollider _rootCollider;
    [SerializeField] private PlayerController _playerController; // if this doent work take this line out and ungreen the two lines before it 

    void Awake()
    {
        ValidateConstraints();
    }

    private void ValidateConstraints()
    {
        Assert.IsNotNull(_leftIkConstraint, "Left IK constraint is not assigned");
        Assert.IsNotNull(_rightIkConstraint, "Right IK constraint is not assigned");
        Assert.IsNotNull(_leftMultiRotationConstraint, "Left Multi-Rotation constraint is not assigned");
        Assert.IsNotNull(_rightMultiRotationConstraint, "Right Multi-Rotation constraint is not assigned");
        Assert.IsNotNull(_playerController, "PlayerController not assigned");// if this doesnt work take this line out and ungreen the bottom two lines
        //Assert.IsNotNull(_rigidbody, "Rigidbody used to controle character is not assigned");
        //Assert.IsNotNull(_rootCollider, "RootCollider attached to character is not assigned");

    }
}

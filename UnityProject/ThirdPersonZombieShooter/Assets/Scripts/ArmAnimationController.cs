using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ArmAnimationController : MonoBehaviour
{
    [SerializeField]
    Equipable _equipable;

    [SerializeField, Tooltip("The position the right hand attempts to reach.")]
    Transform _rightHandIKTarget;

    [SerializeField, Tooltip("The position the left hand attempts to reach.")]
    Transform _leftHandIKTarget;

    [SerializeField]
    TwoBoneIKConstraint _rightHandConstraint;

    [SerializeField]
    TwoBoneIKConstraint _leftHandConstraint;

    private void Update()
    {
        // Update the positions and rotations of the hand IK targets if an equipable is set.
        if (_equipable == null)
            return;

        // - Right arm.
        _rightHandIKTarget.position = _equipable.RightHandIKAnchor.position;
        _rightHandIKTarget.rotation = _equipable.RightHandIKAnchor.rotation;

        // - Left arm.
        _leftHandIKTarget.position = _equipable.LeftHandIKAnchor.position;
        _leftHandIKTarget.rotation = _equipable.LeftHandIKAnchor.rotation;
    }

    public void SetIKWeight(float weight)
    {
        _rightHandConstraint.weight = weight;
        _leftHandConstraint.weight = weight;
    }
}

using UnityEngine;

public class Equipable : MonoBehaviour
{
    [SerializeField, Tooltip("Where the right hand should attach to.")]
    Transform _rightHandIKAnchor;

    [SerializeField, Tooltip("Where the left hand should attach to.")]
    Transform _leftHandIKAnchor;

    public Transform RightHandIKAnchor
    {
        get { return _rightHandIKAnchor; }
        private set {
            _rightHandIKAnchor = value;
        }
    }

    public Transform LeftHandIKAnchor
    {
        get { return _leftHandIKAnchor; }
        private set
        {
            _leftHandIKAnchor = value;
        }
    }
}

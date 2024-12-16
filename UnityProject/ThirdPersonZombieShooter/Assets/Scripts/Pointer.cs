using UnityEngine;

public class Pointer : MonoBehaviour
{
    [SerializeField]
    LayerMask _targets;

    public Vector3 Target
    {
        get {
            if (!_targetIsDirty)
                return _target;

            // Raycast in the direction of the gun.
            if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, Mathf.Infinity, _targets))
                _target = hitInfo.point;
            else
                _target = transform.position + transform.forward * 1000;

            _targetIsDirty = false;

            return _target;
        }
        private set { _target = value; }
    }

    Vector3 _target;

    bool _targetIsDirty;

    void Update()
    {
        _targetIsDirty = true;
    }
}

using UnityEngine;

public class TestPointer : MonoBehaviour
{
    [SerializeField]
    Pointer _pointer;

    [SerializeField]
    Transform _targetMarker;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            _targetMarker.position = _pointer.Target;
    }
}

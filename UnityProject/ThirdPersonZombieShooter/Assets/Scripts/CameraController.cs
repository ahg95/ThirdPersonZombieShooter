using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Camera _camera;

    [SerializeField]
    Transform _cameraPivot;

    [Header("Configurations")]
    [SerializeField]
    Transform _runCameraPosition;

    [SerializeField]
    Transform _aimCameraPosition;

    [SerializeField]
    float _runMouseSensititvity;

    [SerializeField]
    float _aimWalkMouseSensitivity;

    [SerializeField]
    float _aimFOV;

    float _runFOV;

    [SerializeField]
    float _animationDuration;

    float _aimAnimationProgress;

    float _localXRotation;
    float _localYRotation;

    PlayerInput _input;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Initialize input.
        _input = new();
        _input.Gameplay.Aim.Enable();
        _input.Gameplay.Look.Enable();

        // Initialize "default" field of view.
        _runFOV = _camera.fieldOfView;

        // Store initial rotations.
        _localXRotation = _cameraPivot.localRotation.x;
        _localYRotation = _cameraPivot.localRotation.y;
    }

    void Update()
    {
        var isAiming = _input.Gameplay.Aim.ReadValue<float>() != 0;



        // Look around.
        var mouseSensitivity = isAiming ? _aimWalkMouseSensitivity : _runMouseSensititvity;
        var lookDelta = _input.Gameplay.Look.ReadValue<Vector2>() * Time.deltaTime * mouseSensitivity;

        _localXRotation = Mathf.Clamp(_localXRotation - lookDelta.y, -90, 90);
        _localYRotation += lookDelta.x;
        _cameraPivot.localRotation = Quaternion.Euler(_localXRotation, _localYRotation, 0);


        var progressDelta = Time.deltaTime / _animationDuration;
        if (!isAiming)
            progressDelta = -progressDelta;

        _aimAnimationProgress = Mathf.Clamp01(_aimAnimationProgress + progressDelta);

        var t = EasingFunction.EaseBezier(_aimAnimationProgress);

        _camera.transform.position = Vector3.Lerp(_runCameraPosition.position, _aimCameraPosition.position, t);
        _camera.fieldOfView = Mathf.Lerp(_runFOV, _aimFOV, t);
    }
}

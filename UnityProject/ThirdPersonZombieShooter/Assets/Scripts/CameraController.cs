using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Camera _camera;

    [SerializeField]
    Transform _cameraPivot;

    [Header("Configurations")]
    [SerializeField]
    Transform _aimCameraPosition;

    Transform _runCameraPosition;

    [SerializeField]
    float _runMouseSensititvity;

    [SerializeField]
    float _aimMouseSensitivity;

    [SerializeField]
    float _aimFOV;

    float _runFOV;

    [SerializeField, Tooltip("How long switching the camera from aim to run takes.")]
    float _transitionAnimationDuration;

    float _transitionAnimationProgress;

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

        // Initialize "default" camera position.
        var runCameraPositionGO = new GameObject("RunCameraPosition");
        _runCameraPosition = runCameraPositionGO.transform;
        _runCameraPosition.position = _camera.transform.position;
        _runCameraPosition.rotation = _camera.transform.rotation;
        _runCameraPosition.parent = _aimCameraPosition.parent;

        // Store initial rotations.
        _localXRotation = _cameraPivot.localRotation.x;
        _localYRotation = _cameraPivot.localRotation.y;
    }

    void Update()
    {
        var isAiming = _input.Gameplay.Aim.ReadValue<float>() != 0;



        // Rotate the camera pivot according to the mouse delta.
        var mouseSensitivity = isAiming ? _aimMouseSensitivity : _runMouseSensititvity;
        var lookDelta = _input.Gameplay.Look.ReadValue<Vector2>() * Time.deltaTime * mouseSensitivity;
        _localXRotation = Mathf.Clamp(_localXRotation - lookDelta.y, -90, 90);
        _localYRotation += lookDelta.x;
        _cameraPivot.localRotation = Quaternion.Euler(_localXRotation, _localYRotation, 0);



        // Animate the camera between aiming and running.
        // - Update the animation progress.
        var progressDelta = Time.deltaTime / _transitionAnimationDuration;
        if (!isAiming)
            progressDelta = -progressDelta;
        _transitionAnimationProgress = Mathf.Clamp01(_transitionAnimationProgress + progressDelta);

        // - Apply smoothing to the animation.
        var t = EasingFunction.EaseBezier(_transitionAnimationProgress);

        // - Apply the animation to the camera position and field of view.
        _camera.transform.position = Vector3.Lerp(_runCameraPosition.position, _aimCameraPosition.position, t);
        _camera.fieldOfView = Mathf.Lerp(_runFOV, _aimFOV, t);
    }

    private void OnDestroy()
    {
        Destroy(_runCameraPosition.gameObject);
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    MovementController _movementController;

    [SerializeField]
    float _mouseSensititvity;

    [SerializeField]
    Transform _cameraTransform;

    [SerializeField]
    Transform _bodyTransform;

    float _localXRotation;
    float _localYRotation;

    PlayerInput _playerInput;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _localXRotation = transform.localRotation.x;
        _localYRotation = transform.localRotation.y;

        // Initialize and enable player input.
        _playerInput = new PlayerInput();
        _playerInput.Gameplay.Enable();
    }

    void Update()
    {
        // Look around.
        var lookDelta = _playerInput.Gameplay.Look.ReadValue<Vector2>() * Time.deltaTime * _mouseSensititvity;

        _localXRotation = Mathf.Clamp(_localXRotation - lookDelta.y, -90, 90);
        _localYRotation += lookDelta.x;
        _cameraTransform.localRotation = Quaternion.Euler(_localXRotation, _localYRotation, 0);



        // Move.
        var movementInput = _playerInput.Gameplay.Movement.ReadValue<Vector2>();

        movementInput = Rotate(movementInput, - _localYRotation * Mathf.Deg2Rad);

        _movementController._MovementInput = movementInput;
    }

    static Vector2 Rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }
}
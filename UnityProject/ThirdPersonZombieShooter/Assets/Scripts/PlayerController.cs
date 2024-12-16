using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    MovementController _movementController;

    [SerializeField]
    Transform _cameraPivot;

    [Header("Configurations")]
    [SerializeField]
    float _runTopSpeed;

    [SerializeField]
    float _aimWalkTopSpeed;

    PlayerInput _input;

    private void Start()
    {
        // Initialize and enable player input.
        _input = new PlayerInput();
        _input.Gameplay.Movement.Enable();
        _input.Gameplay.Aim.Enable();
    }

    void LateUpdate()
    {
        // Adjust top speed depending on if the player is aiming or not.
        var isAiming = _input.Gameplay.Aim.ReadValue<float>() != 0;
        _movementController._CurrentTopSpeed = isAiming ? _aimWalkTopSpeed : _runTopSpeed;



        // Set the movement input for the movement controller.
        var movementInput = _input.Gameplay.Movement.ReadValue<Vector2>();
        // - Rotate the movement input based on the camera angle.
        movementInput = Rotate(movementInput, -_cameraPivot.eulerAngles.y * Mathf.Deg2Rad);
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
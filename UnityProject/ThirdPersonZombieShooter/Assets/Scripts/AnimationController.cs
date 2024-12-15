using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    Rigidbody _rigidbody;

    [SerializeField]
    MovementController _movementController;

    [SerializeField]
    Transform _camera;

    [Header("Properties")]
    [SerializeField]
    float _runTurningSpeed;

    [SerializeField]
    float _runAnimationSpeed;

    [SerializeField]
    float _aimWalkTurningSpeed;

    [SerializeField]
    float _aimWalkAnimationSpeed;

    [SerializeField]
    float _aimAnimationSmoothingFactor;

    Animator _animatorController;

    float _angularVelocity = 0;

    PlayerInput _input;

    float _forwardSpeed;
    float _sidewaysSpeed;

    float _forwardVelocity;
    float _sidewaysVelocity;

    private void Awake()
    {
        _animatorController = GetComponent<Animator>();

        _input = new();
        _input.Gameplay.Aim.Enable();
    }

    void Update()
    {
        bool isAiming = _input.Gameplay.Aim.ReadValue<float>() != 0;

        // Change the animator's animation speed based on the movement controller's top speed.
        // This prevents foot slipping when the top speed is changed.
        var animationSpeed = isAiming ? _aimWalkAnimationSpeed : _runAnimationSpeed;

        _animatorController.speed = _movementController._CurrentTopSpeed / animationSpeed;



        // Set the upper body layer weight depending on if the player is aiming.
        // The second layer is responsible for setting the up and down angle of aiming.
        _animatorController.SetLayerWeight(1, isAiming ? 1f : 0f);

        var angle = _camera.rotation.eulerAngles.x;
        if (angle > 90)
            angle -= 360;

        var aimHeight = -angle / 60f;

        // Set the aim angle if the player is aiming.
        _animatorController.SetFloat("AimHeight", aimHeight);






        var currVelFlattened = _rigidbody.linearVelocity;
        currVelFlattened.y = 0;

        // Rotate the player model.
        // If the player is not aiming then rotate the model towards the movement direction.
        // Otherwise, rotate the model towards the aiming direction.

        var currentAngle = transform.eulerAngles.y;

        var turningSpeed = isAiming ? _aimWalkTurningSpeed : _runTurningSpeed;

        // - Calculate target angle.

        if (isAiming)
        {
            var cameraForwardFlattened = _camera.forward;

            cameraForwardFlattened.y = 0;

            var targetAngle = Vector3.SignedAngle(Vector3.back, cameraForwardFlattened, Vector3.up) + 180;

            var nextAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref _angularVelocity, 1 / turningSpeed);

            transform.eulerAngles = new Vector3(0, nextAngle, 0);
        }
        else
        {
            var targetAngle = Vector3.SignedAngle(Vector3.back, currVelFlattened, Vector3.up) + 180;

            if (currVelFlattened.magnitude > 0.1f)
            {
                var nextAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref _angularVelocity, 1 / turningSpeed);

                transform.eulerAngles = new Vector3(0, nextAngle, 0);
            }
        }



        // Set the animation parameters.
        // - Set the parameter for if the player is aiming.
        _animatorController.SetBool("IsAiming", isAiming);

        // - Calculate the movement speed in the forward direction.
        var forwardSpeed = currVelFlattened.magnitude * Mathf.Cos(Vector3.Angle(transform.forward, currVelFlattened) * Mathf.Deg2Rad);

        _forwardSpeed = Mathf.SmoothDamp(_forwardSpeed, forwardSpeed, ref _forwardVelocity, _aimAnimationSmoothingFactor);
        if (isAiming)
            forwardSpeed = _forwardSpeed;

        _animatorController.SetFloat("ForwardSpeed", forwardSpeed / _movementController._CurrentTopSpeed);

        // - Calculate the movement speed in the right direction.
        var sidewaysSpeed = currVelFlattened.magnitude * Mathf.Cos(Vector3.Angle(transform.right, currVelFlattened) * Mathf.Deg2Rad);

        // - Smooth out the animation parameter if the player is aiming. Otherwise, the animations look too jerky.
        _sidewaysSpeed = Mathf.SmoothDamp(_sidewaysSpeed, sidewaysSpeed, ref _sidewaysVelocity, _aimAnimationSmoothingFactor);
        if (isAiming)
            sidewaysSpeed = _sidewaysSpeed;

        _animatorController.SetFloat("SidewaysSpeed", sidewaysSpeed / _movementController._CurrentTopSpeed);
    }
}

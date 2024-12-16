using UnityEngine;

public class MovementAnimationController : MonoBehaviour
{
    [SerializeField]
    Animator _animator;

    [SerializeField]
    Rigidbody _rigidbody;

    [SerializeField]
    MovementController _movementController;

    [SerializeField]
    Transform _cameraPivot;

    [SerializeField]
    Pointer _cameraPointer;

    [SerializeField]
    Transform _characterModel;

    [SerializeField, Tooltip("How much distance the walking animation covers per second.")]
    float _walkAnimationSpeed;

    [SerializeField, Tooltip("How much distance the running animation covers per second.")]
    float _runAnimationSpeed;

    [SerializeField, Tooltip("How quickly the character model is turned towards the aimed at position.")]
    float _walkTurningSpeed;

    [SerializeField, Tooltip("How quickly the character model is turned towards the movement direction.")]
    float _runTurningSpeed;

    [SerializeField]
    float _aimAnimationSmoothingFactor;

    PlayerInput _input;

    float _forwardSpeed;
    float _sidewaysSpeed;

    float _forwardVelocity;
    float _sidewaysVelocity;

    float _angularVelocity;

    // The angle at which the character aims down or up in the corresponding poses.
    const float VERTICALAIMANGLE = 60;

    void Start()
    {
        // Initialize and enable input.
        _input = new();
        _input.Gameplay.Aim.Enable();
    }

    void Update()
    {
        // Set animation parameter for if player is aiming or not.
        bool isAiming = _input.Gameplay.Aim.ReadValue<float>() != 0;
        _animator.SetBool("IsAiming", isAiming);



        // Change the animator's animation speed based on the movement controller's top speed.
        // This prevents foot slipping when the top speed is changed.
        var animationSpeed = isAiming ? _walkAnimationSpeed : _runAnimationSpeed;
        _animator.speed = _movementController._CurrentTopSpeed / animationSpeed;



        // Enable up- and down aiming of upper body if player is aiming.
        if (isAiming)
        {
            // Enable the animation layer responsible for the up- and down aim animation.
            _animator.SetLayerWeight(1, 1f);

            // Calculate signed vertical angle of camera with horizon.
            var angle = _cameraPivot.rotation.eulerAngles.x;
            if (angle > 90)
                angle -= 360;

            // Set animator parameter.
            _animator.SetFloat("AimHeight", -angle / VERTICALAIMANGLE);
        }
        else
        {
            // Disable the animation layer responsible for the up- and down aim animation.
            _animator.SetLayerWeight(1, 0f);
        }



        var currVelFlattened = _rigidbody.linearVelocity;
        currVelFlattened.y = 0;



        // Rotate the character model.
        var currentAngle = _characterModel.eulerAngles.y;
        var turningSpeed = isAiming ? _walkTurningSpeed : _runTurningSpeed;
        if (isAiming)
        {
            // Rotate the character model towards the point the player is looking at.
            // - Calculate the angle towards the looked-at point.
            var targetDirection = _cameraPointer.Target - _characterModel.position;
            targetDirection.y = 0;

            var targetAngle = Vector3.SignedAngle(Vector3.back, targetDirection, Vector3.up) + 180;

            // - Smoothly transition to the target angle.
            var nextAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref _angularVelocity, 1 / turningSpeed);

            // - Set the rotation of the character model.
            _characterModel.eulerAngles = new Vector3(0, nextAngle, 0);
        }
        else
        {
            // Rotate the character model towards the movement direction if the player is moving.
            // - Calculate the angle in the movement 
            var targetAngle = Vector3.SignedAngle(Vector3.back, currVelFlattened, Vector3.up) + 180;

            // - Check if the player is moving.
            if (currVelFlattened.magnitude > 0.1f)
            {
                // Smoothly transition to the target angle.
                var nextAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref _angularVelocity, 1 / turningSpeed);

                // Rotate the character model.
                _characterModel.eulerAngles = new Vector3(0, nextAngle, 0);
            }
        }



        // Set the forward speed parameter.
        // - Calculate the movement speed in the forward direction.
        var forwardSpeed = currVelFlattened.magnitude * Mathf.Cos(Vector3.Angle(_characterModel.forward, currVelFlattened) * Mathf.Deg2Rad);
        
        // - Calculate a smoothed-out parameter to be used when aiming. Otherwise the animations look too jerky.
        _forwardSpeed = Mathf.SmoothDamp(_forwardSpeed, forwardSpeed, ref _forwardVelocity, _aimAnimationSmoothingFactor);
        if (isAiming)
            forwardSpeed = _forwardSpeed;

        _animator.SetFloat("ForwardSpeed", forwardSpeed / _movementController._CurrentTopSpeed);



        // Set the sideways speed parameter.
        // - Calculate the movement speed in the right direction.
        var sidewaysSpeed = currVelFlattened.magnitude * Mathf.Cos(Vector3.Angle(_characterModel.right, currVelFlattened) * Mathf.Deg2Rad);

        // - Calculate a smoothed-out parameter to be used when aiming. Otherwise the animations look too jerky.
        _sidewaysSpeed = Mathf.SmoothDamp(_sidewaysSpeed, sidewaysSpeed, ref _sidewaysVelocity, _aimAnimationSmoothingFactor);
        if (isAiming)
            sidewaysSpeed = _sidewaysSpeed;

        _animator.SetFloat("SidewaysSpeed", sidewaysSpeed / _movementController._CurrentTopSpeed);
    }
}

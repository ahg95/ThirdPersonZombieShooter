using System;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [HideInInspector]
    public Vector2 _MovementInput;

    [Header("General")]
    public float _TopSpeed;

    [SerializeField]
    LayerMask _groundLayer;

    [SerializeField]
    LayerMask _obstaclesLayer;

    [Header("Acceleration")]
    [Tooltip("The time it takes the player from completely standing still to reaching top speed.")]
    public float _TimeToReachTopSpeed;

    [SerializeField, Tooltip("The curve that the player's speed follows when accelerating.")]
    EasingFunctionID _accelerationProfile;

    public EasingFunctionID AccelerationProfile
    {
        get { return _accelerationProfile; }
        set
        {
            _accelerationProfile = value;
            UpdateAccelerationFunction();
        }
    }

    [Header("Deceleration")]
    [Tooltip("The time it takes the player from moving at top speed to completely standing still.")]
    public float _TimeToStop;

    [SerializeField, Tooltip("The curve that the player's speed follows when decelerating.")]
    EasingFunctionID _decelerationProfile;

    public EasingFunctionID DecelerationProfile
    {
        get { return _decelerationProfile; }
        set
        {
            _decelerationProfile = value;
            UpdateDecelerationFunction();
        }
    }

    [Header("Inertia")]
    [SerializeField, Tooltip("How much the controller will try to maintain the current velocity.")]
    float _inertiaFactor;

    public float InertiaFactor
    {
        get { return _inertiaFactor; }
        set
        {
            _inertiaFactor = value;
            UpdateMaximumVelocityChange();
        }
    }

    float _maximumVelocityChange;

    Vector2 _previousVelocity2D;

    [Header("Obstacle Avoidance")]
    [Tooltip("Enables obstacle avoidance.")]
    public bool _ObstacleAvoidanceEnabled;
    
    [Range(0, 90), Tooltip("The controller will only avoid an obstacle if the angle between its surface normal and the input direction is at least this high.")]
    public float _MinimumSurfaceAngle;

    [Tooltip("The highest angle that the controller is allowed to steer the player away from their intended movement direction.")]
    public float _AllowedSteeringAngle;

    [Tooltip("The controller may steer the player around obstacles by this additional angle. Higher values improve performance.")]
    public float _AllowedAngleError;

    [Tooltip("The maximum distance at which the controller will avoid an obstacle.")]
    public float _DetectionDistance;

    Rigidbody _rigidbody;

    CapsuleCollider _collider;


    // The following functions define the acceleration and deceleration of the player.
    // The x axis of these functions describe the time, and the y axis describe the speed of the player.
    [SerializeField, HideInInspector]
    Func<float, float> _accelerationFunction;

    [SerializeField, HideInInspector]
    Func<float, float> _inverseAccelerationFunction;

    [SerializeField, HideInInspector]
    Func<float, float> _decelerationFunction;

    [SerializeField, HideInInspector]
    Func<float, float> _inverseDecelerationFunction;

    [SerializeField, HideInInspector]
    Func<float, float> _integralDecelerationFunction;

    private void OnValidate()
    {
        UpdateAccelerationFunction();
        UpdateDecelerationFunction();

        UpdateMaximumVelocityChange();
    }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        UpdateAccelerationFunction();
        UpdateDecelerationFunction();

        UpdateMaximumVelocityChange();
    }

    private void FixedUpdate()
    {
        // Calculate the next velocity of the player.
        var movementInput = _MovementInput;

        var newVelocity = CalculateNextVelocity(_previousVelocity2D, _rigidbody.linearVelocity, movementInput);

        if (_ObstacleAvoidanceEnabled)
        {
            if (CalculateMovementDirectionAvoidingObstacles(ref movementInput, newVelocity))
            {
                newVelocity = CalculateNextVelocity(_previousVelocity2D, _rigidbody.linearVelocity, movementInput);
            }
        }

        // Update the previous velocity.
        _previousVelocity2D = new Vector2(newVelocity.x, newVelocity.z);

        // Apply the new velocity.
        _rigidbody.linearVelocity = newVelocity;

        // Snap the player model to the ground.
        SnapToGround();
    }

    bool CalculateMovementDirectionAvoidingObstacles(ref Vector2 movementInput, Vector3 currentVelocity)
    {
        // Check if the player would run against an obstacle.

        // - Perform a capsule cast in the movement direction of the player.
        // - - Calculate values that define the collider's capsule shape for the raycast.
        var capsulePoint1 = _collider.center + transform.position + Vector3.up * _collider.height / 2;
        var capsulePoint2 = _collider.center + transform.position + Vector3.down * _collider.height / 2;

        // - - The radius of the capsule for the raycast is chosen smaller than the collider's radius to allow the raycast to hit obstacles that touch the collider.
        var colliderRadius = _collider.radius * 0.99f;

        // - Calculate the distance for the raycast. The raycast should only check for obstacles that the player would definitely collide with plus the provided detection distance.
        // - The detection distance should be greater 0 because the calculated stopping position deviates somewhat from the actual position where the player stops.
        var stoppingPosition = CalculateStoppingPosition(transform.position, currentVelocity);
        var raycastDistance = (transform.position - stoppingPosition).magnitude + _DetectionDistance;

        var raycastDirection = currentVelocity;

        // If there is no obstacle in the way then don't adjust the movement input.
        if (!Physics.CapsuleCast(capsulePoint1, capsulePoint2, colliderRadius, raycastDirection, out var raycastHit, raycastDistance, _obstaclesLayer))
            return false;

        // There is an obstacle in the path of the player.
        // Check if the player intentionally moves towards the obstacle by checking the angle between the input and the hit normal.
        var normalInverted2D = new Vector2(-raycastHit.normal.x, -raycastHit.normal.z);

        var angleToWall = Vector2.SignedAngle(movementInput, normalInverted2D);

        if (Mathf.Abs(angleToWall) <= _MinimumSurfaceAngle)
            return false;

        // The player probably does not want to hit the obstacle, so engage the obstacle avoidance.

        // - Capsule cast around the obstacle until there is no obstacle in the way
        // or until avoiding the obstacle means deviating too much from the player's intended movement direction.
        var deviationAngle = 0f;

        var hitPoint = raycastHit.point;

        while (true)
        {
            // Check if avoiding the obstacle means deviating too much from the player's intended movement direction.
            // - Calculate the angle by which the velocity must be rotated to avoid the obstacle.
            var hitpointDelta = transform.position - hitPoint;
            hitpointDelta.y = 0;
            var hitpointDistance = hitpointDelta.magnitude;

            var angleIncrease = Mathf.Abs(Mathf.Atan(colliderRadius / hitpointDistance) + _AllowedAngleError);

            // - Calculate the total angle of deviation.
            deviationAngle += angleIncrease;

            // - If the total angle of deviation is too large then we abort the obstacle avoidance. The player will move in the intended direction against the obstacle.
            if (deviationAngle > _AllowedSteeringAngle)
                break;

            // The new angle does not deviate too much from the player's intended movement direction.
            // Check if there is an obstacle in the new direction.

            // - Calculate a new direction based on the angle increase.
            raycastDirection = Quaternion.AngleAxis(Mathf.Sign(angleToWall) * angleIncrease, Vector3.up) * raycastDirection;

            // - Calculate the new raycast distance
            raycastDistance = hitpointDistance;

            if (!Physics.CapsuleCast(capsulePoint1, capsulePoint2, colliderRadius, raycastDirection, out var hit, raycastDistance, _obstaclesLayer))
            {
                raycastDirection = raycastDirection.normalized;

                movementInput.x = raycastDirection.x;
                movementInput.y = raycastDirection.z;

                return true;
            }

            // Update the hit point that should be avoided.
            hitPoint = hit.point;
        }

        return false;
    }


    void SnapToGround()
    {
        // Position the player on top of the ground.
        if (Physics.Raycast(transform.position, Vector3.down, out var groundHit, Mathf.Infinity, _groundLayer))
            transform.position = groundHit.point + Vector3.up * _collider.height / 2 - _collider.center;
    }


    Vector3 CalculateNextVelocity(Vector2 previousVelocity2D, Vector3 currentVelocity, Vector2 movementInput)
    {
        // Calculate the target velocity as a two-dimensional vector.
        var targetVelocity2D = movementInput * _TopSpeed;

        // The current velocity will attempt to transition to the target velocity in a straight line.
        // Note that the damping that is applied later can create a curve in the transition.
        var currentVelocity2D = new Vector2(currentVelocity.x, currentVelocity.z);
        var deltaVelocityDirection = (targetVelocity2D - currentVelocity2D).normalized;

        // The point on the line between current and target velocity that is closest to the origin is where the controller switches from decelerating to accelerating.
        var turningPoint = Vector2Utility.CalculateLinePointClosestToOrigin(currentVelocity2D, deltaVelocityDirection);
        var currentDistanceToTurningPoint = (turningPoint - currentVelocity2D).magnitude;

        var deltaTurningPoint = turningPoint - currentVelocity2D;

        Vector2 newVelocity2D;

        // If the turning point is located in front of the target velocity then the controller must decelerate. Otherwise, it must accelerate.
        if (Vector2.Dot(deltaVelocityDirection, deltaTurningPoint) > 0)
        {
            // The turning point is located in front of the target velocity, so the controller must decelerate.
            // The current speed in the relevant direction is described by the distance to the turning point.
            var x = InverseDecelerationCurve(currentDistanceToTurningPoint);

            x -= Time.fixedDeltaTime;

            // If x < 0 then the start of the deceleration curve has been reached.
            // This means that the controller will reach the turning point and must accelerate for the remaining time.
            if (x < 0)
            {
                x = -x;

                var newSpeed = AccelerationCurve(x);

                newVelocity2D = Vector2.MoveTowards(turningPoint, targetVelocity2D, newSpeed);

            }
            else
            {
                // The new velocity will stay behind the turning point.
                var newSpeed = DecelerationCurve(x);

                newVelocity2D = Vector2.MoveTowards(turningPoint, currentVelocity2D, newSpeed);
            }
        }
        else
        {
            // The controller must accelerate.

            var x = InverseAccelerationCurve(currentDistanceToTurningPoint);

            x += Time.fixedDeltaTime;

            var newSpeed = AccelerationCurve(x);

            newVelocity2D = Vector2.MoveTowards(turningPoint, targetVelocity2D, newSpeed);
        }

        // Apply inertia to the new velocity.
        newVelocity2D = Vector2.MoveTowards(previousVelocity2D, newVelocity2D, _maximumVelocityChange);

        var newVelocity = new Vector3(newVelocity2D.x, 0, newVelocity2D.y);

        return newVelocity;
    }

    Vector3 CalculateStoppingPosition(Vector3 position, Vector3 velocity)
    {
        // Initialize the result variable.
        velocity.y = 0;
        var currentSpeed = velocity.magnitude;

        // Calculate where the currentVelocity is on the deceleration curve.
        var x = InverseDecelerationCurve(currentSpeed);

        // Calculate the distance traveled when stopping now.
        var distance = IntegralDecelerationCurve(x);

        var stoppingPosition = position + velocity.normalized * distance;

        return stoppingPosition;
    }

    void UpdateAccelerationFunction()
    {
        _accelerationFunction = EasingFunction.GetFunction(_accelerationProfile);
        _inverseAccelerationFunction = EasingFunction.GetInverseFunction(_accelerationProfile);
    }

    void UpdateDecelerationFunction()
    {
        _decelerationFunction = EasingFunction.GetFunction(_decelerationProfile);
        _inverseDecelerationFunction = EasingFunction.GetFunction(_decelerationProfile);
        _integralDecelerationFunction = EasingFunction.GetIntegralFunction(_decelerationProfile);
    }

    void UpdateMaximumVelocityChange()
    {
        _maximumVelocityChange = _inertiaFactor == 0 ? Mathf.Infinity : 1 / _inertiaFactor * Time.fixedDeltaTime;
    }


    // The following functions perform necessary transformations on the easing functions. These include:
    // - Scaling the functions on the y axis by max Speed
    // - Scaling the functions on the x axis by the acceleration- and deceleration durations respectively
    // - Adding return statements for input values out of the domain of the functions

    float AccelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _TimeToReachTopSpeed)
            return _TopSpeed;

        return _TopSpeed * _accelerationFunction(x / _TimeToReachTopSpeed);
    }

    float InverseAccelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _TopSpeed)
            return _TimeToReachTopSpeed;

        return _TimeToReachTopSpeed * _inverseAccelerationFunction(x / _TopSpeed);
    }

    float DecelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _TimeToStop)
            return _TopSpeed;

        return _TopSpeed * _decelerationFunction(x / _TimeToStop);
    }

    float InverseDecelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _TopSpeed)
            return _TimeToStop;

        return _TimeToStop * _inverseDecelerationFunction(x / _TopSpeed);
    }

    float IntegralDecelerationCurve(float x)
    {
        if (x <= 0)
            return 0;

        if (x >= _TimeToStop)
            return _TopSpeed * _TimeToStop;

        return _TopSpeed * _TimeToStop * _integralDecelerationFunction(x / _TimeToStop);
    } 
}
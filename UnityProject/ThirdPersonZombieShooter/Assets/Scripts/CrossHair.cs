using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [SerializeField]
    CanvasGroup _crossHair;

    [SerializeField]
    Transform _top, _right, _bottom, _left;

    [SerializeField]
    float _unsteadyAnimationDuration;

    [SerializeField]
    float _displacement;

    float _unsteadyAnimationProgress;

    [SerializeField]
    float _appearAnimationDuration;

    float _appearAnimationProgress;

    PlayerInput _input;

    Vector2 _topPos, _rightPos, _bottomPos, _leftPos;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize and enable input.
        _input = new();
        _input.Gameplay.Aim.Enable();
        _input.Gameplay.Shoot.Enable();

        _appearAnimationProgress = 0;

        // Initialize positions.
        _topPos = _top.position;
        _rightPos = _right.position;
        _bottomPos = _bottom.position;
        _leftPos = _left.position;

        // Add callback for when the player shoots.
        _input.Gameplay.Shoot.performed += (cc) =>
        {
            _unsteadyAnimationProgress = Mathf.Clamp01(_unsteadyAnimationProgress + 0.5f);
        };
    }

    void Update()
    {
        // Set the crosshair to appear or disappear depending on if the player is aiming.
        var appear = _input.Gameplay.Aim.ReadValue<float>() != 0;



        // Play appearance animation.
        var appearAnimationDelta = Time.deltaTime / _appearAnimationDuration;

        if (!appear)
            appearAnimationDelta = -appearAnimationDelta;

        _appearAnimationProgress = Mathf.Clamp01(_appearAnimationProgress + appearAnimationDelta);

        _crossHair.alpha = _appearAnimationProgress;



        // Move crosshair arms.
        var unsteadyAnimationDelta = Time.deltaTime / _unsteadyAnimationDuration;
        if (appear)
            unsteadyAnimationDelta = - unsteadyAnimationDelta;

        _unsteadyAnimationProgress = Mathf.Clamp01(_unsteadyAnimationProgress + unsteadyAnimationDelta);

        var t = EasingFunction.EaseInCirc(_unsteadyAnimationProgress);

        _top.position = _topPos + Vector2.up * _displacement * t;
        _right.position = _rightPos + Vector2.right * _displacement * t;
        _bottom.position = _bottomPos + Vector2.down * _displacement * t;
        _left.position = _leftPos + Vector2.left * _displacement * t;
    }
}

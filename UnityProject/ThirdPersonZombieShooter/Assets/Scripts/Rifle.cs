using UnityEngine;

public class Rifle : MonoBehaviour
{
    [SerializeField]
    LayerMask _targetsLayerMask;

    [SerializeField]
    Transform _muzzle;

    [SerializeField]
    LineRenderer _lineRenderer;

    [SerializeField]
    float _fireRate;

    float _timeSinceLastShot;

    PlayerInput _input;

    private void Start()
    {
        // Initialize and enable player input.
        _input = new();
        _input.Gameplay.Shoot.Enable();
    }

    private void Update()
    {
        _timeSinceLastShot += Time.deltaTime;

        // Animate bullet trail.

        var trailAnimationProgress = _timeSinceLastShot / (1 / _fireRate);

        // - Set alpha.
        var alpha = (1 - trailAnimationProgress) * 0.1f;

        _lineRenderer.startColor = new Color(1, 1, 1, alpha);
        _lineRenderer.endColor = new Color(1, 1, 1, alpha);

        // - Set width
        var width = 0.05f * (1 - trailAnimationProgress);

        _lineRenderer.startWidth = width;
        _lineRenderer.endWidth = width;




        if (_input.Gameplay.Shoot.ReadValue<float>() != 0 && _timeSinceLastShot > 1 / _fireRate)
            Shoot();
    }

    void Shoot()
    {
        Debug.Log("Shoot");

        Vector3 targetPosition;

        // Raycast in the direction of the gun.
        if (Physics.Raycast(_muzzle.position, _muzzle.forward, out var hitInfo, Mathf.Infinity, _targetsLayerMask))
            targetPosition = hitInfo.point;
        else
            targetPosition = _muzzle.position + _muzzle.forward * 1000;

        // Set the bullet trail.
        var lineRendererPositions = new Vector3[2];
        lineRendererPositions[0] = _muzzle.position;
        lineRendererPositions[1] = targetPosition;

        _lineRenderer.SetPositions(lineRendererPositions);

        _timeSinceLastShot = 0;
    }
}

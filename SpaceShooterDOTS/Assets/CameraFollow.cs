using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Default Follow")]
    [SerializeField] private Transform target;
    [SerializeField, Range(1, 10)] private float freeRadius = 2f;
    [SerializeField, Range(1, 20)] private float resetSpeed = 1f;
    [SerializeField, Range(0.1f, 1f)] private float margin = 0.5f;
    [SerializeField] private Transform _transform;

    [Header("Rotation")]
    [SerializeField, Range(15, 30), Tooltip("Angular speed in radians per sec.")] private float rotationSpeed = 20f;

    private bool _isLerping;
    private Camera _mainCamera;

    private void Awake()
    {
        _isLerping = false;
        _mainCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        ExecuteFollow();
    }

    private void ExecuteFollow()
    {
        RotateTowardsTarget();
        Vector3 camTargetDir = target.position - _transform.position;
        float distance = camTargetDir.magnitude;

        //Follow target from behind with free move zone
        Vector3 newPosition = target.position - target.forward * freeRadius;
        if (_isLerping)
        {
            _transform.localPosition = Vector3.Lerp(_transform.position, newPosition, resetSpeed * Time.deltaTime);
        }
        if (distance <= freeRadius && distance >= freeRadius - margin)
        {
            _isLerping = false;
        }
        else
        {
            _isLerping = true;
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 targetDirection = target.position - _transform.position;
        targetDirection.Normalize();
        Vector3 lookDirection = Vector3.RotateTowards(_transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
        _transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}

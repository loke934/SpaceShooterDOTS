using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("Default Follow")]
    [SerializeField] private Transform target;
    [SerializeField, Range(1, 10)] private float freeMoveRadius = 7f;
    [SerializeField] private Transform _transform;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float distToTarget = 7f;
    [SerializeField] private float yOffset = 2f;

    private bool _isLerping;

    private void Awake()
    {
        _isLerping = false;
    }

    private void LateUpdate()
    {
        ExecuteFollow();
    }

    private void ExecuteFollow()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        Vector3 newPosition = new Vector3(target.position.x, target.position.y + yOffset, target.position.z - distToTarget);

        if (_isLerping)
        {
            _transform.localPosition =  new Vector3(Mathf.Lerp(transform.position.x, newPosition.x, moveSpeed * Time.deltaTime), newPosition.y, newPosition.z);
        }
        if (distance <= freeMoveRadius)
        {
            _isLerping = false;
        }
        else
        {
            _isLerping = true;
        }
    }
    
}

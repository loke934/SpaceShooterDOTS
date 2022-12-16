using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(1f, 20f)] private float moveSpeed = 5f;
    [SerializeField] private Camera mainCamera;

    void Update()
    {
        ExecuteMovement();
    }

    void ExecuteMovement()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        Vector3 velocity = new Vector3(playerInput.x, playerInput.y, 0f) * moveSpeed;
        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPosition = transform.position + displacement;
        transform.localPosition = newPosition;
    }
}

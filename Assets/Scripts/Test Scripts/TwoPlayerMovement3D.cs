using UnityEngine;
using UnityEngine.InputSystem;

public class TwoPlayerMovement3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private InputActionAsset inputActions;

    [Header("Players (assign in Inspector)")]
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Rigidbody reflectionRb;

    [Header("Action Maps / Actions")]
    [SerializeField] private string playerMapName = "Player";
    [SerializeField] private string moveActionName = "Move";

    private InputAction playerMove;

    private void Awake()
    {
        SetupRb(playerRb);
        SetupRb(reflectionRb);

        var playerMap = inputActions.FindActionMap(playerMapName, true);
        playerMove = playerMap.FindAction(moveActionName, true);
    }

    private void OnEnable() => playerMove.Enable();
    private void OnDisable() => playerMove.Disable();

    private void FixedUpdate()
    {
        if (playerRb == null || reflectionRb == null) return;

        Vector2 input = playerMove.ReadValue<Vector2>();
        
        if (input.sqrMagnitude < 0.01f)
        {
            StopBoth();
            return;
        }

        // 1. Calculate and apply velocity
        Vector3 pVel = new Vector3(input.x, input.y, 0f) * moveSpeed;
        Vector3 rVel = new Vector3(input.x, -input.y, 0f) * moveSpeed;

        playerRb.linearVelocity = pVel;
        reflectionRb.linearVelocity = rVel;

        // 2. The Jitter Fix: 
        // We only force a stop if the actual movement is EXTREMELY low (stuck).
        // Using a lower threshold (like 0.1) prevents the "jitter" from triggering a stop.
        if (playerRb.linearVelocity.magnitude < 0.1f || reflectionRb.linearVelocity.magnitude < 0.1f)
        {
            StopBoth();
        }
    }

    private void StopBoth()
    {
        playerRb.linearVelocity = Vector3.zero;
        reflectionRb.linearVelocity = Vector3.zero;
    }

    private bool IsActuallyBlocked(Rigidbody rb, Vector3 intendedVel)
    {
        // If we are trying to move but the actual velocity is nearly 0
        // it means we are grinding against a wall.
        return rb.linearVelocity.magnitude < 0.5f; 
    }

    private void SetupRb(Rigidbody rb)
    {
        if (rb == null) return;
        rb.useGravity = false;
        // Collision Detection should be Continuous for maze walls
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }
}
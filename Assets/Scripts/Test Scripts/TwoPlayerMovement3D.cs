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

    // 1. Determine intended direction
    Vector3 pDir = new Vector3(input.x, input.y, 0f).normalized;
    Vector3 rDir = new Vector3(input.x, -input.y, 0f).normalized;

    // 2. PRE-CHECK: Can both move in their intended directions?
    // We "look ahead" by a tiny distance (0.1f)
    bool pPathBlocked = IsPathBlocked(playerRb, pDir);
    bool rPathBlocked = IsPathBlocked(reflectionRb, rDir);

    // 3. MUTUAL RULE: If either is blocked, both stop
    if (pPathBlocked || rPathBlocked)
    {
        StopBoth();
    }
    else
    {
        playerRb.linearVelocity = pDir * moveSpeed;
        reflectionRb.linearVelocity = rDir * moveSpeed;
    }
}

[Header("Physics Settings")]
[SerializeField] private LayerMask wallLayer; // Select "Maze" in the Inspector

private bool IsPathBlocked(Rigidbody rb, Vector3 direction)
{
    // 1. Calculate the 'Front' of your snowflake based on movement direction
    // Since your snowflake is 0.025 wide, the edge is 0.0125 away from center.
    float radius = 0.0125f; 
    Vector3 frontEdge = rb.position + (direction * radius);

    // 2. Use a "Skinny" sensor box so it doesn't catch on the sides of the walls
    // For a 0.025 wide object, a 0.01 width (half-extent of 0.005) is much safer.
    Vector3 skinnyBox = new Vector3(0.005f, 0.005f, 0.002f); 

    // 3. Look ahead just slightly further than before to catch walls early
    float checkDistance = 0.005f; 

    RaycastHit hit;
    if (Physics.BoxCast(frontEdge, skinnyBox, direction, out hit, rb.rotation, checkDistance, wallLayer))
    {
        // 4. THE ANGLE CHECK: Only stop if hitting the wall head-on
        // This stops you from getting stuck when just "grazing" a side wall.
        float angle = Vector3.Dot(hit.normal, direction);
        if (angle < -0.6f) 
        {
            return true;
        }
    }
    
    return false;
}

private void OnDrawGizmos()
{
    if (playerRb == null || reflectionRb == null) return;

    Gizmos.color = Color.red;
    Vector2 input = playerMove != null ? playerMove.ReadValue<Vector2>() : Vector2.zero;
    
    if (input.sqrMagnitude > 0.01f)
    {
        Vector3 pDir = new Vector3(input.x, input.y, 0f).normalized;
        Vector3 rDir = new Vector3(input.x, -input.y, 0f).normalized;

        // Draw the probe boxes
        Gizmos.DrawWireCube(playerRb.position + pDir * 0.015f, new Vector3(0.01f, 0.01f, 0.005f));
        Gizmos.DrawWireCube(reflectionRb.position + rDir * 0.015f, new Vector3(0.01f, 0.01f, 0.005f));
    }
}}

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
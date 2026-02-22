using UnityEngine;
using UnityEngine.InputSystem;

public class TwoPlayerMovement3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f; // Reduced for better control at 0.025 scale
    [SerializeField] private InputActionAsset inputActions;

    [Header("Players (assign in Inspector)")]
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Rigidbody reflectionRb;

    [Header("Action Maps / Actions")]
    [SerializeField] private string playerMapName = "Player";
    [SerializeField] private string moveActionName = "Move";

    [Header("Physics Settings")]
    [SerializeField] private LayerMask wallLayer; // Ensure "Maze" layer is selected in Inspector

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
        
        // 1. If no input, stop immediately
        if (input.sqrMagnitude < 0.01f)
        {
            StopBoth();
            return;
        }

        // 2. Determine intended direction
        Vector3 pDir = new Vector3(input.x, input.y, 0f).normalized;
        Vector3 rDir = new Vector3(input.x, -input.y, 0f).normalized;

        // 3. PRE-CHECK: Use the Goldilocks Probe to see if either is blocked
        bool pPathBlocked = IsPathBlocked(playerRb, pDir);
        bool rPathBlocked = IsPathBlocked(reflectionRb, rDir);

        // 4. MUTUAL RULE: If either hits a barrier, both stop
        if (pPathBlocked || rPathBlocked)
        {
            StopBoth();
        }
        else
        {
            // Apply velocity normally if path is clear
            playerRb.linearVelocity = pDir * moveSpeed;
            reflectionRb.linearVelocity = rDir * moveSpeed;
        }
    }

    private bool IsPathBlocked(Rigidbody rb, Vector3 direction)
    {
        // Dimensions for 0.025 scale: Skinny enough to ignore side walls (0.004f half-extent)
        Vector3 skinnyBox = new Vector3(0.004f, 0.004f, 0.001f); 
        
        // Start slightly inside the edge so the box doesn't spawn already hitting a wall
        float radius = 0.01f; 
        Vector3 probeStart = rb.position + (direction * radius);

        // Look ahead distance: Long enough to catch barriers before physics overlaps them
        float checkDistance = 0.005f;

        RaycastHit hit;
        // Shoot the BoxCast only against the Maze layer
        if (Physics.BoxCast(probeStart, skinnyBox, direction, out hit, rb.rotation, checkDistance, wallLayer))
        {
            // The Dot Product check: Only returns true if the wall is facing us (-1.0 to -0.5)
            // This allows the character to slide against side walls (0.0) without stopping
            float dot = Vector3.Dot(hit.normal, direction);
            if (dot < -0.5f) 
            {
                return true;
            }
        }
        return false;
    }

    private void StopBoth()
    {
        playerRb.linearVelocity = Vector3.zero;
        reflectionRb.linearVelocity = Vector3.zero;
    }

    private void SetupRb(Rigidbody rb)
    {
        if (rb == null) return;
        rb.useGravity = false;
        
        // Continuous detection is vital for micro-scale mesh walls
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 
        
        // Freeze Z and Rotations to keep movement strictly 2D on the XY plane
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        
        // Smooth out the visual movement
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        // Remove air resistance for snappy movement
        rb.linearDamping = 0f;

        // Increase solver iterations for better physics precision at small scales
        rb.solverIterations = 30;
    }

    // Visualizes the "Goldilocks" probe in the Scene View
    private void OnDrawGizmos()
    {
        if (playerRb == null || reflectionRb == null || playerMove == null) return;

        Gizmos.color = Color.yellow;
        Vector2 input = playerMove.ReadValue<Vector2>();
        
        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 pDir = new Vector3(input.x, input.y, 0f).normalized;
            Vector3 rDir = new Vector3(input.x, -input.y, 0f).normalized;

            // Draw WireCubes to show where the sensors are looking
            Gizmos.DrawWireCube(playerRb.position + pDir * 0.015f, new Vector3(0.008f, 0.008f, 0.005f));
            Gizmos.DrawWireCube(reflectionRb.position + rDir * 0.015f, new Vector3(0.008f, 0.008f, 0.005f));
        }
    }
}
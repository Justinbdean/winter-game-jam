using UnityEngine;
using UnityEngine.InputSystem;

public class TwoPlayerMovement3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f; 
    [SerializeField] private InputActionAsset inputActions;

    [Header("Players (assign in Inspector)")]
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Rigidbody reflectionRb;

    [Header("Action Maps / Actions")]
    [SerializeField] private string playerMapName = "Player";
    [SerializeField] private string moveActionName = "Move";

    [Header("Physics Settings")]
    [SerializeField] private LayerMask wallLayer; 

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

        Vector3 pDir = new Vector3(input.x, input.y, 0f).normalized;
        Vector3 rDir = new Vector3(input.x, -input.y, 0f).normalized;

        bool pPathBlocked = IsPathBlocked(playerRb, pDir);
        bool rPathBlocked = IsPathBlocked(reflectionRb, rDir);

        if (pPathBlocked || rPathBlocked)
        {
            StopBoth();
            
            playerRb.Sleep();
            reflectionRb.Sleep();
        }
        else
        {
            playerRb.linearVelocity = pDir * moveSpeed;
            reflectionRb.linearVelocity = rDir * moveSpeed;
        }
    }

    private bool IsPathBlocked(Rigidbody rb, Vector3 direction)
    {
        Vector3 skinnyBox = new Vector3(0.004f, 0.004f, 0.001f); 
        float radius = 0.01f; 
        Vector3 probeStart = rb.position + (direction * radius);
        float checkDistance = 0.005f;

        RaycastHit hit;
        if (Physics.BoxCast(probeStart, skinnyBox, direction, out hit, rb.rotation, checkDistance, wallLayer))
        {
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
        playerRb.angularVelocity = Vector3.zero;
        reflectionRb.angularVelocity = Vector3.zero;
    }

    private void SetupRb(Rigidbody rb)
    {
        if (rb == null) return;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.linearDamping = 0f;
        rb.solverIterations = 30;
    }

    private void OnDrawGizmos()
    {
        if (playerRb == null || reflectionRb == null || playerMove == null) return;

        Gizmos.color = Color.yellow;
        Vector2 input = playerMove.ReadValue<Vector2>();
        
        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 pDir = new Vector3(input.x, input.y, 0f).normalized;
            Vector3 rDir = new Vector3(input.x, -input.y, 0f).normalized;

            Gizmos.DrawWireCube(playerRb.position + pDir * 0.015f, new Vector3(0.008f, 0.008f, 0.005f));
            Gizmos.DrawWireCube(reflectionRb.position + rDir * 0.015f, new Vector3(0.008f, 0.008f, 0.005f));
        }
    }
}
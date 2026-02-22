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
    [SerializeField] private string reflectionMapName = "Player Reflection";
    [SerializeField] private string moveActionName = "Move";

    [Header("UI / Helpers")]
    [SerializeField] private GameObject solution;

    private InputAction playerMove;
    private InputAction reflectionMove;

    private void Awake()
    {
        SetupRb(playerRb);
        SetupRb(reflectionRb);

        var playerMap = inputActions.FindActionMap(playerMapName, true);
        playerMove = playerMap.FindAction(moveActionName, true);

        var reflectionMap = inputActions.FindActionMap(reflectionMapName, true);
        reflectionMove = reflectionMap.FindAction(moveActionName, true);
    }

    private void OnEnable()
    {
        playerMove.Enable();
        reflectionMove.Enable();
    }

    private void OnDisable()
    {
        playerMove.Disable();
        reflectionMove.Disable();
    }

    private void FixedUpdate()
    {
        // Move both as normal
        MoveRb(playerRb, playerMove.ReadValue<Vector2>());
        MoveRb(reflectionRb, reflectionMove.ReadValue<Vector2>());

        // 🔥 NEW: if either velocity becomes zero, force both to stop
        SyncStopIfOneStops();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.nKey.wasPressedThisFrame)
        {
            if (solution != null)
                solution.SetActive(!solution.activeSelf);
        }
    }

    private void SetupRb(Rigidbody rb)
    {
        if (rb == null) return;

        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    private void MoveRb(Rigidbody rb, Vector2 input)
    {
        if (rb == null) return;

        Vector3 move = new Vector3(input.x, input.y, 0f);

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        rb.linearVelocity = move * moveSpeed * Time.deltaTime;
    }

    // ⭐ THIS is the only new behavior you asked for
    private void SyncStopIfOneStops()
    {
        if (playerRb == null || reflectionRb == null) return;

        // Small threshold avoids float precision issues
        bool playerStopped = playerRb.linearVelocity.sqrMagnitude < 0.0001f;
        bool reflectionStopped = reflectionRb.linearVelocity.sqrMagnitude < 0.0001f;

        if (playerStopped || reflectionStopped)
        {
            playerRb.linearVelocity = Vector3.zero;
            reflectionRb.linearVelocity = Vector3.zero;
        }
    }
}
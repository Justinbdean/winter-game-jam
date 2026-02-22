using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement3D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private InputActionAsset inputActions;

    private InputAction moveAction;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();


        rb.useGravity = false;
        rb.freezeRotation = true;


        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        var map = inputActions.FindActionMap("Player", true);
        moveAction = map.FindAction("Move", true);
    }

    private void OnEnable() => moveAction.Enable();
    private void OnDisable() => moveAction.Disable();

    private void FixedUpdate()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(input.x, input.y, 0f);

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        rb.linearVelocity = new Vector3(input.x, input.y, 0f) * moveSpeed * Time.deltaTime;
    }
}
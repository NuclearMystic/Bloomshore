using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Very small, no-frills 3-D character mover.</summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintMultiplier = 2f;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] float gravity = -9.81f;

    CharacterController cc;
    PlayerControls controls;
    Vector2 moveInput;
    float yVel;
    bool isSprinting;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        controls = new PlayerControls();
        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += _ => moveInput = Vector2.zero;
        controls.Gameplay.Jump.started += _ => TryJump();
        controls.Gameplay.Sprint.started += _ => isSprinting = true; 
        controls.Gameplay.Sprint.canceled += _ => isSprinting = false; 
    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    void Update()
    {
        //   1. Horizontal motion in camera space
        Vector3 camF = Camera.main.transform.forward; camF.y = 0;
        Vector3 camR = Camera.main.transform.right; camR.y = 0;
        Vector3 move = (camF.normalized * moveInput.y + camR.normalized * moveInput.x).normalized;

        float speed = isSprinting ? walkSpeed * sprintMultiplier : walkSpeed;
        cc.Move(move * speed * Time.deltaTime);

        //   2. Gravity & vertical motion
        yVel += gravity * Time.deltaTime;
        cc.Move(Vector3.up * yVel * Time.deltaTime);
        if (cc.isGrounded && yVel < 0) yVel = -0.1f; // stick to ground
    }

    void TryJump()
    {
        if (cc.isGrounded) yVel = jumpForce;
    }
}

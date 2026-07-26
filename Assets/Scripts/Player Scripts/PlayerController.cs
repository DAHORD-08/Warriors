using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    public Transform cameraTransform;
    public ThirdPersonCamera thirdPersonCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private float smoothVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
        if (thirdPersonCamera == null && cameraTransform != null)
            thirdPersonCamera = cameraTransform.GetComponent<ThirdPersonCamera>();
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f) velocity.y = -2f;

        Vector2 input = Vector2.zero;
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.qKey.isPressed || kb.aKey.isPressed) input.x -= 1f;
            if (kb.dKey.isPressed) input.x += 1f;
            if (kb.zKey.isPressed || kb.wKey.isPressed) input.y += 1f;
            if (kb.sKey.isPressed) input.y -= 1f;
        }
        Vector3 inputDir = new Vector3(input.x, 0f, input.y).normalized;

        bool isLocked = thirdPersonCamera != null && thirdPersonCamera.IsLocked;

        if (isLocked)
        {
            Vector3 lookDir = thirdPersonCamera.LockedTarget.position - transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            if (inputDir.magnitude >= 0.1f && cameraTransform != null)
            {
                Vector3 camForward = cameraTransform.forward; camForward.y = 0f; camForward.Normalize();
                Vector3 camRight = cameraTransform.right; camRight.y = 0f; camRight.Normalize();
                Vector3 moveDir = camForward * input.y + camRight * input.x;
                controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
            }
        }
        else if (inputDir.magnitude >= 0.1f && cameraTransform != null)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVelocity, 1f / rotationSpeed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        if (kb != null && kb.spaceKey.wasPressedThisFrame && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
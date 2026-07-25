using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset = new Vector3(0f, 1.6f, 0f);
    public float distance = 5f;
    public float minDistance = 1.5f, maxDistance = 8f, zoomSpeed = 2f;
    public float sensitivityX = 0.2f, sensitivityY = 0.15f;
    public float minPitch = -30f, maxPitch = 60f;
    public string lockableTag = "Lockable";
    public float maxLockDistance = 20f;
    public float lockSpeed = 8f;

    private float yaw, pitch = 15f;
    private Transform lockedTarget;
    public bool IsLocked => lockedTarget != null;
    public Transform LockedTarget => lockedTarget;

    void Start()
    {
        if (target != null) yaw = target.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            lockedTarget = lockedTarget != null ? null : FindNearestLockable();
        }
        if (lockedTarget != null && !lockedTarget.gameObject.activeInHierarchy)
            lockedTarget = null;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Mouse mouse = Mouse.current;
        Vector3 focusPoint = target.position + targetOffset;
        Vector3 lookPoint = focusPoint;

        if (lockedTarget != null)
        {
            Vector3 dir = lockedTarget.position - target.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                yaw = Mathf.LerpAngle(yaw, Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg, lockSpeed * Time.deltaTime);
            pitch = Mathf.LerpAngle(pitch, 15f, lockSpeed * Time.deltaTime);
            lookPoint = Vector3.Lerp(focusPoint, lockedTarget.position + Vector3.up, 0.5f);
        }
        else if (mouse != null)
        {
            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * sensitivityX;
            pitch = Mathf.Clamp(pitch - delta.y * sensitivityY, minPitch, maxPitch);
        }

        if (mouse != null)
            distance = Mathf.Clamp(distance - mouse.scroll.ReadValue().y * zoomSpeed * 0.01f, minDistance, maxDistance);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pos = focusPoint - rot * Vector3.forward * distance;
        if (Physics.Linecast(focusPoint, pos, out RaycastHit hit)) pos = hit.point;

        transform.position = pos;
        transform.LookAt(lookPoint);
    }

    private Transform FindNearestLockable()
    {
        Transform nearest = null;
        float best = maxLockDistance * maxLockDistance;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(lockableTag))
        {
            float d = (go.transform.position - target.position).sqrMagnitude;
            if (d < best) { best = d; nearest = go.transform; }
        }
        return nearest;
    }
}
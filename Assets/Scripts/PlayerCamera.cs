using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] private float lookSensitivity = 0.2f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float fastMoveMultiplier = 3f;

    [Header("Pan")]
    [SerializeField] private float panSpeed = 0.5f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minFov = 20f;
    [SerializeField] private float maxFov = 80f;

    private float yaw;
    private float pitch;
    private Camera camera;

    private void Awake()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
        camera = GetComponent<Camera>();
        camera.fieldOfView = maxFov;
    }

    private void Update()
    {
        HandleLook();
        HandleFly();
        HandlePan();
        HandleScroll();
    }

    private void HandleLook()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (!Input.GetMouseButton(1)) return;

        yaw += Input.GetAxisRaw("Mouse X") * lookSensitivity * 100f * Time.deltaTime;
        pitch -= Input.GetAxisRaw("Mouse Y") * lookSensitivity * 100f * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -89f, 89f);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void HandleFly()
    {
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? fastMoveMultiplier : 1f);
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) dir += transform.forward;
        if (Input.GetKey(KeyCode.S)) dir -= transform.forward;
        if (Input.GetKey(KeyCode.A)) dir -= transform.right;
        if (Input.GetKey(KeyCode.D)) dir += transform.right;
        if (Input.GetKey(KeyCode.E)) dir += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) dir -= Vector3.up;

        transform.position += dir * speed * Time.deltaTime;
    }

    private void HandlePan()
    {
        if (!Input.GetMouseButton(2)) return;
        float x = -Input.GetAxisRaw("Mouse X") * panSpeed;
        float y = -Input.GetAxisRaw("Mouse Y") * panSpeed;
        transform.Translate(x, y, 0f, Space.Self);
    }

    private void HandleScroll()
    {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0f)) return;
        camera.fieldOfView = Mathf.Clamp(camera.fieldOfView - scroll * zoomSpeed * 100f * Time.deltaTime, minFov, maxFov);
    }
}
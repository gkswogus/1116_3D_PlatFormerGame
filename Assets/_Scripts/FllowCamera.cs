using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2.5f, -5);
    public bool orbitEnabled = true;
    public float sensitivity = 0.12f;
    public float minimumPitch = -25f;
    public float maximumPitch = 65f;
    public CinemachineCamera virtualCamera;
    CinemachineFollow body;
    float yaw;
    float pitch;

    public bool Aiming => orbitEnabled && Mouse.current != null && Mouse.current.rightButton.isPressed;

    void Start()
    {
        body = virtualCamera.GetComponent<CinemachineFollow>();
    }

    void Update()
    {
        if (target == null || virtualCamera == null)
        {
            return;
        }

        UpdateCursor(Aiming);

        if (Aiming)
        {
            ReadOrbitInput();
        }

        body.FollowOffset = Quaternion.Euler(pitch, yaw, 0) * offset;
    }

    void ReadOrbitInput()
    {
        Vector2 delta = Mouse.current.delta.ReadValue();
        yaw += delta.x * sensitivity;
        pitch -= delta.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minimumPitch, maximumPitch);
    }

    void UpdateCursor(bool aiming)
    {
        Cursor.lockState = aiming ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !aiming;
    }

    public void Snap()
    {
        if (body == null)
        {
            body = virtualCamera.GetComponent<CinemachineFollow>();
        }

        yaw = target.eulerAngles.y;
        pitch = 0;
        virtualCamera.Follow = target;
        Transform aim = target.Find("Camera Aim");

        if (aim == null)
        {
            aim = new GameObject("Camera Aim").transform;

            aim.SetParent(target, false);
        }

        aim.localPosition = Vector3.up * 1.5f;
        virtualCamera.LookAt = aim;
        body.FollowOffset = Quaternion.Euler(0, yaw, 0) * offset;
        virtualCamera.PreviousStateIsValid = false;
    }

    void OnDisable()
    {
        UpdateCursor(false);
    }
}

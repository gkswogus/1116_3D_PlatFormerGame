using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chinematic : MonoBehaviour
{
    public float sensitivity = 0.12f;
    public float minimumPitch = -30f;
    public float maximumPitch = 45f;

    CinemachineFollow follow;
    Vector3 startOffset;
    float yaw;
    float pitch;

    void Awake()
    {
        follow = GetComponent<CinemachineFollow>();
        startOffset = follow.FollowOffset;
    }

    void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        if (!mouse.rightButton.isPressed)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector2 mouseMovement = mouse.delta.ReadValue();

        yaw -= mouseMovement.x * sensitivity;

        pitch -= mouseMovement.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minimumPitch, maximumPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        follow.FollowOffset = rotation * startOffset;
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
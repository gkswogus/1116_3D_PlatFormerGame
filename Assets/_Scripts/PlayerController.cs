using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 360f;

    private CharacterController controller;
    private Camera maincamera;

    public bool jumping = false;

    private float upVelocity = 0f;
    private float gravity = -10f;
    private float jumpHeight = 1.5f;

    private Animator anim;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        maincamera = Camera.main;
        anim = GetComponentInChildren<Animator>();
    }

    // WASD 입력 받기
    Vector2 ReadMoveInput()
    {
        Vector2 input = Vector2.zero;
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed) input.y += 1f;
            if (keyboard.sKey.isPressed) input.y -= 1f;
            if (keyboard.dKey.isPressed) input.x += 1f;
            if (keyboard.aKey.isPressed) input.x -= 1f;
        }

        return input.normalized;
    }

    // 땅에 닿아 있는지 확인
    public bool Grounded()
    {
        return controller != null && controller.isGrounded;
    }

    // 점프 입력 확인
    bool JumpPressed()
    {
        Keyboard keyboard = Keyboard.current;

        return keyboard != null &&
               keyboard.spaceKey.wasPressedThisFrame;
    }

    // 캐릭터 회전
    private void RotatePlayer(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion rotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                rotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    // 캐릭터 이동 및 점프
    private void MovePlayer()
    {
        float dt = Time.deltaTime;

        Vector2 input = ReadMoveInput();

        // 카메라 방향을 수평으로 변환
        Vector3 forward = maincamera.transform.forward;
        Vector3 right = maincamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // 이동 방향 계산
        Vector3 moveDirection =
            forward * input.y + right * input.x;

        moveDirection = Vector3.ClampMagnitude(
            moveDirection, 1f
        );

        // 이동 방향으로 캐릭터 회전
        RotatePlayer(moveDirection);

        // 착지 상태 확인
        bool grounded = Grounded();

        // 땅에 있을 때
        if (grounded)
        {
            if (upVelocity < 0f)
            {
                upVelocity = -2f;
            }

            // 점프
            if (JumpPressed())
            {
                upVelocity = Mathf.Sqrt(
                    jumpHeight * -2f * gravity
                );
            }
        }
       
        // 중력 적용
        upVelocity += gravity * dt;

        // 수평 이동과 수직 이동 분리
        Vector3 velocity =
            moveDirection * moveSpeed +
            Vector3.up * upVelocity;

        // 실제 이동
        controller.Move(velocity * dt);
        anim.SetFloat("Speed", input.magnitude);
        anim.SetBool("Grounded", Grounded());
        // 점프 상태 갱신
        jumping = !Grounded();

        Debug.Log(jumping);
    }

    private void Update()
    {
        if (controller == null || maincamera == null)
            return;

        MovePlayer();

    }
}
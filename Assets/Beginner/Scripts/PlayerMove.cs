using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerMove : MonoBehaviour
{
    public float speed = 4;
    public float rotationSpeed = 360f;
    public int MaxJumpCount = 2;
    public ParticleSystem dustParticles;

    CharacterController controller;
    int jumpCount;
    float upVelocity;
    float gravity = -10f;
    float jumpHeight = 1.5f;
    Camera mainCamera;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        bool grounded = Grounded();

        if (grounded && upVelocity <= 0f)
        {
            upVelocity = -2f;
            jumpCount = 0;
        }

        Vector2 inputDir = ReadMoveInput();

        if (JumpPressed() && jumpCount < MaxJumpCount)
        {
            upVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpCount++;
        }

        // 이동 방향 계산
        Vector3 moveDir = mainCamera.transform.forward * inputDir.y
            + mainCamera.transform.right * inputDir.x;

        moveDir.y = 0f;                 // 위 방향 값 삭제 (XZ 평면만 사용)
        moveDir = moveDir.normalized;   // 벡터의 정규화

        MovePlayer(moveDir);
        RotatePlayer(moveDir);

        UpdateAnimator(moveDir.magnitude * speed);
        UpdateDustParticles();
    }

    void UpdateDustParticles()
    {
        if (dustParticles == null)
        {
            return;
        }

        Vector3 moveVelocity = controller.velocity;
        moveVelocity.y = 0f;
        bool movingOnGround = controller.isGrounded && moveVelocity.sqrMagnitude > 0.01f;

        if (movingOnGround && !dustParticles.isPlaying)
        {
            dustParticles.Play();
        }
        else if (!movingOnGround && dustParticles.isPlaying)
        {
            dustParticles.Stop();
        }
    }

    void OnDisable()
    {
        if (dustParticles != null)
        {
            dustParticles.Stop();
        }
    }

    void RotatePlayer(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion rotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(
                                        transform.rotation,
                                        rotation,
                            rotationSpeed * Time.deltaTime);
        }
    }

    void MovePlayer(Vector3 moveDir)
    {
        upVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDir * speed + Vector3.up * upVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    Vector2 ReadMoveInput()
    {
        Vector2 input = Vector2.zero;
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed)
            {
                input.y = 1;
            }

            if (keyboard.sKey.isPressed)
            {
                input.y = -1;
            }

            if (keyboard.dKey.isPressed)
            {
                input.x = 1;
            }

            if (keyboard.aKey.isPressed)
            {
                input.x = -1;
            }
        }
        return input;
    }

    private bool JumpPressed()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            return true;
        }

        return false;
    }

    public bool Grounded()
    {
        if (controller != null && controller.isGrounded)
        {
            return true;
        }

        return false;
    }

    void UpdateAnimator(float movementSpeed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", movementSpeed);
            animator.SetBool("Grounded", controller.isGrounded);
        }
    }
}



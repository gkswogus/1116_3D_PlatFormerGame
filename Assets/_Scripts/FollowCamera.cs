using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// Cinemachine 3인칭 카메라의 "이동 기준"과 커서를 담당
// - 이 스크립트는 CinemachineCamera 오브젝트에 붙임
// - 카메라 회전/공전/따라가기는 Cinemachine이 처리함
//     CinemachineOrbitalFollow       : 타겟 주위를 공전 (수평/수직 각도 축을 가짐)
//     CinemachineInputAxisController : 마우스(Look 액션) → OrbitalFollow 각도 축
//     CinemachineRotationComposer    : 타겟을 바라봄
// - 플레이어는 PlanarRotation을 기준으로 이동함 (W = 카메라가 보는 앞)
public class FollowCamera : MonoBehaviour
{
    [Header("Cinemachine 참조 (비워두면 같은 오브젝트에서 찾음)")]
    public CinemachineOrbitalFollow orbitalFollow;
    public CinemachineInputAxisController inputController;

    [Header("커서")]
    [Tooltip("켜면 마우스 오른쪽 버튼을 누를 때만 회전, 끄면 항상 회전(커서 잠금)")]
    public bool holdRightMouseToRotate = false;

    // 플레이어 이동 기준: OrbitalFollow의 수평 각도(목표값)
    // 실제 카메라 transform이 아니라 목표 각도를 쓰므로 damping으로 카메라가 늦게 따라와도 이동이 휘지 않음
    public Quaternion PlanarRotation =>
        Quaternion.Euler(0f, orbitalFollow.HorizontalAxis.Value, 0f);

    private void Awake()
    {
        if (orbitalFollow == null) orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        if (inputController == null) inputController = GetComponent<CinemachineInputAxisController>();
    }

    private void Start()
    {
        SetCursorLocked(!holdRightMouseToRotate);
    }

    private void Update()
    {
        UpdateCursorState();

        // 커서가 잠겨 있을 때만 Cinemachine이 마우스 입력을 읽도록 켬/끔
        if (inputController != null)
        {
            inputController.enabled = Cursor.lockState == CursorLockMode.Locked;
        }
    }

    // 항상 회전 모드: Esc로 커서 풀기, 화면 클릭으로 다시 잠그기
    // 우클릭 모드: 누르는 동안만 커서 잠그기
    private void UpdateCursorState()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (holdRightMouseToRotate)
        {
            SetCursorLocked(mouse != null && mouse.rightButton.isPressed);
            return;
        }

        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            SetCursorLocked(false);
        }
        else if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            SetCursorLocked(true);
        }
    }

    private void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void OnDisable()
    {
        SetCursorLocked(false);
    }
}

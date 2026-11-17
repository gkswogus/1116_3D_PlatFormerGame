using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

// 점프맵: 게임 시작 시 발판이 보이고, hideDelay초 뒤 투명 머티리얼로 바뀜
// 이 오브젝트의 자식에 있는 모든 MeshRenderer(발판)에 적용됨
public class JumpMap : MonoBehaviour
{
    [Tooltip("시작할 때 발판에 씌울 보이는 머티리얼 (비워두면 발판의 원래 머티리얼 사용)")]
    public Material visibleMaterial;

    [Tooltip("hideDelay초 뒤 발판에 씌울 투명 머티리얼")]
    [FormerlySerializedAs("mt")]
    public Material transparentMaterial;

    [Tooltip("게임 시작 후 투명해질 때까지의 시간(초)")]
    public float hideDelay = 1f;

    private MeshRenderer[] platforms;

    private void Awake()
    {
        // 자식 발판의 렌더러를 전부 모음 (Components: 복수형)
        platforms = GetComponentsInChildren<MeshRenderer>();

        if (visibleMaterial != null)
        {
            SetPlatformMaterial(visibleMaterial);
        }
    }

    private void Start()
    {
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        if (transparentMaterial != null)
        {
            SetPlatformMaterial(transparentMaterial);
        }
    }

    private void SetPlatformMaterial(Material material)
    {
        foreach (MeshRenderer platform in platforms)
        {
            // renderer.materials[0] = x 는 복사본만 바뀌어서 적용되지 않음
            // sharedMaterial에 대입해야 실제로 바뀜 (머티리얼 복제도 생기지 않음)
            platform.sharedMaterial = material;
        }
    }
}

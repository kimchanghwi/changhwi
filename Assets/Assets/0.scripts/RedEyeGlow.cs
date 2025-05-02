using UnityEngine;

public class RedEyeGlow : MonoBehaviour
{
    [Header("필수 연결")]
    public SpriteRenderer spriteRenderer; // Inspector에서 연결
    public Transform player;              // 플레이어
    public Transform ai;                  // AI (거리 계산 대상)

    // 게임 시작 시 측정한 플레이어-AI 간 초기 거리
    private float initialDistance;

    void Start()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("[RedEyeGlow] SpriteRenderer가 연결되지 않았습니다.");
            enabled = false;
            return;
        }

        if (player == null || ai == null)
        {
            Debug.LogError("[RedEyeGlow] Player 혹은 AI가 연결되지 않았습니다.");
            enabled = false;
            return;
        }

        // 게임 시작 시 플레이어와 AI의 2D 거리 측정
        initialDistance = Vector2.Distance(player.position, ai.position);

        // 초기 상태: 최대 거리에서 알파값 = 0 (완전 투명)
        SetAlpha(0f);
    }

    void Update()
    {
        // 플레이어 혹은 AI가 삭제되었는지 재확인
        if (player == null || ai == null) return;

        // 현재 거리 측정
        float currentDistance = Vector2.Distance(player.position, ai.position);

        // 알파값 계산
        //    - (초기 거리 - 현재 거리) / 초기 거리
        //    - 가까워질수록 값이 커져서 불투명해짐
        float alpha = (initialDistance - currentDistance) / initialDistance;

        // 0~1 범위로 제한
        alpha = Mathf.Clamp01(alpha);

        // 알파값 적용
        SetAlpha(alpha);

        Debug.Log($"[RedEyeGlow] Distance: {currentDistance:F3}, Alpha: {alpha:F3}");
    }

    private void SetAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}

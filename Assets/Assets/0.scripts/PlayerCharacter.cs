using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;          // 스프라이트 컴포넌트 참조

    [Header("이동/점프 설정")]
    public float startSpeed = 5f;                   // 시작 속도
    public float accelerationRate = 2f;             // 초당 속도 증가량
    public float maxSpeed = 7f;                     // 최대 속도
    private float currentSpeed;                     // 현재 이동 속도
    public float jumpForce = 10f;                   // 점프 시 위로 가해질 힘
    private bool isGrounded = false;                // 땅에 닿았는지 체크

    [Header("충돌 애니메이션 설정")]
    public string victoryAnimationName = "victory"; // 플레이어 승리 애니메이션

    [Header("더블 점프 설정")]
    public int maxJumpCount = 2;                    // 최대 점프 횟수 (2단 점프)
    private int jumpCount = 0;                      // 현재 점프 횟수

    [Header("충돌 무효화 시간 설정")]
    public float ignoreDuration = 2f;               // 충돌 무효화 시간(초)

    // AI와의 충돌 발생 시 정지
    private bool aiCollisionOccurred = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer 초기화
        currentSpeed = startSpeed;
    }

    void Update()
    {
        // AI와의 충돌이 발생하면 영구 정지
        if (aiCollisionOccurred)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // 가속도 적용
        currentSpeed += accelerationRate * Time.deltaTime;
        if (currentSpeed > maxSpeed)
            currentSpeed = maxSpeed;

        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        // 점프 
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
        }

        // 점프 중일 때 애니메이션 정지, 착지 시 재개
        animator.speed = isGrounded ? 1f : 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 땅과 충돌하면 점프 횟수 초기화
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }

        // 플레이어가 AI와 충돌하면 영구 정지 처리
        if (collision.gameObject.CompareTag("ai") && !aiCollisionOccurred)
        {
            aiCollisionOccurred = true;

            // 승리 애니메이션 재생 및 영구 정지
            animator.Play(victoryAnimationName);
            currentSpeed = 0f;
            rb.velocity = Vector2.zero;
        }

        // 플레이어가 장애물과 충돌하면 일시적으로 정지
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            // 일시 정지 코루틴 실행
            StartCoroutine(TemporaryStop());

            // 충돌 판정을 일정 시간 무효화
            Collider2D playerCollider = GetComponent<Collider2D>();
            Collider2D obstacleCollider = collision.collider;
            StartCoroutine(DisableCollisionForSeconds(playerCollider, obstacleCollider, ignoreDuration));

            // 무효화 시간 동안 스프라이트 깜빡임 효과 적용
            StartCoroutine(BlinkSprite(ignoreDuration, 0.1f));
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
            isGrounded = false;
    }

    // 장애물 충돌 시 일시 정지 후 회복하는 코루틴
    IEnumerator TemporaryStop()
    {
        // 저장된 속도를 보존할 필요가 있다면 여기서 임시 저장
        float savedSpeed = currentSpeed;
        currentSpeed = 2f;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(ignoreDuration);

        // 일시 정지 후 다시 기본 속도(또는 원하는 속도)로 회복
        currentSpeed = startSpeed;
    }

    // 플레이어와 장애물 간 충돌 판정을 일정 시간 무효화하는 코루틴
    IEnumerator DisableCollisionForSeconds(Collider2D col1, Collider2D col2, float duration)
    {
        Physics2D.IgnoreCollision(col1, col2, true);
        yield return new WaitForSeconds(duration);
        Physics2D.IgnoreCollision(col1, col2, false);
    }

    // 무효화 시간 동안 스프라이트 깜빡임 효과를 주는 코루틴
    IEnumerator BlinkSprite(float duration, float blinkInterval)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(blinkInterval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval * 2;
        }
        // 깜빡임 종료 후 스프라이트가 활성 상태인지 확인
        spriteRenderer.enabled = true;
    }
}

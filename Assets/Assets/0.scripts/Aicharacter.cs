using UnityEngine;

public class AICharacter : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("이동 설정")]
    public float speed = 5f;                             // AI의 이동속도

    [Header("충돌 애니메이션 설정")]
    public string loseAnimationName = "lose";            // AI 패배 애니메이션

    // 충돌 발생 시 이동 정지를 위한 플래그
    private bool collisionOccurred = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rb.velocity = new Vector2(speed,rb.velocity.y);

        // 충돌 발생 후에는 이동 업데이트를 중단
        if (collisionOccurred)
        {
            rb.velocity = Vector2.zero;
            return;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어와 충돌 시 AI 패배 처리
        if (collision.gameObject.CompareTag("player") && !collisionOccurred)
        {
            collisionOccurred = true;
            rb.velocity = Vector2.zero;
            animator.Play(loseAnimationName);
        }

        // 장애물과의 충돌 처리 (필요 시)
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            collisionOccurred = true;
            rb.velocity = Vector2.zero;
        }
    }
}

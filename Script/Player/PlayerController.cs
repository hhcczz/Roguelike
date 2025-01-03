using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb; // Rigidbody2D 참조
    private Vector2 movement; // 이동 벡터

    private Animator animator; // Animator 참조
    private float lastHitTime = 0f; // 마지막으로 공격받은 시간
    private readonly float hitCooldown = 0.1f; // 적 공격 쿨다운 시간

    private void Start()
    {
        // Rigidbody2D 및 Animator 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // WASD 또는 방향키 입력 처리
        movement.x = Input.GetAxisRaw("Horizontal"); // 좌우 입력
        movement.y = Input.GetAxisRaw("Vertical");   // 상하 입력

        // 이동 방향에 따라 회전 처리
        if (movement.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // 오른쪽 방향
        }
        else if (movement.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // 왼쪽 방향
        }
    }

    private void FixedUpdate()
    {
        // Rigidbody2D를 사용한 이동 처리
        rb.velocity = movement.normalized * PlayerStateManager.Instance.GetTotalPlayerSpeed();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 특정 태그("Structures")와 충돌한 경우 처리
        if (collision.gameObject.CompareTag("Structures"))
        {
            // 충돌한 방향 계산
            Vector2 collisionNormal = collision.contacts[0].normal;

            // 반대 방향으로 움직이도록 설정
            Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, collisionNormal);
            rb.velocity = reflectedVelocity * PlayerStateManager.Instance.GetTotalPlayerSpeed();

            Debug.Log($"구조물과 충돌 발생! 새로운 방향: {reflectedVelocity}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 특정 태그("Monster")와 트리거 충돌 처리
        if (other.CompareTag("Monster"))
        {
            if (Time.time - lastHitTime >= hitCooldown)
            {
                lastHitTime = Time.time; // 마지막 히트 시간 갱신

                // 몬스터의 공격력 가져오기
                MonsterController monsterController = other.GetComponent<MonsterController>();
                if (monsterController != null)
                {
                    float monsterDamage = monsterController.GetAttackDamage();
                    monsterDamage -= PlayerStateManager.Instance.GetTotalArmor();
                    monsterDamage = Mathf.Max(monsterDamage, 0f);

                    PlayerStateManager.Instance.ReduceHealth(monsterDamage);
                    Debug.Log($"몬스터 공격으로 {monsterDamage} 데미지를 입음.");
                }

                // 히트 애니메이션 재생
                animator.SetTrigger("Hit");

                // 0.5초 동안 애니메이션 재생 유지
                Invoke(nameof(ResetAnimation), 0.5f);
            }
        }
    }

    private void ResetAnimation()
    {
        animator.ResetTrigger("Hit");
    }
}

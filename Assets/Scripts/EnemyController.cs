using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, Health.IHealthListener
{
    enum State
    {
        Idle,
        Follow,
        Attack,
        Die
    }

    GameObject player;
    NavMeshAgent agent;
    Animator animator;
    new AudioSource audio;

    State state;

    float currentStateTime;
    public float TimeForNextState = 2;


    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        audio = GetComponent<AudioSource>();

        state = State.Idle;
        currentStateTime = TimeForNextState;
    }

    private void Update()
    {
        switch (state) // 현재 상태에 따라
        {
            case State.Idle: // Idle 상태일 때
                currentStateTime -= Time.deltaTime; // 현재 상태의 지속시간 감소
                if (currentStateTime < 0) // 현재 상태가 0초 미만이면
                {
                    float distance = (player.transform.position - transform.position).magnitude; // 플레이어와 적 유닛 간의 거리
                    if (distance < 1.5f) // 거리가 1.5m 이내면
                    {
                        StartAttack(); // 공격 전환
                    }
                    else // 1.5m 밖이면
                    {
                        StartFollow(); // 따라가기 전환
                    }
                }
                break;
            case State.Follow:
                if (agent.remainingDistance < 1.5f || !agent.hasPath) // 플레이어 간의 거리가 1.5m 이내이거나
                {// 갈 수 있는 경로가 없을 때
                    StartIdle(); // Idle 전환
                }
                break;
            case State.Attack:
                currentStateTime -= Time.deltaTime;
                if (currentStateTime < 0)
                {
                    StartIdle();
                }
                break;
        }
    }

    void StartIdle()
    {
        audio.Stop();
        state = State.Idle; // Idle 상태 전환
        currentStateTime = TimeForNextState; // 현재 상태 지속시간 초기화
        agent.isStopped = true; // 적 유닛 정지
        animator.SetTrigger("Idle"); // 애니메이션 트리거 Idle 작동
    }

    void StartFollow()
    {
        audio.Play();
        state = State.Follow; // Follow 상태 전환
        agent.destination = player.transform.position; // 도착지를 플레이어 위치로 지정
        agent.isStopped = false; // 적 유닛 이동
        animator.SetTrigger("Run"); // 애니메이션 트리거 Run 작동
    }

    void StartAttack()
    {
        state = State.Attack; // Attack 상태 전환
        currentStateTime = TimeForNextState; // 현재 상태 지속시간 초기화
        animator.SetTrigger("Attack"); // 애니메이션 트리거 Attack 작동
    }

    public void OnDie()
    {
        state = State.Die; // Die 상태 전환
        agent.isStopped = true; // 적 유닛 정지
        animator.SetTrigger("Die"); // 애니메이션 트리거 Die 작동
        Invoke("DestroyThis", 2f); // Invoke로 DestroyThis를 2초 후 작동
    }

    void DestroyThis()
    {
        GameManager.Instance.EnemyDie();
        Destroy(gameObject); // 오브젝트 파괴
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") // 콜라이더의 태그가 플레이어라면
        {
            other.GetComponent<Health>().Damage(1); // 플레이어의 Health 컴포넌트에서 Damage(1)을 호출
        }
    }
}

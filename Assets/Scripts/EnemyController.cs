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
        switch (state) // ���� ���¿� ����
        {
            case State.Idle: // Idle ������ ��
                currentStateTime -= Time.deltaTime; // ���� ������ ���ӽð� ����
                if (currentStateTime < 0) // ���� ���°� 0�� �̸��̸�
                {
                    float distance = (player.transform.position - transform.position).magnitude; // �÷��̾�� �� ���� ���� �Ÿ�
                    if (distance < 1.5f) // �Ÿ��� 1.5m �̳���
                    {
                        StartAttack(); // ���� ��ȯ
                    }
                    else // 1.5m ���̸�
                    {
                        StartFollow(); // ���󰡱� ��ȯ
                    }
                }
                break;
            case State.Follow:
                if (agent.remainingDistance < 1.5f || !agent.hasPath) // �÷��̾� ���� �Ÿ��� 1.5m �̳��̰ų�
                {// �� �� �ִ� ��ΰ� ���� ��
                    StartIdle(); // Idle ��ȯ
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
        state = State.Idle; // Idle ���� ��ȯ
        currentStateTime = TimeForNextState; // ���� ���� ���ӽð� �ʱ�ȭ
        agent.isStopped = true; // �� ���� ����
        animator.SetTrigger("Idle"); // �ִϸ��̼� Ʈ���� Idle �۵�
    }

    void StartFollow()
    {
        audio.Play();
        state = State.Follow; // Follow ���� ��ȯ
        agent.destination = player.transform.position; // �������� �÷��̾� ��ġ�� ����
        agent.isStopped = false; // �� ���� �̵�
        animator.SetTrigger("Run"); // �ִϸ��̼� Ʈ���� Run �۵�
    }

    void StartAttack()
    {
        state = State.Attack; // Attack ���� ��ȯ
        currentStateTime = TimeForNextState; // ���� ���� ���ӽð� �ʱ�ȭ
        animator.SetTrigger("Attack"); // �ִϸ��̼� Ʈ���� Attack �۵�
    }

    public void OnDie()
    {
        state = State.Die; // Die ���� ��ȯ
        agent.isStopped = true; // �� ���� ����
        animator.SetTrigger("Die"); // �ִϸ��̼� Ʈ���� Die �۵�
        Invoke("DestroyThis", 2f); // Invoke�� DestroyThis�� 2�� �� �۵�
    }

    void DestroyThis()
    {
        GameManager.Instance.EnemyDie();
        Destroy(gameObject); // ������Ʈ �ı�
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") // �ݶ��̴��� �±װ� �÷��̾���
        {
            other.GetComponent<Health>().Damage(1); // �÷��̾��� Health ������Ʈ���� Damage(1)�� ȣ��
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float Hp = 10;
    public float MaxHp = 10;
    public float invincibleTime;

    public AudioClip HurtSound;
    public AudioClip DieSound;

    public Image HPGauge;

    IHealthListener healthListner;
    float lastDamageTime;

    void Start()
    {
        healthListner = GetComponent<IHealthListener>();
    }

    public void Damage(float damage)
    {
        if (Hp > 0 && lastDamageTime + invincibleTime < Time.time) // ���� �ð��� (���������� ���� �ð� + ���� �ð�)���� ���ĸ�
        {
            Hp -= damage; // hp ����

            if (HPGauge != null) // HPGauge �ν��Ͻ��� ������� �ʴٸ�
            {
                HPGauge.fillAmount = Hp / MaxHp; // hp�� maxhp�� ���� ���� hpgauge�� ä�� �������� ����
            }

            lastDamageTime = Time.time; // ���� �ð� ����

            if (Hp <= 0) // hp�� 0���� �۰ų� ������
            {
                if (healthListner != null) // listner�� ������� �ʴٸ�
                {
                    healthListner.OnDie(); // ondie ȣ��
                }
                if (DieSound != null)
                {
                    GetComponent<AudioSource>().PlayOneShot(DieSound);
                }
            }
            else // hp�� 0���� ũ��
            {
                GetComponent<AudioSource>().PlayOneShot(HurtSound);
            }
        }
    }

    public interface IHealthListener
    {
        void OnDie();
    }
}

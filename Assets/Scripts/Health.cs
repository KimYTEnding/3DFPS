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
        if (Hp > 0 && lastDamageTime + invincibleTime < Time.time) // 현재 시간이 (마지막으로 맞은 시간 + 무적 시간)보다 이후면
        {
            Hp -= damage; // hp 감소

            if (HPGauge != null) // HPGauge 인스턴스가 비어있지 않다면
            {
                HPGauge.fillAmount = Hp / MaxHp; // hp를 maxhp로 나눈 값을 hpgauge의 채움 수준으로 설정
            }

            lastDamageTime = Time.time; // 맞은 시간 저장

            if (Hp <= 0) // hp가 0보다 작거나 같으면
            {
                if (healthListner != null) // listner가 비어있지 않다면
                {
                    healthListner.OnDie(); // ondie 호출
                }
                if (DieSound != null)
                {
                    GetComponent<AudioSource>().PlayOneShot(DieSound);
                }
            }
            else // hp가 0보다 크면
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

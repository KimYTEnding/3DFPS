using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float time; // 터지는 시간
    public float Damage;
    public AudioClip explodeSound;

    private void Update()
    {
        time -= Time.deltaTime; // 터지는 시간 감소

        if (time <= 0) // 시간이 다 지났을 때
        {
            GetComponent<Animator>().SetTrigger("Explode"); // 트리거 발동
            Destroy(gameObject, 1); // 폭발반경과 함께 수류탄 오브젝트 삭제 / 애니메이션 시간에 따라 시간 지정
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Health>().Damage(Damage);
        }
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().PlayOneShot(explodeSound);
    }
}

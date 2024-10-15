using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float time; // ������ �ð�
    public float Damage;
    public AudioClip explodeSound;

    private void Update()
    {
        time -= Time.deltaTime; // ������ �ð� ����

        if (time <= 0) // �ð��� �� ������ ��
        {
            GetComponent<Animator>().SetTrigger("Explode"); // Ʈ���� �ߵ�
            Destroy(gameObject, 1); // ���߹ݰ�� �Բ� ����ź ������Ʈ ���� / �ִϸ��̼� �ð��� ���� �ð� ����
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

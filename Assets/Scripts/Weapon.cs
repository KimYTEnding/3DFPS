using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    public GameObject TrailPrefab; // �Ѿ� ���� ������
    public Transform FiringPosition; // �� ��ġ, �Ѿ��� ������ ������ ��ġ
    public GameObject ParticlePrefab;
    public TMP_Text BulletText;
    public AudioClip GunShotSound;

    public int CurrentBullet = 8; // ��ź ��
    public int TotalBullet = 30; // ��ü ź�� ��
    public int MaxBulletInMagazine = 8; // źâ �� ź�� ��

    public float Damage = 1;

    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        BulletText.text = CurrentBullet.ToString() + " / " + TotalBullet.ToString(); // ź�� text ���
    }

    public void FireWeapon()
    {
        if (CurrentBullet > 0) // ���� ��ź�� ������ ��
        {
            if (animator != null) // �ִϸ����Ͱ� �ִٸ�
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) // ���°� idle�̸�
                {
                    animator.SetTrigger("Fire"); // Ʈ���� �ߵ�
                    CurrentBullet--; // ��ź ����
                    Fire(); // �޼ҵ� ȣ��
                }
            }
            else
            {
                CurrentBullet--; // ��ź ����
                Fire(); // �޼ҵ� ȣ��
            }
        }
    }

    protected virtual void Fire()
    {
        RayCastFire();
    }

    public void ReloadWeapon()
    {
        if (TotalBullet > 0) // ź���� ������ ���
        {
            if (animator != null) // �ִϸ����Ͱ� ���� ���
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) // ���°� idle�̸�
                {
                    animator.SetTrigger("Reload"); // Ʈ���� �ߵ�
                    Reload(); // �޼ҵ� ȣ��
                }
            }
            else
            {
                Reload();
            }
        }
    }

    void Reload()
    {
        if (TotalBullet >= MaxBulletInMagazine - CurrentBullet) // ź�� ���� (źâ ź�� - ��ź)���� ���ų� ������
        {
            TotalBullet -= MaxBulletInMagazine - CurrentBullet; // ź�࿡�� (źâ ź�� - ��ź)�� ����
            CurrentBullet = MaxBulletInMagazine; // ��ź�� źâ ź������ ����
        }
        else // ź�� ���� źâ ź�� - ��ź���� ������
        {
            CurrentBullet += TotalBullet; // ��ź�� ���� ź���� �־���
            TotalBullet = 0; // ź���� ���
        }
    }

    void RayCastFire()
    {
        GetComponent<AudioSource>().PlayOneShot(GunShotSound);
        Camera cam = Camera.main; // ī�޶� ��������

        RaycastHit hit; // ���� �߻��Ͽ� ����� �޾ƿ� ����
        Ray r = cam.ViewportPointToRay(Vector3.one / 2); // ī�޶� �ֽ��ϴ� ȭ���� ���

        Vector3 hitPosition = r.origin + r.direction * 200; // HitPosition ���� / ray ��ġ + ray ���� * 200

        if (Physics.Raycast(r, out hit, 1000)) // �� r�� ��� �浹 ���(hit)�� �޾ƿ�
        { // ��򰡿� ���� �ε����� true / �� r�� �߻��Ͽ� ����� �޴� hit, �߻� �ִ�Ÿ��� 1000
            hitPosition = hit.point; // �ε��� ��ǥ�� ����

            GameObject particle = Instantiate(ParticlePrefab); // ��ƼŬ ����
            particle.transform.position = hitPosition; // ���� ��ǥ�� ��ƼŬ ��ġ ����
            particle.transform.forward = hit.normal; // ���� ��ǥ�� ǥ���� ��ƼŬ ��������

            if (hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<Health>().Damage(Damage);
            }
        }

        GameObject go = Instantiate(TrailPrefab); // Ʈ���϶��� ������ ������Ʈ ����
        Vector3[] pos = new Vector3[] { FiringPosition.position, hitPosition }; // ������ ���� 3D �� ��ġ ����
        go.GetComponent<LineRenderer>().SetPositions(pos); // Ʈ���϶��� ������ ��ǥ����
        go.GetComponent<SelfDestroy>().time = 0.1f;
        Destroy(go, 0.5f);
        //StartCoroutine(DestroyTrail(go));
    }

    //IEnumerator DestroyTrail(GameObject obj)
    //{
    //    yield return new WaitForSeconds(0.5f); // 0.5f ��ŭ ��ٸ�
    //    //yield return null; // 1������ ��ٸ�
    //    Destroy(obj);
    //}
}

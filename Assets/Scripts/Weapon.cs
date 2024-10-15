using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    public GameObject TrailPrefab; // 총알 궤적 프리팹
    public Transform FiringPosition; // 쏜 위치, 총알이 나가기 시작한 위치
    public GameObject ParticlePrefab;
    public TMP_Text BulletText;
    public AudioClip GunShotSound;

    public int CurrentBullet = 8; // 잔탄 수
    public int TotalBullet = 30; // 전체 탄약 수
    public int MaxBulletInMagazine = 8; // 탄창 당 탄약 수

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
        BulletText.text = CurrentBullet.ToString() + " / " + TotalBullet.ToString(); // 탄약 text 출력
    }

    public void FireWeapon()
    {
        if (CurrentBullet > 0) // 현재 잔탄이 남았을 때
        {
            if (animator != null) // 애니메이터가 있다면
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) // 상태가 idle이면
                {
                    animator.SetTrigger("Fire"); // 트리거 발동
                    CurrentBullet--; // 잔탄 감소
                    Fire(); // 메소드 호출
                }
            }
            else
            {
                CurrentBullet--; // 잔탄 감소
                Fire(); // 메소드 호출
            }
        }
    }

    protected virtual void Fire()
    {
        RayCastFire();
    }

    public void ReloadWeapon()
    {
        if (TotalBullet > 0) // 탄약이 남았을 경우
        {
            if (animator != null) // 애니메이터가 있을 경우
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) // 상태가 idle이면
                {
                    animator.SetTrigger("Reload"); // 트리거 발동
                    Reload(); // 메소드 호출
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
        if (TotalBullet >= MaxBulletInMagazine - CurrentBullet) // 탄약 수가 (탄창 탄약 - 잔탄)보다 같거나 많으면
        {
            TotalBullet -= MaxBulletInMagazine - CurrentBullet; // 탄약에서 (탄창 탄약 - 잔탄)을 빼줌
            CurrentBullet = MaxBulletInMagazine; // 잔탄을 탄창 탄약으로 설정
        }
        else // 탄약 수가 탄창 탄약 - 잔탄보다 적으면
        {
            CurrentBullet += TotalBullet; // 잔탄에 현재 탄약을 넣어줌
            TotalBullet = 0; // 탄약을 비움
        }
    }

    void RayCastFire()
    {
        GetComponent<AudioSource>().PlayOneShot(GunShotSound);
        Camera cam = Camera.main; // 카메라 가져오기

        RaycastHit hit; // 빛을 발사하여 결과를 받아올 변수
        Ray r = cam.ViewportPointToRay(Vector3.one / 2); // 카메라가 주시하는 화면의 가운데

        Vector3 hitPosition = r.origin + r.direction * 200; // HitPosition 선언 / ray 위치 + ray 방향 * 200

        if (Physics.Raycast(r, out hit, 1000)) // 빛 r을 쏘아 충돌 결과(hit)를 받아옴
        { // 어딘가에 빛이 부딪히면 true / 빛 r을 발사하여 결과를 받는 hit, 발사 최대거리는 1000
            hitPosition = hit.point; // 부딪힌 좌표값 저장

            GameObject particle = Instantiate(ParticlePrefab); // 파티클 생성
            particle.transform.position = hitPosition; // 맞은 좌표에 파티클 위치 지정
            particle.transform.forward = hit.normal; // 맞은 좌표의 표면을 파티클 정면으로

            if (hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<Health>().Damage(Damage);
            }
        }

        GameObject go = Instantiate(TrailPrefab); // 트레일라인 프리팹 오브젝트 생성
        Vector3[] pos = new Vector3[] { FiringPosition.position, hitPosition }; // 라인을 그을 3D 점 위치 저장
        go.GetComponent<LineRenderer>().SetPositions(pos); // 트레일라인 꼭짓점 좌표지정
        go.GetComponent<SelfDestroy>().time = 0.1f;
        Destroy(go, 0.5f);
        //StartCoroutine(DestroyTrail(go));
    }

    //IEnumerator DestroyTrail(GameObject obj)
    //{
    //    yield return new WaitForSeconds(0.5f); // 0.5f 만큼 기다림
    //    //yield return null; // 1프레임 기다림
    //    Destroy(obj);
    //}
}

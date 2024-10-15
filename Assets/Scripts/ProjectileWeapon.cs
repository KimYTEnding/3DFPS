using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public GameObject ProjectilePrefab; // 발사 오브젝트
    public float ProjectileAngle = 30; // 발사 각도
    public float ProjectileForce = 10; // 발사 힘
    public float ProjectileTime = 5; // 발사 시간

    protected override void Fire()
    {
        ProjectileFire();
    }

    void ProjectileFire()
    {
        Camera cam = Camera.main; // 카메라 불러오기

        Vector3 forward = cam.transform.forward; // 카메라가 주시하는 방향 벡터
        /// 1방법
        //Vector3 up = cam.transform.up; // 카메라의 y축 벡터

        //Vector3 direction = forward + up * Mathf.Tan(ProjectileAngle * Mathf.Deg2Rad); // 주시 방향 + (라디안으로 치환한 발사각의 탄젠트값 * y축 벡터)

        /// 2 방법
        Vector3 direction = Quaternion.AngleAxis(-ProjectileAngle, cam.transform.right) * forward;

        direction.Normalize(); // 벡터 값 정규화
        direction *= ProjectileForce; // 발사 방향에 발사 힘 곱하기

        GameObject go = Instantiate(ProjectilePrefab); // 수류탄 오브젝트 생성
        go.transform.position = FiringPosition.position; // 생성한 수류탄 초기 위치 지정
        go.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse); // 생성한 수류탄을 지정한 벡터로 투척
        go.GetComponent<Bomb>().time = ProjectileTime; // 터지는 시간 지정
    }
}

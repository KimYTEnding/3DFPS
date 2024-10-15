using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public GameObject ProjectilePrefab; // �߻� ������Ʈ
    public float ProjectileAngle = 30; // �߻� ����
    public float ProjectileForce = 10; // �߻� ��
    public float ProjectileTime = 5; // �߻� �ð�

    protected override void Fire()
    {
        ProjectileFire();
    }

    void ProjectileFire()
    {
        Camera cam = Camera.main; // ī�޶� �ҷ�����

        Vector3 forward = cam.transform.forward; // ī�޶� �ֽ��ϴ� ���� ����
        /// 1���
        //Vector3 up = cam.transform.up; // ī�޶��� y�� ����

        //Vector3 direction = forward + up * Mathf.Tan(ProjectileAngle * Mathf.Deg2Rad); // �ֽ� ���� + (�������� ġȯ�� �߻簢�� ź��Ʈ�� * y�� ����)

        /// 2 ���
        Vector3 direction = Quaternion.AngleAxis(-ProjectileAngle, cam.transform.right) * forward;

        direction.Normalize(); // ���� �� ����ȭ
        direction *= ProjectileForce; // �߻� ���⿡ �߻� �� ���ϱ�

        GameObject go = Instantiate(ProjectilePrefab); // ����ź ������Ʈ ����
        go.transform.position = FiringPosition.position; // ������ ����ź �ʱ� ��ġ ����
        go.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse); // ������ ����ź�� ������ ���ͷ� ��ô
        go.GetComponent<Bomb>().time = ProjectileTime; // ������ �ð� ����
    }
}

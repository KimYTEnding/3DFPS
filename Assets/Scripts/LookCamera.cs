using UnityEngine;

public class LookCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
        // 오브젝트 위치에서 카메라가 바라보는 방향으로 배치
    }
}

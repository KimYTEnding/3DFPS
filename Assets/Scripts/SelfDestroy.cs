using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float time = 0.1f;

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class GaugeColor : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Image image = GetComponent<Image>();
        GetComponent<Image>().color = Color.HSVToRGB(image.fillAmount / 3, 1.0f, 1.0f);
        // fillamount���� ���� ���� ����, �ʱⰪ�� �ʷϻ�(360/3 = 120)
    }
}

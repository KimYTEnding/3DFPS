using UnityEngine;
using UnityEngine.UI;

public class GaugeColor : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Image image = GetComponent<Image>();
        GetComponent<Image>().color = Color.HSVToRGB(image.fillAmount / 3, 1.0f, 1.0f);
        // fillamount값에 따라 색상 변경, 초기값은 초록색(360/3 = 120)
    }
}

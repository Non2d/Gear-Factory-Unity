using UnityEngine;
using UnityEngine.UI;

public class UIBossHitPoint : MonoBehaviour
{
    [SerializeField]
    private SO_GearFactory gearFactory;
    private Slider slider;
    private Image fillImage;
    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("Slider component not found!");
        }

        // FillAreaのFill部分のImageコンポーネントを取得
        if (slider.fillRect != null)
        {
            fillImage = slider.fillRect.GetComponent<Image>();
            if (fillImage == null)
            {
                Debug.LogError("Fill Image component not found!");
            }
        }
    }

    // Update is called once per frame
    public void UpdateGauge()
    {
        if (gearFactory != null && slider != null)
        {
            slider.value = gearFactory.cubeBossHp;

            // スライダーの値が0になった場合、Fill部分を非表示に
            if (slider.value <= 0 && fillImage != null)
            {
                fillImage.enabled = false;  // Fill部分を非表示にする
            }
            else if (slider.value > 0 && fillImage != null)
            {
                fillImage.enabled = true;   // Fill部分を再表示
            }
        }
    }
}

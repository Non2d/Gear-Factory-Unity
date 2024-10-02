using UnityEngine;
using UnityEngine.UI;

public class UIPlayerEnergy : MonoBehaviour
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

    public void UpdatePlayerEnergyGauge()
    {
        if (gearFactory != null && slider != null)
        {
            if (Input.GetKey(KeyCode.X))
            {
                gearFactory.playerEnergy -= 0.1f;  // テスト用のデクリメント処理
            }
            slider.value = gearFactory.playerEnergy;

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
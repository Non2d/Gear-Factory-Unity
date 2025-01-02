using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSceneController : BaseSceneController
{
    [SerializeField] private Slider CameraSensSlider;
    [SerializeField] private Slider ZoomSensSlider;
    [SerializeField] private SO_GearFactory gf;

    private const string CameraSensKey = "CameraSensitivity"; // 保存用キー
    private const string ZoomSensKey = "ZoomSensitivity"; // 保存用キー

    void Start()
    {
        // 保存された値を読み込み
        if (CameraSensSlider != null)
        {
            float savedCameraSens = PlayerPrefs.GetFloat(CameraSensKey, 0.5f); // デフォルト値0.5
            CameraSensSlider.value = savedCameraSens;
            gf.mouseSensitivity = Mathf.Lerp(100, 900, savedCameraSens);
        }

        if (ZoomSensSlider != null)
        {
            float savedZoomSens = PlayerPrefs.GetFloat(ZoomSensKey, 0.5f); // デフォルト値0.5
            ZoomSensSlider.value = savedZoomSens;
            gf.zoomSpeed = Mathf.Lerp(5, 15, savedZoomSens);
        }

        // スライダーの値変更イベントを登録
        if (CameraSensSlider != null)
        {
            CameraSensSlider.onValueChanged.AddListener(OnCameraSensChanged);
        }

        if (ZoomSensSlider != null)
        {
            ZoomSensSlider.onValueChanged.AddListener(OnZoomSensChanged);
        }
    }

    void OnCameraSensChanged(float value)
    {
        if (gf != null)
        {
            gf.mouseSensitivity = Mathf.Lerp(100, 900, value);
            PlayerPrefs.SetFloat(CameraSensKey, value); // 保存
        }
    }

    void OnZoomSensChanged(float value)
    {
        if (gf != null)
        {
            gf.zoomSpeed = Mathf.Lerp(5, 15, value);
            PlayerPrefs.SetFloat(ZoomSensKey, value); // 保存
        }
    }

    void OnDestroy()
    {
        // イベントリスナーを解除
        if (CameraSensSlider != null)
        {
            CameraSensSlider.onValueChanged.RemoveListener(OnCameraSensChanged);
        }

        if (ZoomSensSlider != null)
        {
            ZoomSensSlider.onValueChanged.RemoveListener(OnZoomSensChanged);
        }
    }
}

using UnityEngine;
using TMPro;

public class DisplayPlayerLife : MonoBehaviour
{
    [SerializeField]
    private SO_GearFactory gearFactory;
    private TextMeshProUGUI playerLifeText;

    private void Start()
    {
        // this.gameObject から TextMeshProUGUI コンポーネントを取得
        playerLifeText = this.GetComponent<TextMeshProUGUI>();

        if (playerLifeText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found!");
            return;
        }

        // イベントリスナーを追加
        if (gearFactory != null)
        {
            gearFactory.OnPlayerLifeChanged += UpdatePlayerLifeText; //C#では+-でリスナに関数をバインド/バインド解除する。UE5のDelegateやAddDynamicと同じ。
        }

        UpdatePlayerLifeText();
    }

    private void OnDestroy()
    {
        // イベントリスナーを削除
        if (gearFactory != null)
        {
            gearFactory.OnPlayerLifeChanged -= UpdatePlayerLifeText;
        }
    }

    public void UpdatePlayerLifeText()
    {
        if (gearFactory != null && playerLifeText != null)
        {
            playerLifeText.text = gearFactory.playerLife.ToString();
        }
    }
}
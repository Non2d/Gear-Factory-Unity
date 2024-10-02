using UnityEngine;
using TMPro;
using UnityEditor.SearchService;

public class UIPlayerLife : MonoBehaviour
{
    [SerializeField]
    private SO_GearFactory gearFactory;

    [SerializeField]
    private IngameSceneController sc;
    private TextMeshProUGUI playerLifeText;

    private void Start()
    {
        // this.gameObject から TextMeshProUGUI コンポーネントを取得
        playerLifeText = GetComponent<TextMeshProUGUI>();

        if (playerLifeText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found!");
            return;
        }

        // イベントリスナーを追加
        if (sc != null)
        {
            sc.OnPlayerLifeChanged += UpdatePlayerLifeText; //C#では+-でリスナに関数をバインド/バインド解除する。UE5のDelegateやAddDynamicと同じ。
        }

        UpdatePlayerLifeText();
    }

    private void OnDestroy()
    {
        // イベントリスナーを削除
        if (sc != null)
        {
            sc.OnPlayerLifeChanged -= UpdatePlayerLifeText;
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
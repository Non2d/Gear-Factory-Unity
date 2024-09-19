using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //Buttonから直接呼び出す関数はこちらで定義

    //UIのcanvasGroupを設定
    private CanvasGroup CGmenu;
    private CanvasGroup CGgameOverMenu;

    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject gameOverMenu;


    public void Start()
    {
        CGmenu = menu.GetComponent<CanvasGroup>();
        CGgameOverMenu = gameOverMenu.GetComponent<CanvasGroup>();

        CGmenu.alpha = 0; // メニューを非表示
        CGmenu.interactable = false; // メニューを操作不可能にする
        CGmenu.blocksRaycasts = false; // メニューを操作不可能にする

        CGgameOverMenu.alpha = 0; // ゲームオーバーメニューを非表示
        CGgameOverMenu.interactable = false; // ゲームオーバーメニューを操作不可能にする
        CGgameOverMenu.blocksRaycasts = false; // ゲームオーバーメニューを操作不可能にする
    }

    // シーン遷移用のメソッド
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // アプリケーション終了用のメソッド
    public void QuitApplication()
    {
#if UNITY_EDITOR
        // Unityエディタ内での動作
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ビルドされたアプリケーションでの動作
        Application.Quit();
#endif
    }

    public void Pause()
    {
        CGmenu.alpha = 1; // メニューを表示
        CGmenu.interactable = true; // メニューを操作可能にする
        CGmenu.blocksRaycasts = true; // メニューを操作可能にする
        Cursor.visible = true; // マウスカーソルを表示
        Cursor.lockState = CursorLockMode.None; // マウスカーソルのロックを解除
        Time.timeScale = 0; // 時間を停止
    }

    public void CancelPause()
    {
        CGmenu.alpha = 0; // メニューを非表示
        CGmenu.interactable = false; // メニューを操作不可能にする
        CGmenu.blocksRaycasts = false; // メニューを操作不可能にする
        Cursor.visible = false; // マウスカーソルを非表示
        Cursor.lockState = CursorLockMode.Locked; // マウスカーソルをロック
        Time.timeScale = 1; // 時間を再開
    }

    public void ResetPauseForBackToTitle()
    {
        CGmenu.alpha = 0; // メニューを非表示
        CGmenu.interactable = false; // メニューを操作不可能にする
        CGmenu.blocksRaycasts = false; // メニューを操作不可能にする
        Time.timeScale = 1; // 時間を再開
    }

    public void ShowGameOverMenu()
    {
        CGgameOverMenu.alpha = 1; // ゲームオーバーメニューを表示
        CGgameOverMenu.interactable = true; // ゲームオーバーメニューを操作可能にする
        CGgameOverMenu.blocksRaycasts = true; // ゲームオーバーメニューを操作可能にする
        Cursor.visible = true; // マウスカーソルを表示
        Cursor.lockState = CursorLockMode.None; // マウスカーソルのロックを解除
        Time.timeScale = 0; // 時間を停止
    }

    public void RestartGame()
    {
        CGgameOverMenu.alpha = 0; // ゲームオーバーメニューを非表示
        CGgameOverMenu.interactable = false; // ゲームオーバーメニューを操作不可能にする
        CGgameOverMenu.blocksRaycasts = false; // ゲームオーバーメニューを操作不可能にする
        Cursor.visible = false; // マウスカーソルを非表示
        Cursor.lockState = CursorLockMode.Locked; // マウスカーソルをロック
        Time.timeScale = 1; // 時間を再開
    }
}
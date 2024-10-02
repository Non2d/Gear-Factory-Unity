using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseSceneController : MonoBehaviour
{
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
}
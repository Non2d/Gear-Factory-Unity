using UnityEngine;
using System.IO;

public class EnvExample : MonoBehaviour
{
    void Start()
    {
        // 環境変数ファイルを読み込む
        string envPath = Path.Combine(Application.streamingAssetsPath, "config.env");
        EnvLoader.LoadEnvFile(envPath);

        // 環境変数を取得
        string apiKey = EnvLoader.GetEnv("API_KEY", "default_key");
        string serverUrl = EnvLoader.GetEnv("SERVER_URL", "https://default.com");

        Debug.Log("APIキー: " + apiKey);
        Debug.Log("サーバーURL: " + serverUrl);
    }
}

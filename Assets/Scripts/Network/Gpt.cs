using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;  // UnityWebRequestを使用するために必要
using System.Text;  // Encodingを使用するために必要

public class Gpt : MonoBehaviour
{
    private string apiUrl = "http://localhost:8000";  // FastAPIのURL

    // コルーチンでAPIリクエストを送信
    IEnumerator GetRequest()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            // リクエストの送信とレスポンスの待機
            yield return request.SendWebRequest();

            // エラーハンドリング
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // 成功した場合のレスポンス
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }

    private string apiUrl2 = "http://localhost:8000/gpt";  // FastAPIのURL

    // "Fire burning"という文字列をポスト
    IEnumerator PostRequest()
    {
        // リクエストデータをJSON形式にシリアライズ
        string jsonData = JsonUtility.ToJson(new TextRequest { text = "Fire burning" });

        // UnityWebRequestを使用してPOSTリクエストを作成
        using (UnityWebRequest request = new UnityWebRequest(apiUrl2, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // リクエストの送信とレスポンスの待機
            yield return request.SendWebRequest();

            // エラーハンドリング
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // 成功した場合のレスポンス
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(GetRequest());
        StartCoroutine(PostRequest());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // リクエストデータのクラス定義
    [System.Serializable]
    public class TextRequest
    {
        public string text;
    }
}

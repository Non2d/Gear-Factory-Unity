using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Http : MonoBehaviour
{
    private string apiUrl = "https://vps4.nkmr.io/card-meet/v1/";

    // ボタンから呼び出すメソッド
    public void OnButtonClick()
    {
        StartCoroutine(GetRequest());
    }

    // APIリクエストを実行する
    IEnumerator GetRequest()
    {
        using UnityWebRequest request = UnityWebRequest.Get(apiUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }
}

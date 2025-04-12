using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerResult
{
    public string player_name;
    public float total_time;
    public int deaths;
    public float total_energy;
}

[System.Serializable]
public class PlayerResultWrapper
{
    public PlayerResult[] results;
}

public class RankingController : BaseSceneController
{
    [SerializeField] private Transform rankViewContent;
    [SerializeField] private GameObject rankViewPrefab;

    void Start()
    {
        StartCoroutine(GetRanking());
    }

    private IEnumerator GetRanking()
    {
        string apiUrl = "https://vps4.nkmr.io/gear-factory/v1/simple-results";

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);

                // JSONをデシリアライズ
                PlayerResultWrapper wrapper = JsonUtility.FromJson<PlayerResultWrapper>(request.downloadHandler.text);
                foreach (var result in wrapper.results)
                {
                    string readableTime = $"{(int)result.total_time / 60:D2}:{(int)result.total_time % 60:D2}";
                    SpawnText(result.player_name);
                    SpawnText(readableTime);
                    SpawnText(result.deaths.ToString());
                    SpawnText(result.total_energy.ToString());
                    // Debug.Log($"Player: {result.player_name}, Total Time: {readableTime}, Deaths: {result.deaths}, Total Energy: {result.total_energy}");
                }
            }
        }
    }

    public void SpawnText(string text)
    {
        var go = Instantiate(rankViewPrefab, rankViewContent);
        go.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
    }
}
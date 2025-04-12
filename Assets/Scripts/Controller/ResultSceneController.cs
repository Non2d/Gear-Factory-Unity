using UnityEngine;
using TMPro;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ResultSceneController : BaseSceneController
{
    [SerializeField] private GameObject resultTime;
    [SerializeField] private GameObject totalResults;
    [SerializeField] private GameObject genAiAnalysis;
    [SerializeField] private SO_GearFactory gf;

    [SerializeField] private TextMeshProUGUI playerNameInput;

    TimeSpan totalDuration = new TimeSpan();
    void Start()
    {
        TextMeshProUGUI textMeshProUGUI = resultTime.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI totalResultsMeshProUGUI = totalResults.GetComponent<TextMeshProUGUI>();

        // dummy data
        foreach (string levelKey in gf.sceneNames)
        {
            if (!gf.startTimes.ContainsKey(levelKey))
            {
                gf.totalUsedEnergy = 9999999;
                gf.totalDeaths = 999999;
                gf.startTimes[levelKey] = new DateTime(2024, 12, 30, 20, 0, 0);
            }
            if (!gf.endTimes.ContainsKey(levelKey))
            {
                gf.totalUsedEnergy = 9999999;
                gf.totalDeaths = 999999;
                gf.endTimes[levelKey] = gf.startTimes[levelKey].AddSeconds(1000);
            }
        }

        textMeshProUGUI.text = "Time for each stage:\n";

        List<StageClearTime> stageClearTimes = new List<StageClearTime>();

        foreach (string levelKey in gf.sceneNames)
        {
            if (!gf.startTimes.ContainsKey(levelKey) || !gf.endTimes.ContainsKey(levelKey))
            {
                continue;
            }
            TimeSpan duration = gf.endTimes[levelKey] - gf.startTimes[levelKey];
            totalDuration += duration;
            string displayName = levelKey.Replace("Level01", "Stage");
            textMeshProUGUI.text += $"{displayName} - {duration.Minutes:D2} : {duration.Seconds:D2} : {duration.Milliseconds:D3} \n";

            stageClearTimes.Add(new StageClearTime
            {
                stage_name = displayName,
                clear_time = duration.TotalSeconds
            });
        }
        totalResultsMeshProUGUI.text = $"Total Time  - {totalDuration.Minutes:D2}:{totalDuration.Seconds:D2}.{totalDuration.Milliseconds:D3}\n";
        totalResultsMeshProUGUI.text += $"Death       - {gf.totalDeaths}\n";
        totalResultsMeshProUGUI.text += $"Used Energy - {Mathf.FloorToInt(gf.totalUsedEnergy / 30)}\n";

        SetGenAiAnalysis(stageClearTimes);
    }

    public void SetGenAiAnalysis(List<StageClearTime> stageClearTimes)
    {
        StartCoroutine(GetGenAiAnalysisCoroutine(stageClearTimes));
    }

    private IEnumerator GetGenAiAnalysisCoroutine(List<StageClearTime> StageClearTimes)
    {
        string apiUrl = "https://vps4.nkmr.io/gear-factory/v1/analyze";

        // 送信するデータのクラスを定義
        var analysisData = new AnalysisData
        {
            player_name = "not_used",
            total_time = totalDuration.TotalSeconds,
            deaths = gf.totalDeaths,
            total_energy = Mathf.FloorToInt(gf.totalUsedEnergy / 30),
            stage_clear_times = StageClearTimes
        };

        string jsonData = JsonUtility.ToJson(analysisData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                string cleaned_response_text = request.downloadHandler.text.Replace("\"", "");
                genAiAnalysis.GetComponent<TextMeshProUGUI>().text = cleaned_response_text;
            }
        }
    }

    public void SendResult()
    {
        StartCoroutine(SendResultCoroutine());
    }

    private IEnumerator SendResultCoroutine()
    {
        string apiUrl = "https://vps4.nkmr.io/gear-factory/v1/results";

        string sendName = string.IsNullOrEmpty(playerNameInput.text) ? "NoName" : playerNameInput.text;

        // 送信するデータのクラスを定義
        var resultData = new ResultData
        {
            player_name = sendName, // プレイヤー名を設定
            total_time = totalDuration.TotalSeconds, // 総プレイ時間
            deaths = gf.totalDeaths,
            total_energy = Mathf.FloorToInt(gf.totalUsedEnergy / 100),
            stage_clear_times = new List<StageClearTime>()
        };

        // 各ステージのクリアタイムを追加
        foreach (var kvp in gf.startTimes)
        {
            if (gf.endTimes.ContainsKey(kvp.Key))
            {
                var clearTime = (gf.endTimes[kvp.Key] - kvp.Value).TotalSeconds;
                resultData.stage_clear_times.Add(new StageClearTime
                {
                    stage_name = kvp.Key,
                    clear_time = (int)clearTime
                });
            }
        }

        string jsonData = JsonUtility.ToJson(resultData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Result sent successfully.");
            }
        }
    }
}

// 送信するデータのクラス定義
[Serializable]
public class ResultData
{
    public string player_name;
    public double total_time;
    public int deaths;
    public int total_energy;
    public string groq_analysis;
    public List<StageClearTime> stage_clear_times;
}

[Serializable]
public class AnalysisData
{
    public string player_name;
    public double total_time;
    public int deaths;
    public int total_energy;
    public List<StageClearTime> stage_clear_times;
}

[Serializable]
public class StageClearTime
{
    public string stage_name;
    public double clear_time;
}
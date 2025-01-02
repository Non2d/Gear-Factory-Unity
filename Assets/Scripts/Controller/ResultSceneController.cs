using UnityEngine;
using TMPro;
using System;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;

public class ResultSceneController : BaseSceneController
{
    [SerializeField] private GameObject resultTime;
    [SerializeField] private GameObject totalResults;
    [SerializeField] private SO_GearFactory gf;

    [SerializeField] private TextMeshProUGUI playerNameInput;

    TimeSpan totalDuration = new TimeSpan();
    void Start()
    {
        TextMeshProUGUI textMeshProUGUI = resultTime.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI totalResultsMeshProUGUI = totalResults.GetComponent<TextMeshProUGUI>();
        // dummy data
        // foreach (string levelKey in gf.sceneNames)
        // {
        //     gf.startTimes[levelKey] = new DateTime(2024, 12, 30, 20, 0, 0);
        //     gf.endTimes[levelKey] = gf.startTimes[levelKey].AddSeconds(1000);
        // }

        textMeshProUGUI.text = "Time for each stage:\n";
        
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
        }
        totalResultsMeshProUGUI.text = $"Total Time 	- {totalDuration.Minutes:D2} : {totalDuration.Seconds:D2} : {totalDuration.Milliseconds:D3}\n";
        totalResultsMeshProUGUI.text += $"Death 		- {gf.totalDeaths}\n";
        totalResultsMeshProUGUI.text += $"Used Energy	- {Mathf.FloorToInt(gf.totalUsedEnergy / 100)}\n";
    }

    public async void SendResult()
    {
        // string apiUrl = "http://localhost:7791/results";
        string apiUrl = "https://vps4.nkmr.io/gear-factory/v1/results";

        string sendName = "";
        if (playerNameInput.text == "")
        {
            sendName = "NoName";
        }
        else
        {
            sendName = playerNameInput.text;
        }

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

        using (HttpClient client = new HttpClient())
        {
            try
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Result sent successfully.");
                }
                else
                {
                    Debug.LogError("Error sending result: " + response.ReasonPhrase);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("Request exception: " + e.Message);
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
public class StageClearTime
{
    public string stage_name;
    public int clear_time;
}
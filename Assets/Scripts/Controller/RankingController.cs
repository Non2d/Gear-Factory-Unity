using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

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

    async void Start()
    {
        var results = await GetRanking();
        foreach (var result in results)
        {
            string readableTime = $"{(int)result.total_time / 60:D2}:{(int)result.total_time % 60:D2}";
            SpawnText(result.player_name);
            SpawnText(readableTime);
            SpawnText(result.deaths.ToString());
            SpawnText(result.total_energy.ToString());
            // Debug.Log($"Player: {result.player_name}, Total Time: {readableTime}, Deaths: {result.deaths}, Total Energy: {result.total_energy}");
        }
    }

    public async Task<PlayerResult[]> GetRanking()
    {
        // string apiUrl = "http://localhost:7791/simple-results";
        string apiUrl = "https://vps4.nkmr.io/gear-factory/v1/simple-results";

        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(apiUrl);
            var json = await response.Content.ReadAsStringAsync();

            // JSONをデシリアライズ
            PlayerResultWrapper wrapper = JsonUtility.FromJson<PlayerResultWrapper>(json);
            return wrapper.results;
        }
    }

    public void SpawnText(string text)
    {
        var go = Instantiate(rankViewPrefab, rankViewContent);
        go.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
    }
}

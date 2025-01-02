using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class IngameSceneController : BaseSceneController
{
    //thisの読み込み
    IngameSceneController sc;

    //Playerの読み込み
    [SerializeField] private GameObject player;

    //Scriptable Objectの読み込み
    [SerializeField] private SO_GearFactory gf;

    // Gaugeの読み込み
    [SerializeField] private UIPlayerEnergy playerEnergyGauge;

    // ギャンブルモード化の設定
    public bool isGamblingMode = false;
    public float dpsMultiplier = 1.0f;

    //UIのcanvasGroupを設定
    private CanvasGroup CGmenu;
    private CanvasGroup CGgameOverMenu;

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject SpawnPoint;

    // events
    public event Action OnPlayerLifeChanged;

    private bool isDead = false;

    private SpherePlayer sp;

    private void Awake()
    {
        //thisの読み込み
        sc = GetComponent<IngameSceneController>();

        // CanvasGroupコンポーネントの参照を取得
        CGmenu = menu.GetComponent<CanvasGroup>();
        CGgameOverMenu = gameOverMenu.GetComponent<CanvasGroup>();
    }

    public void Start()
    {
        InitializeGame();
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Level0101")
        {
            gf.startTimes.Clear();
            gf.endTimes.Clear();
            gf.totalDeaths = 0;
            gf.totalUsedEnergy = 0;
        }
        if (!gf.startTimes.ContainsKey(currentSceneName)){
            gf.startTimes.Add(currentSceneName, DateTime.Now);
        }
    }

    public void Update()
    {
        // Dictionaryの内容を文字列に変換してログに出力
        // string startTimesString = string.Join(", ", gf.startTimes.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        // Debug.Log("Start Times: " + startTimesString);
        string endTimesString = string.Join(", ", gf.endTimes.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        Debug.Log("End Times: " + endTimesString);

#if UNITY_EDITOR
        for (int i = 1; i <= 6; i++)
        {
            KeyCode keyCode = KeyCode.Alpha0 + i;
            if (Input.GetKeyDown(keyCode))
            {
                CheatLoadScene(i);
                break; //小さい数字優先
            }
        }
#endif
    }

    /// <summary>
    /// ゲームの初期化
    /// </summary>
    private void InitializeGame()
    {
        sp = player.GetComponent<SpherePlayer>();
        sp.dpsMultiplier = dpsMultiplier; // ダメージ量をステージごとに調整
        gf.Initialize();
        RespawnPlayer();

        Time.timeScale = 1;

        OnPlayerLifeChanged?.Invoke();

        InitializeCanvasGroup(CGmenu);
        InitializeCanvasGroup(CGgameOverMenu);
    }

    /// <summary>
    /// CanvasGroupの初期化
    /// </summary>
    /// <param name="canvasGroup">Canvasゲームオブジェクトの子コンポーネント</param>
    private void InitializeCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    //UI制御関連
    /// <summary>
    /// ポーズ処理
    /// </summary>
    public void Pause()
    {
        ShowCanvasGroup(CGmenu);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    /// <summary>
    /// ポーズ解除処理
    /// </summary>
    public void CancelPause()
    {
        HideCanvasGroup(CGmenu);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    /// <summary>
    /// ゲームオーバーメニューを表示し、プレイヤーを無効化、爆死エフェクトを再生
    /// </summary>
    public void HandleGameOver()
    {
        ShowCanvasGroup(CGgameOverMenu);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        HideAndFixPlayer();
        sp.ExplodeEffect();
    }

    /// <summary>
    /// CanvasGroupを表示する
    /// </summary>
    /// <param name="canvasGroup"></param>
    private void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// CanvasGroupを非表示にする
    /// </summary>
    /// <param name="canvasGroup"></param>
    private void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// ゲームを再開する。残機のUIへの通知も行う
    /// </summary>
    public void RestartGame()
    {
        gf.Initialize();
        RespawnPlayer();
        OnPlayerLifeChanged?.Invoke();
        HideCanvasGroup(CGgameOverMenu);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    /// <summary>
    /// プレイヤーにダメージを与える。エネルギーが0以下になった場合、プレイヤーをデスさせる
    /// </summary>
    /// <param name="damage"></param>
    public void GivePlayerDamage(float damage)
    {
        gf.playerEnergy -= damage;
        gf.totalUsedEnergy += damage; // 即死の場合はエネルギー消費とはみなさない
        playerEnergyGauge.UpdatePlayerEnergyGauge();

        if (gf.playerEnergy <= 0)
        {
            HandlePlayerDeath();
        }
    }

    /// <summary>
    /// プレイヤーのエネルギーが0になった場合、ゲームオーバー処理を行う
    /// </summary>
    public void HandlePlayerDeath()
    {
        if (isDead) return; // Delay中に再度死亡処理が呼ばれるのを防ぐ
        isDead = true;

        gf.totalDeaths++;

        if (gf.playerLife <= 0)
        {
            sc?.HandleGameOver();
        }
        else
        {
            gf.playerLife--;
            OnPlayerLifeChanged?.Invoke();
            StartCoroutine(RespawnPlayerDelayed(3.0f));
        }
    }

    /// <summary>
    /// プレイヤーを【delay】秒後にリスポーンさせる
    /// </summary>
    /// <param name="delay"></param>
    private IEnumerator RespawnPlayerDelayed(float delay)
    {
        HideAndFixPlayer();
        sp.ExplodeEffect();
        yield return new WaitForSeconds(delay);
        RespawnPlayer();
    }

    /// <summary>
    /// プレイヤーを非表示にし、プレイヤーのコンポーネントを無効化する。座標も固定する。
    /// </summary>
    private void HideAndFixPlayer()
    {
        var renderer = player.GetComponent<Renderer>();
        var collider = player.GetComponent<Collider>();
        var rigidbody = player.GetComponent<Rigidbody>();
        var spherePlayer = player.GetComponent<SpherePlayer>();

        renderer.enabled = false;
        collider.enabled = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        spherePlayer.SetInputEnabled(false);
    }

    /// <summary>
    /// プレイヤーをリスポーンさせる。
    /// </summary>
    private void RespawnPlayer()
    {
        var renderer = player.GetComponent<Renderer>();
        var collider = player.GetComponent<Collider>();
        var rigidbody = player.GetComponent<Rigidbody>();
        var spherePlayer = player.GetComponent<SpherePlayer>();

        renderer.enabled = true;
        collider.enabled = true;
        rigidbody.constraints = RigidbodyConstraints.None;
        spherePlayer.SetInputEnabled(true); //循環参照になってるので、後でイベントかエフェクトをこっちで直接取得して解決する
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        gf.playerEnergy = gf.initPlayerEnergy;
        playerEnergyGauge.UpdatePlayerEnergyGauge();
        isDead = false;

        player.transform.position = SpawnPoint.transform.position;
        player.transform.rotation = SpawnPoint.transform.rotation;
    }


    /// <summary>
    /// プレゼン用のチート関数
    /// </summary>
    public void CheatLoadScene(int sceneIndex)
    {
        string nextSceneName = "Level010" + sceneIndex.ToString();
        SceneManager.LoadScene(sceneIndex);
    }
}
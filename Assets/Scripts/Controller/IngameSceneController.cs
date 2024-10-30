using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IngameSceneController : BaseSceneController
{
    //thisの読み込み
    IngameSceneController sc;

    //Playerの読み込み
    [SerializeField]
    private GameObject player;

    //Scriptable Objectの読み込み
    [SerializeField]
    private SO_GearFactory gf;

    // Gaugeの読み込み
    [SerializeField]
    private UIPlayerEnergy playerEnergyGauge;

    //UIのcanvasGroupを設定
    private CanvasGroup CGmenu;
    private CanvasGroup CGgameOverMenu;

    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject SpawnPoint;

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

    public void Start() //原則：直接Sceneを開いても、タイトル画面のPlayボタン経由でも、全く同じ挙動になること！...と思ったけど、最初からと続きからでは異なるか。Playはとりあえず「最初から」という扱いで。
    {
        //初期値設定
        sp = player.GetComponent<SpherePlayer>();

        //初期化
        gf.Initialize();//プレイヤーのステータス等を初期化
        RespawnPlayer();

        Time.timeScale = 1; //明示的に初期化しないと遷移後動かないことも

        //UI関連
        OnPlayerLifeChanged?.Invoke(); //プレイヤー残機の初期化をUIに反映

        CGmenu.alpha = 0; //ポーズメニューの初期設定
        CGmenu.interactable = false;
        CGmenu.blocksRaycasts = false;

        CGgameOverMenu.alpha = 0; // ゲームオーバーメニューの初期設定
        CGgameOverMenu.interactable = false;
        CGgameOverMenu.blocksRaycasts = false;
    }

    //UI制御関連
    public void Pause()
    {
        CGmenu.alpha = 1; // メニューを表示、操作可能に
        CGmenu.interactable = true;
        CGmenu.blocksRaycasts = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0; // 時間を停止
    }

    public void CancelPause()
    {
        CGmenu.alpha = 0; // メニューを非表示、操作不可能に
        CGmenu.interactable = false;
        CGmenu.blocksRaycasts = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1; // 時間を再開
    }

    public void ShowGameOverMenu()
    {
        CGgameOverMenu.alpha = 1; // ゲームオーバーメニューを表示・操作可能に
        CGgameOverMenu.interactable = true;
        CGgameOverMenu.blocksRaycasts = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Time.timeScale = 0; // 時間を停止
        HideAndFixPlayer();
        sp.ExplodeEffect();
    }

    public void ResetPauseForBackToTitle()
    {
        CGmenu.alpha = 0; // メニューを非表示、操作不可能に
        CGmenu.interactable = false;
        CGmenu.blocksRaycasts = false;

        Time.timeScale = 1; // 時間を再開
    }

    /// <summary>
    /// ゲームをリスタートする．ここではメニューの初期化を書いている．
    /// </summary>
    public void RestartGame()
    {
        
        gf.Initialize();//プレイヤーのステータス等を初期化
        RespawnPlayer();

        OnPlayerLifeChanged?.Invoke(); //プレイヤー残機の初期化をUIに反映

        CGgameOverMenu.alpha = 0; // ゲームオーバーメニューの初期設定
        CGgameOverMenu.interactable = false;
        CGgameOverMenu.blocksRaycasts = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1; // 時間を再開
    }

    /// <summary>
    /// プレイヤーにダメージを与える．エネルギが0以下になったらデス処理を行う．
    /// </summary>
    public void GivePlayerDamage(float damage)
    {
        gf.playerEnergy -= damage;
        playerEnergyGauge.UpdatePlayerEnergyGauge();

        if (gf.playerEnergy <= 0)
        {
            HandlePlayerDeath();
        }
    }

    /// <summary>
    /// プレイヤーの残機を減らし，3秒後にリスポーンさせる．残機が0以下になったらゲームオーバー画面を表示する．
    /// </summary>
    public void HandlePlayerDeath()
    {
        if(isDead){
            return;
        }
        isDead = true;

        if (gf.playerLife <= 0)
        {
            if (sc != null)
            {
                sc.ShowGameOverMenu();
            }
            else
            {
                Debug.LogError("SceneController component not found on sceneController GameObject.");
            }
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
    private IEnumerator RespawnPlayerDelayed(float delay)
    {
        HideAndFixPlayer();
        sp.ExplodeEffect(); //循環参照気味なので注意...あとでイベントか、シンプルにエフェクトをすべてSceneControllerに移す
        yield return new WaitForSeconds(delay);
        RespawnPlayer();
    }

    /// <summary>
    /// プレイヤーを非表示にする
    /// </summary>
    private void HideAndFixPlayer()
    {
        player.GetComponent<Renderer>().enabled = false;
        player.GetComponent<Collider>().enabled = false;
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        player.GetComponent<SpherePlayer>().SetInputEnabled(false);
    }

    /// <summary>
    /// プレイヤーのステータスをリセットし、リスポーンさせる
    /// </summary>
    private void RespawnPlayer()
    {
        //Reset status
        player.GetComponent<Renderer>().enabled = true;
        player.GetComponent<Collider>().enabled = true;
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        player.GetComponent<SpherePlayer>().SetInputEnabled(true);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        gf.playerEnergy = gf.initPlayerEnergy;
        playerEnergyGauge.UpdatePlayerEnergyGauge();
        isDead = false;

        //Respawn
        Vector3 playerSpawnPosition = SpawnPoint.transform.position;
        Quaternion playerSpawnRotation = SpawnPoint.transform.rotation;
        player.transform.position = playerSpawnPosition;
        player.transform.rotation = playerSpawnRotation;
    }
}
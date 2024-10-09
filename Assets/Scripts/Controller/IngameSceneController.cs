using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameSceneController : BaseSceneController
{
    //thisの読み込み
    IngameSceneController sc;

    //Playerの読み込み
    [SerializeField]
    private GameObject player;

    //Scriptable Objectの読み込み
    [SerializeField]
    private SO_GearFactory gearFactory;

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
        //初期化
        gearFactory.Initialize();//プレイヤーのステータス等を初期化
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

        Time.timeScale = 0; // 時間を停止
    }

    public void ResetPauseForBackToTitle()
    {
        CGmenu.alpha = 0; // メニューを非表示、操作不可能に
        CGmenu.interactable = false;
        CGmenu.blocksRaycasts = false;

        Time.timeScale = 1; // 時間を再開
    }

    public void RestartGame()
    {
        gearFactory.Initialize();//プレイヤーのステータス等を初期化
        RespawnPlayer();

        OnPlayerLifeChanged?.Invoke(); //プレイヤー残機の初期化をUIに反映

        CGgameOverMenu.alpha = 0; // ゲームオーバーメニューの初期設定
        CGgameOverMenu.interactable = false;
        CGgameOverMenu.blocksRaycasts = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1; // 時間を再開
    }

    //Player制御ロジック
    public void GivePlayerDamage(float damage)
    {
        gearFactory.playerEnergy -= damage;
        playerEnergyGauge.UpdatePlayerEnergyGauge();

        if (gearFactory.playerEnergy <= 0)
        {
            HandlePlayerDeath();
        }
    }

    public void HandlePlayerDeath()
    {
        if (gearFactory.playerLife <= 0)
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
            gearFactory.playerLife--;
            OnPlayerLifeChanged?.Invoke();
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        //Reset status
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        gearFactory.playerEnergy = gearFactory.PlayerEnergyMax;
        playerEnergyGauge.UpdatePlayerEnergyGauge();

        //Respawn
        Vector3 playerSpawnPosition = SpawnPoint.transform.position;
        Quaternion playerSpawnRotation = SpawnPoint.transform.rotation;
        player.transform.position = playerSpawnPosition;
        player.transform.rotation = playerSpawnRotation;
    }
}
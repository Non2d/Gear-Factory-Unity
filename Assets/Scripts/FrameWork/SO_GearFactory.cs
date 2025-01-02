using UnityEngine;
using System;
using System.Collections.Generic; 

[CreateAssetMenu(menuName = "SO_GearFactory")]
//ここでは、データとシンプルな初期化関数のみ扱う。
public class SO_GearFactory : ScriptableObject
{
    //initial values
    public int initPlayerLife;

    public float initPlayerEnergy;
    
    public float PlayerEnergyMax;

    //variable values
    public int playerLife;
    public float playerEnergy;

    public string AnimStateChase = "CB_Chase";
    public string AnimConditionAttack = "IsAttacking";

    public float cubeBossHpMax;
    public float cubeBossHp;

    public float mouseSensitivity;
    public float mouseSensitivityMax;
    public float mouseSensitivityMin;
    public float zoomSpeed;
    public float zoomSpeedMax;
    public float zoomSpeedMin;

    public int totalDeaths = 0;
    public float totalUsedEnergy = 0;

    public string[] sceneNames = { "Level0101", "Level0102", "Level0103", "Level0104", "Level0105", "Level0106" };
    
    // Dictionary to store level names with DateTime
    public Dictionary<string, DateTime> startTimes = new();
    public Dictionary<string, DateTime> endTimes = new();

    // public void Setup()
    // {
    //     mouseSensitivity = Mathf.Lerp(mouseSensitivityMin, mouseSensitivityMax, 0.5f);
    //     zoomSpeed = Mathf.Lerp(zoomSpeedMin, zoomSpeedMax, 0.5f);
    // }

    public void Initialize()
    {
        playerLife = initPlayerLife;
        playerEnergy = initPlayerEnergy;
        cubeBossHp = cubeBossHpMax;
    }
}
using UnityEngine;
using System;

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

    public void Initialize()
    {
        playerLife = initPlayerLife;
        playerEnergy = initPlayerEnergy;
        cubeBossHp = cubeBossHpMax;
    }
}
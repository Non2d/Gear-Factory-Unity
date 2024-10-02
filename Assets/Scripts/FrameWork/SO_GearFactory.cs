using UnityEngine;
using System;

[CreateAssetMenu(menuName = "SO_GearFactory")]
//ここでは、データとシンプルな初期化関数のみ扱う。
public class SO_GearFactory : ScriptableObject
{
    //initial values
    [SerializeField]
    private int initPlayerLife;
    [SerializeField]
    private GameObject initPlayerSpawner;
    [SerializeField]
    private float initPlayerEnergy;
    
    public float PlayerEnergyMax;

    //variable values
    public int playerLife;
    public Vector3 playerSpawnPosition;
    public Quaternion playerSpawnRotation;
    public float playerEnergy;

    public void Initialize()
    {
        playerLife = initPlayerLife;
        playerSpawnPosition = initPlayerSpawner.transform.position;
        playerSpawnRotation = initPlayerSpawner.transform.rotation;
        playerEnergy = initPlayerEnergy;
    }
}
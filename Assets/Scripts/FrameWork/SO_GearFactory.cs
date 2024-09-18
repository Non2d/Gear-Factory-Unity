using UnityEngine;
using System;

[CreateAssetMenu(menuName = "SO_GearFactory")]
public class SO_GearFactory : ScriptableObject
{
    public event Action OnPlayerLifeChanged;

    //initial values
    [SerializeField]
    private int initialPlayerLife = 3;

    //variable values
    private int _playerLife;
    public int playerLife
    {
        get { return _playerLife; }
        set
        {
            if (_playerLife != value)
            {
                _playerLife = value;
                OnPlayerLifeChanged?.Invoke();
            }
        }
    }

    [SerializeField]
    private GameObject respawnPoint;

    public void InitializePlayerLife()
    {
        _playerLife = initialPlayerLife;
        OnPlayerLifeChanged?.Invoke();
    }

    public void HandlePlayerDeath()
    {
        if (_playerLife <= 0)
        {
            //game over
        }
        else
        {
            _playerLife--;
            OnPlayerLifeChanged?.Invoke();
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        if (respawnPoint != null)
        {
            GameObject player = GameObject.Find("SpherePlayer");
            //Reset status
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            //Respawn
            GameObject respawnPoint = GameObject.Find("RespawnPoint");
            player.transform.position = respawnPoint.transform.position;
            player.transform.rotation = respawnPoint.transform.rotation;
        }
        else
        {
            Debug.LogError("Respawn point is not set.");
        }
    }
}
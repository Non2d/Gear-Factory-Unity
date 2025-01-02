using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Goal : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool isFinalGoal = false;
    [SerializeField] private SO_GearFactory gf;

    private void OnTriggerEnter(Collider other)
    {
        if (isFinalGoal)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(nextSceneName);
            string currentSceneName = SceneManager.GetActiveScene().name;
            gf.endTimes[currentSceneName] = DateTime.Now;
        }
    }
}
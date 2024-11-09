using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool isFinalGoal = false;

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
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToLevel1 : MonoBehaviour
{
    public string sceneToLoad = "LEVEL 2";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
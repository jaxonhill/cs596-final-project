using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadNextScene : MonoBehaviour
{
    private const string DefaultWinSceneName = "YouWin";

    [SerializeField] private string nextSceneName = "LEVEL 1";
    [SerializeField] private AudioSource teleportSound;

    private bool isLoading = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isLoading) return;

        if (other.CompareTag("Player"))
        {
            isLoading = true;
            StartCoroutine(PlaySoundThenLoad());
        }
    }

    private IEnumerator PlaySoundThenLoad()
    {
        if (teleportSound != null)
        {
            teleportSound.Play();
            yield return new WaitForSeconds(teleportSound.clip.length);
        }

        string sceneToLoad = string.IsNullOrWhiteSpace(nextSceneName) ? DefaultWinSceneName : nextSceneName;
        SceneManager.LoadScene(sceneToLoad);
    }
}

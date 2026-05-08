using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TriInspector;

public class LoadNextScene : MonoBehaviour
{
    [SerializeField, Scene] private string nextSceneName = "showcase-scene";
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

        SceneManager.LoadScene(nextSceneName);
    }
}
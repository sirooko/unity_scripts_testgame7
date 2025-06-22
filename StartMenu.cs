using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class StartMenu : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioSource audioSource;

    public void OnStartClicked()
    {
        StartCoroutine(PlayAndLoadScene());
    }
    IEnumerator PlayAndLoadScene()
    {
        PlayClickSound();
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene("GameScene");
    }

    public void OnQuitClicked()
    {
        PlayClickSound();
        Application.Quit();
    }

    void PlayClickSound()
    {
        //Debug.Log("Click sound triggered");
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            //Debug.LogWarning("AudioSource or Clip is missing!");
        }
    }
}

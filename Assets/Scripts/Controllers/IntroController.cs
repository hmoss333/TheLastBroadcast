using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] string sceneToLoad;


    void Start()
    {
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        videoPlayer.Play();

        yield return new WaitForSeconds(0.5f);

        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}

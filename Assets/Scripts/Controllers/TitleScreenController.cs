using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreenController : MonoBehaviour
{
    [SerializeField] AudioSource staticSource;
    [SerializeField] AudioSource musicSource;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI quoteText, sigText;

    [SerializeField] float startDelayTime, musicDelayTime, sigDelayTime, titleDisplayTime, sceneDelayTime; 


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TitleScreenRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TitleScreenRoutine()
    {
        staticSource.volume = 1;
        staticSource.Play();

        yield return new WaitForSeconds(startDelayTime);

        musicSource.volume = 0;
        musicSource.Play();

        //Update Audio
        staticSource.volume = Mathf.Lerp(1, 0.5f, 3f);
        musicSource.volume = Mathf.Lerp(0, 1, 3f);

        while (quoteText.color.a < 1)
            quoteText.color = new Color(quoteText.color.r, quoteText.color.g, quoteText.color.b, Mathf.Lerp(0, 1, 4.5f));

        yield return new WaitForSeconds(sigDelayTime);

        while (sigText.color.a < 1)
            sigText.color = new Color(sigText.color.r, sigText.color.g, sigText.color.b, Mathf.Lerp(0, 1, 4.5f));

        yield return new WaitForSeconds(3f);

        while (quoteText.color.a > 0 && sigText.color.a > 0)
        {
            quoteText.color = new Color(quoteText.color.r, quoteText.color.g, quoteText.color.b, Mathf.Lerp(1, 0, 3f));
            sigText.color = new Color(sigText.color.r, sigText.color.g, sigText.color.b, Mathf.Lerp(1, 0, 3f));
        }

        yield return new WaitForSeconds(musicDelayTime);

        //Update Audio
        staticSource.volume = Mathf.Lerp(0.5f, 1f, 3f);
        musicSource.volume = Mathf.Lerp(1, 0, 3f);

        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, Mathf.Lerp(0, 1, 3f));

        yield return new WaitForSeconds(titleDisplayTime);

        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, Mathf.Lerp(1, 0, 3f));

        yield return new WaitForSeconds(sceneDelayTime);

        SceneManager.LoadSceneAsync("RadioRoom");
    }
}

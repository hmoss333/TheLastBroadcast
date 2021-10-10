using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public static FadeController instance;

    [SerializeField] Image objectToFade;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartFade(float aValue, float aTime)
    {
        StartCoroutine(FadeTo(aValue, aTime));
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = objectToFade.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue, t));
            objectToFade.color = newColor;
            yield return null;
        }
    }
}

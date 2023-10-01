using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextWriter : MonoBehaviour
{
    public static TextWriter instace;

    [SerializeField] TMP_Text uiText;
    [SerializeField] float timeBtwnChars;
    Coroutine typeTextRoutine;
    public bool isTyping { get; private set; }
    public int maxChars;


    private void Awake()
    {
        if (instace == null)
            instace = this;
        else
            Destroy(this);
    }

    public void TypeText()
    {
        typeTextRoutine = StartCoroutine(TextVisible());
    }

    // Update is called once per frame
    IEnumerator TextVisible()
    {
        int totalVisibleCharacters = uiText.text.Length;
        int counter = 0;
        isTyping = true;
        uiText.ForceMeshUpdate();

        while (isTyping)
        {
            int visibleCount = counter;
            uiText.maxVisibleCharacters = visibleCount;
            maxChars = visibleCount;

            if (visibleCount >= totalVisibleCharacters)
            {                
                break;
            }
            else
            {
                yield return new WaitForSeconds(timeBtwnChars);
            }

            counter += 1;
        }

        isTyping = false;
        typeTextRoutine = null;
    }

    public void StopTyping()
    {
        StopCoroutine(typeTextRoutine);
        isTyping = false;
        uiText.maxVisibleCharacters = uiText.text.Length;
        uiText.ForceMeshUpdate();
    }
}

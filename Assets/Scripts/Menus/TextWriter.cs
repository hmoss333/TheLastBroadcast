using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextWriter : MonoBehaviour
{
    [SerializeField] TMP_Text uiText;
    [SerializeField] float timeBtwnChars;
    Coroutine typeTextRoutine;

    public void TypeText()
    {
        print("Start typing text routine");
        if (typeTextRoutine == null)
            typeTextRoutine = StartCoroutine(TextVisible());
    }

    // Update is called once per frame
    IEnumerator TextVisible()
    {
        uiText.ForceMeshUpdate();
        int totalVisibleCharacters = uiText.text.Length;//.textInfo.characterCount;
        int counter = 0;

        while (true)
        {
            int visibleCount = counter;//counter % (totalVisibleCharacters + 1);
            uiText.maxVisibleCharacters = visibleCount;
            print("Max visible characters: " + uiText.maxVisibleCharacters);

            if (visibleCount >= totalVisibleCharacters)
            {                
                break;
            }

            counter += 1;
            yield return new WaitForSeconds(timeBtwnChars);
        }

        typeTextRoutine = null;
    }
}

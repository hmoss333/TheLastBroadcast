using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [Header("Lore Variables")]
    [SerializeField] GameObject loreObject;
    [SerializeField] Image loreBackground;
    [SerializeField] TextMeshProUGUI loreText;

    [Header("Ability Variables")]
    [SerializeField] GameObject abilityObject, dialogueObject;
    [SerializeField] Image abilityBackground;
    [SerializeField] TextMeshProUGUI abilityText, dialogueText;
    [SerializeField] Image abilityIcon;

    Coroutine df;

    bool uiActive;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void ToggleLoreUI(string text)
    {
        uiActive = !uiActive;
        loreText.text = text;
        loreObject.SetActive(uiActive);
    }

    public void ToggleAbilityUI(string text, Sprite icon)
    {
        uiActive = !uiActive;
        abilityText.text = text;
        abilityIcon.sprite = icon;
        abilityObject.SetActive(uiActive);
    }

    public void DialogueUI(string text)//, float fadeTime)
    {
        dialogueText.text = text;
        dialogueObject.SetActive(true);

        //if (df != null)
        //    StopCoroutine(df);
        //df = StartCoroutine(DialogueFade(fadeTime));
    }

    public void FadeUI(float fadeTime)
    {
        if (df != null)
            StopCoroutine(df);
        df = StartCoroutine(DialogueFade(fadeTime));
    }

    IEnumerator DialogueFade(float time)
    {
        yield return new WaitForSeconds(time);

        dialogueObject.SetActive(false);
    }
}

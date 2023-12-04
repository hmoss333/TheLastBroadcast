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
    [SerializeField] TextMeshProUGUI loreText, loreTitle;

    [Header("Ability Variables")]
    [SerializeField] GameObject abilityObject;
    [SerializeField] Image abilityBackground, abilityIcon;
    [SerializeField] TextMeshProUGUI abilityText;

    [Header("Dialogue Variables")]
    [SerializeField] Image dialogueBackground;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Image inputIcon;
    float pulseTime = 0.5f;
    float inputAlpha;

    [Header("Inventory Variables")]
    [SerializeField] Image currentItem; 


    bool uiActive;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        inputAlpha = Mathf.PingPong(Time.time * pulseTime, 1f);
        Color tempColor = new Color(inputIcon.color.r, inputIcon.color.g, inputIcon.color.b, inputAlpha);
        inputIcon.color = tempColor;
    }

    public void ToggleLoreUI(bool value)
    {
        loreObject.SetActive(value);
    }

    public void SetLoreText(string text, string title)
    {
        loreText.text = text;
        loreTitle.text = title;
    }

    public void ToggleAbilityUI(string text, Sprite icon)
    {
        //uiActive = !uiActive;
        abilityText.text = text;
        abilityIcon.sprite = icon;
        abilityObject.SetActive(!abilityObject.activeSelf); //uiActive
    }

    public void SetDialogueText(string text, bool typeText)
    {
        dialogueText.text = text;
        if (typeText) { TextWriter.instace.TypeText(); }
        else { dialogueText.maxVisibleCharacters = text.Length; }
    }

    public void ToggleDialogueUI(bool value)
    {
        dialogueText.gameObject.SetActive(value);
        StartCoroutine(FadeTo(value ? 1f : 0f, 0.65f));
    }

    public void ToggleDialogueInputIcon(bool value)
    {
        inputIcon.gameObject.SetActive(value);
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = dialogueBackground.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue, t));
            dialogueBackground.color = newColor;
            yield return null;
        }
    }
}

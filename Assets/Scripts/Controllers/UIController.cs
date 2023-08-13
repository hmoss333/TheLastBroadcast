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

    bool uiActive;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleLoreUI(string text, string title)
    {
        uiActive = !uiActive;
        loreText.text = text;
        loreTitle.text = title;
        loreObject.SetActive(uiActive);
    }

    public void ToggleAbilityUI(string text, Sprite icon)
    {
        uiActive = !uiActive;
        abilityText.text = text;
        abilityIcon.sprite = icon;
        abilityObject.SetActive(uiActive);
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    public void ToggleDialogueUI(bool forceValue)
    {
        dialogueText.gameObject.SetActive(forceValue);
        StartCoroutine(FadeTo(forceValue ? 1f : 0f, 0.65f));
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

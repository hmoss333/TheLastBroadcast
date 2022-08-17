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
    [SerializeField] GameObject abilityObject;
    [SerializeField] Image abilityBackground;
    [SerializeField] TextMeshProUGUI abilityText;
    [SerializeField] Image abilityIcon;

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
}

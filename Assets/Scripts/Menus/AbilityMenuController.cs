using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMenuController : MonoBehaviour
{
    [SerializeField] private GameObject crowbarText, radioText, specialText, gasmaskText, bookText, mirrorText;
    [SerializeField] private GameObject tuneAbility, invisAbility, ratAbility;
    Abilities abilityData;
    List<RadioAbility> radioAbilities;

    

    private void OnEnable()
    {
        abilityData = SaveDataController.instance.saveData.abilities;
        radioAbilities = SaveDataController.instance.saveData.radioAbilities;

        crowbarText.SetActive(abilityData.crowbar);
        radioText.SetActive(abilityData.radio);
        specialText.SetActive(abilityData.radio_special);
        gasmaskText.SetActive(abilityData.gasmask);
        bookText.SetActive(abilityData.book);
        mirrorText.SetActive(abilityData.mirror);

        for (int i = 0; i < radioAbilities.Count; i++)
        {
            if (radioAbilities[i].name == "Tune")
                tuneAbility.SetActive(radioAbilities[i].isActive);
            else if (radioAbilities[i].name == "Invisibility")
                invisAbility.SetActive(radioAbilities[i].isActive);
            else if (radioAbilities[i].name == "Rats")
                ratAbility.SetActive(radioAbilities[i].isActive);
        }
    }
}

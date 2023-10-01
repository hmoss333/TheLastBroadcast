using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMenuController : MonoBehaviour
{
    [SerializeField] private GameObject crowbarText, flashlightText, radioText, specialText, mirrorText;
    [SerializeField] private GameObject tuneAbility, invisAbility, ratAbility;
    Abilities abilityData;
    List<RadioAbility> radioAbilities;

    

    private void OnEnable()
    {
        abilityData = SaveDataController.instance.saveData.abilities;
        radioAbilities = SaveDataController.instance.saveData.radioAbilities;

        crowbarText.SetActive(abilityData.crowbar);
        flashlightText.SetActive(abilityData.flashlight);
        radioText.SetActive(abilityData.radio);
        specialText.SetActive(abilityData.radio_special);
        mirrorText.SetActive(abilityData.mirror);

        for (int i = 0; i < radioAbilities.Count; i++)
        {
            switch (radioAbilities[i].name)
            {
                case "Tune":
                    tuneAbility.SetActive(radioAbilities[i].isActive);
                    break;
                case "Invisibility":
                    invisAbility.SetActive(radioAbilities[i].isActive);
                    break;
                case "Rats":
                    ratAbility.SetActive(radioAbilities[i].isActive);
                    break;
                default:
                    print($"Unable to find ability {radioAbilities[i]}");
                    break;
            }
        }
    }
}

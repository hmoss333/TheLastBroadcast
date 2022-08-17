using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AbilityPickup : InteractObject
{
    public new enum CollectType
    {
        radio,
        crowbar,
        book,
        hand,
        mirror
    }
    public CollectType collectType;

    [SerializeField] string abilityText;
    [SerializeField] Sprite abilityIcon;



    private void Update()
    {
        gameObject.SetActive(!hasActivated);
    }

    public override void Interact()
    {
        print("Collected " + gameObject.name);
        base.Interact();

        if (interacting)
        {
            UIController.instance.ToggleAbilityUI(abilityText, abilityIcon);
            SaveDataController.instance.GiveAbility(collectType.ToString());
        }
        else
        {
            hasActivated = true;
            SaveDataController.instance.SaveObjectData(SceneManager.GetActiveScene().name);
            SaveDataController.instance.SaveFile();
            UIController.instance.ToggleAbilityUI(abilityText, abilityIcon);
            gameObject.SetActive(false);
        }
    }
}

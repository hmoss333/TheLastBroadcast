using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class TranscieverController : InteractObject
{
    [Header("Equipment References")]
    [SerializeField] GeneratorController generator;
    [SerializeField] AntennaController antenna;

    [Header("Ability Values")]
    [SerializeField] float targetFrequency;
    [SerializeField] enum AbilityToGive { Tune, Invisibility, Rats };
    [SerializeField] AbilityToGive abilityToGive;

    [Header("Transmitter Frequency Values")]
    [SerializeField] [Range(0f, 10f)] float currentFrequency;
    [SerializeField] float rotSpeed, offSet;
    private float xInput;
    private bool startCountdown = false, gaveAbility = false;
    [SerializeField] float countdownTime = 3f;

    [Header("UI Values")]
    [SerializeField] TextMeshPro frequencyText;
    [SerializeField] MeshRenderer lightMesh;
    [SerializeField] GameObject dialObj;
    [SerializeField] Color stationColor;
    [SerializeField] Color presetColor;
    [SerializeField] string abilityText;
    [SerializeField] Sprite abilityIcon;

    [Header("Audio Values")]
    [SerializeField] AudioSource staticSource;



    private void Start()
    {
        GetStationdata(abilityToGive.ToString());
        frequencyText.gameObject.SetActive(false);
    }

    private void Update()
    {
        lightMesh.material.color = active ? Color.green : Color.red;
        staticSource.mute = !active;

        //Lock rotation once the player reaches either end of frequency spectrum
        float tempSpeed = rotSpeed;
        if (currentFrequency <= 0.0f || currentFrequency >= 10f)
        {
            tempSpeed = 0.0f;
            currentFrequency = currentFrequency <= 0.0f ? 0.0f : 10f;
        }
        else
        {
            tempSpeed = rotSpeed;
        }



        //Interact Logic
        if (interacting && !gaveAbility)
        {
            frequencyText.color = stationColor;
            xInput = PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>().x;
            dialObj.transform.Rotate(0.0f, xInput * tempSpeed, 0.0f);
            currentFrequency += (float)(xInput * Time.deltaTime);

            if (currentFrequency >= targetFrequency - offSet && currentFrequency <= targetFrequency + offSet
                && generator.hasActivated
                && antenna.hasActivated)
            {
                frequencyText.color = presetColor;
                lightMesh.material.color = presetColor;
                if (currentFrequency >= targetFrequency - 0.15f && currentFrequency <= targetFrequency + 0.15f)
                {
                    startCountdown = true;
                }
                else
                {
                    startCountdown = false;
                }
            }
            else
            {
                startCountdown = false;
            }

            float displayFrequency = currentFrequency * 10f;
            frequencyText.text = displayFrequency.ToString("F1");
        }


        //Countdown Timer
        if (startCountdown)
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime < 0)
            {
                startCountdown = false;
                gaveAbility = true;
                GiveStationAbility(abilityToGive.ToString());
            }
        }
        else
        {
            countdownTime = 3f;
        }

        frequencyText.gameObject.SetActive(interacting);
        CamEffectController.instance.SetEffectState(interacting && startCountdown);
    }

    public override void Interact()
    {
        if (!startCountdown && !hasActivated)
        {
            base.Interact();

            if (active)
            {
                currentFrequency = 0.0f;
                PlayerController.instance.ToggleAvatar();
                CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.lookTransform);
                CameraController.instance.FocusTarget();
            }
        }

        staticSource.mute = !interacting;
    }

    public override void EndInteract()
    {
        if (gaveAbility)
        {
            hasActivated = true;
            //SaveDataController.instance.SaveObjectData();
            UIController.instance.ToggleAbilityUI(abilityText, abilityIcon);
        }
    }

    void GiveStationAbility(string abilityName)
    {
        Debug.Log($"Actived transmitter for {abilityToGive} station");
        if (!SaveDataController.instance.GetSaveData().abilities.radio_special)
            SaveDataController.instance.GiveAbility("radio_special"); //If radio_special has not already been unlocked, set to true
        SaveDataController.instance.GiveRadioAbility(abilityName); //Give new ability station
        //SaveDataController.instance.SaveFile();

        UIController.instance.ToggleAbilityUI(abilityText, abilityIcon);
    }

    void GetStationdata(string abilityName)
    {
        List<RadioAbility> radioAbilities = new List<RadioAbility>();
        radioAbilities = SaveDataController.instance.saveData.radioAbilities;

        for (int i = 0; i < radioAbilities.Count; i++)
        {
            if (radioAbilities[i].name == abilityName)
            {
                targetFrequency = radioAbilities[i].frequency;
                break;
            }
        }
    }
}

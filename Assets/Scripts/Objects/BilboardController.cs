using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BilboardController : InteractObject
{
    public static BilboardController instance;

    [SerializeField] string targetScenario;
    [SerializeField] GameObject focusPoint;
    [SerializeField] TextMeshPro frequencyText;
    [SerializeField] GameObject frequencyCrossOut;
    [SerializeField] TextMeshPro antennaText;
    [SerializeField] GameObject antennaCrossOut;
    [SerializeField] TextMeshPro powerLevelText;
    [SerializeField] GameObject powerLevelCrossOut;

    ScenarioObjective currentScenario;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        currentScenario = SaveDataController.instance.GetScenario(targetScenario);
    }

    private void Update()
    {
        foreach (ObjectiveObj objective in currentScenario.objectives)
        {
            if (objective.value != 0.0f)
            {
                switch (objective.name)
                {
                    case "frequency":
                        frequencyText.text = $"Station Frequency: {objective.value}";
                        break;
                    case "antenna":
                        antennaText.text = $"Antenna Height: {objective.value}";
                        break;
                    case "powerLevel":
                        powerLevelText.text = $"Power Output: {objective.value}";
                        break;
                }
            }
        }

        frequencyCrossOut.SetActive(TransmitterController.instance.hasActivated);
        antennaCrossOut.SetActive(AntennaController.instance.hasActivated);
        powerLevelCrossOut.SetActive(PSController.instance.hasActivated);
    }

    public override void Interact()
    {
        base.Interact();

        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
        CameraController.instance.FocusTarget();
    }
}

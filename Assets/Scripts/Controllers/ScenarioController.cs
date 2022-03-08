using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioController : MonoBehaviour
{
    [SerializeField] string scenarioName;
    [SerializeField] ScenarioObjective scenario;
    ObjectiveObj frequencyObj;
    ObjectiveObj antennaObj;
    ObjectiveObj powerLevelObj;

    // Start is called before the first frame update
    void Start()
    {
        scenario = SaveDataController.instance.GetScenario(scenarioName);
        frequencyObj = scenario.GetObjective("frequency");
        antennaObj = scenario.GetObjective("antenna");
        powerLevelObj = scenario.GetObjective("powerLevel");

        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Check Transmitter value
        if ((TransmitterController.instance.value >= frequencyObj.value - 0.025f && TransmitterController.instance.value <= frequencyObj.value + 0.025f)
                && frequencyObj.value != 0.0f
                && !frequencyObj.hasSet)
        {
            frequencyObj.hasSet = true;
        }

        //Check Antenna value
        if ((AntennaController.instance.value >= antennaObj.value - 0.025f && AntennaController.instance.value <= antennaObj.value + 0.025f)
                && antennaObj.value != 0.0f
                && !antennaObj.hasSet)
        {
            antennaObj.hasSet = true;
        }

        //Check PowerLevel value
        if ((PSController.instance.value >= powerLevelObj.value - 0.025f && PSController.instance.value <= powerLevelObj.value + 0.025f)
                && powerLevelObj.value != 0.0f
                && !powerLevelObj.hasSet)
        {
            powerLevelObj.hasSet = true;
        }

        //If all settings have been configured, generate station
        if (frequencyObj.hasSet && antennaObj.hasSet && powerLevelObj.hasSet && scenario.station == 0.0f)
        {
            scenario.GenerateStation();
            SaveDataController.instance.SaveFile();
        }
    }
}

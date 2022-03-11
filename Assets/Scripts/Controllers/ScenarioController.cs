using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioController : MonoBehaviour
{
    //TODO
    //Multiple scenario controllers are overlapping with each other
    //If a value is set to 0 in one scenario it triggers the SetDisplayColor call
    //May work better to reference all scenarios at once, then check if one scenario is meeting the requirements
    //If yes, then use that to set the color logic


    [SerializeField] string scenarioName;
    [SerializeField] float waitTime;
    float tempWaitTime;
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
        tempWaitTime = waitTime;

        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (frequencyObj.value == 0) { frequencyObj = scenario.GetObjective("frequency"); }
        if (antennaObj.value == 0) { antennaObj = scenario.GetObjective("antenna"); }
        if (powerLevelObj.value == 0) { powerLevelObj = scenario.GetObjective("powerLevel"); }


        //Check Transmitter value
        if (TransmitterController.instance.interacting)
        {
            if ((TransmitterController.instance.value >= frequencyObj.value - 0.025f && TransmitterController.instance.value <= frequencyObj.value + 0.025f) && frequencyObj.value != 0.0f && !frequencyObj.hasSet)
            {
                TransmitterController.instance.SetDisplayColor(Color.cyan);
                tempWaitTime -= Time.deltaTime;
                if (tempWaitTime < 0)
                {
                    frequencyObj.hasSet = true;
                    TransmitterController.instance.SetDisplayColor(Color.green);
                    tempWaitTime = waitTime;
                }
            }
            else
            {
                TransmitterController.instance.SetDisplayColor(Color.red);
                tempWaitTime = waitTime;
            }
        }


        //Check Antenna value
        if (AntennaController.instance.interacting)
        {
            if ((AntennaController.instance.value >= antennaObj.value - 0.025f && AntennaController.instance.value <= antennaObj.value + 0.025f) && antennaObj.value != 0.0f && !antennaObj.hasSet)
            {
                AntennaController.instance.SetDisplayColor(Color.cyan);
                tempWaitTime -= Time.deltaTime;
                if (tempWaitTime < 0)
                {
                    antennaObj.hasSet = true;
                    AntennaController.instance.SetDisplayColor(Color.green);
                    tempWaitTime = waitTime;
                }
            }
            else
            {
                AntennaController.instance.SetDisplayColor(Color.red);
                tempWaitTime = waitTime;
            }
        }


        //Check PowerLevel value
        if (PSController.instance.interacting)
        {
            if ((PSController.instance.value >= powerLevelObj.value - 0.025f && PSController.instance.value <= powerLevelObj.value + 0.025f) && powerLevelObj.value != 0.0f && !powerLevelObj.hasSet)
            {
                PSController.instance.SetDisplayColor(Color.cyan);
                tempWaitTime -= Time.deltaTime;
                if (tempWaitTime < 0)
                {
                    powerLevelObj.hasSet = true;
                    PSController.instance.SetDisplayColor(Color.green);
                    tempWaitTime = waitTime;
                }
            }
            else
            {
                PSController.instance.SetDisplayColor(Color.red);
                tempWaitTime = waitTime;
            }
        }


        //If all settings have been configured, generate station
        if (frequencyObj.hasSet && antennaObj.hasSet && powerLevelObj.hasSet && scenario.station == 0.0f)
        {
            scenario.GenerateStation();
            SaveDataController.instance.SaveFile();
        }
    }
}

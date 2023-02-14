using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoreMenuController : MonoBehaviour
{
    [SerializeField] List<LoreData> loreData = new List<LoreData>();
    [SerializeField] List<LoreData> currentLoreData = new List<LoreData>();
    [SerializeField] UnityEngine.UI.Button loreButtonPrefab;
    [SerializeField] GameObject scrollContent;
    [SerializeField] TMP_Text loreText;

    // Start is called before the first frame update
    private void OnEnable()
    {
        loreData = SaveDataController.instance.loreSaveData.loreData;

        foreach (Transform child in scrollContent.transform)
        {
            Destroy(child.gameObject);
        }

        PopulateLoreGrid();
    }

    void PopulateLoreGrid()
    {
        currentLoreData.Clear();

        for (int i = 0; i < loreData.Count; i++)
        {
            if (loreData[i].collected)
            {
                currentLoreData.Add(loreData[i]);
                int index = loreData[i].id; //needed for 'variable capturing' to ensure the value is consistent
                UnityEngine.UI.Button newLoreButton = Instantiate(loreButtonPrefab, scrollContent.transform);
                newLoreButton.onClick.AddListener(delegate { LoadLoreData(index); });
            }
        }
    }

    public void LoadLoreData(int id)
    {
        for (int i = 0; i < currentLoreData.Count; i++)
        {
            if (currentLoreData[i].id == id)
            {
                print($"Loading lore data for index {currentLoreData[i].id}");
                loreText.text = currentLoreData[i].text;
                break;
            }
        }
    }
}

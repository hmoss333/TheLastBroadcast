using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI.Extensions;

public class LoreMenuController : MonoBehaviour
{
    [SerializeField] List<LoreData> loreData = new List<LoreData>();
    [SerializeField] List<LoreData> currentLoreData = new List<LoreData>();
    [SerializeField] UnityEngine.UI.Button loreButtonPrefab;
    [SerializeField] GameObject scrollButtonContent;
    [SerializeField] TMP_Text loreTitle, loreText;

    // Start is called before the first frame update
    private void OnEnable()
    {
        loreData = SaveDataController.instance.loreSaveData.loreData;

        foreach (Transform child in scrollButtonContent.transform)
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
                UnityEngine.UI.Button newLoreButton = Instantiate(loreButtonPrefab, scrollButtonContent.transform);
                newLoreButton.onClick.AddListener(delegate { LoadLoreData(index); });
            }
        }
    }

    public void LoadLoreData(int id)
    {
        print("TODO: load lore data to visible space in pause menu");
        //for (int i = 0; i < currentLoreData.Count; i++)
        //{
        //    if (currentLoreData[i].id == id)
        //    {
        //        print($"Loading lore data for index {currentLoreData[i].id}");
        //        loreTitle.text = currentLoreData[i].title != "" ? currentLoreData[i].title : $"Missing Title: {currentLoreData[i].id}";
        //        loreText.text = currentLoreData[i].text != "" ? currentLoreData[i].text : $"Missing Text: {currentLoreData[i].id}";
        //        break;
        //    }
        //}
    }

    //TODO
    //Design best way to view long text files for lore menu
    //Probably want to use button inputs to cycle pages
}

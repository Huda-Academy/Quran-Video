using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SimpleFileBrowser;


public class DataManager : MonoBehaviour
{
    // SerializeField for the DropDown
    [SerializeField] private TMP_Dropdown surahDropdown;
    [SerializeField] private TMP_Dropdown qariDropdown;


    // Start is called before the first frame update
    void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        // Load JSON data from resources
        TextAsset surahJson = Resources.Load<TextAsset>("Data/surahs");
        TextAsset qariJson = Resources.Load<TextAsset>("Data/qaris");

        Surah[] surahs = JsonHelper.FromJson<Surah>(surahJson.text);
        Qari[] qaris = JsonHelper.FromJson<Qari>(qariJson.text);

        // Populate the dropdown with the surahs
        foreach (Surah surah in surahs)
        {
            surahDropdown.options.Add(new TMP_Dropdown.OptionData($"{surah.number} - {surah.name}"));
        }

        foreach (Qari qari in qaris)
        {
            qariDropdown.options.Add(new TMP_Dropdown.OptionData(qari.name));
        }
    }

    public void OpenFileBrowser()
    {
        // Open file browser for folder
        FileBrowser.ShowLoadDialog((path) => { Debug.Log("Selected: " + path); }, null, FileBrowser.PickMode.Folders, false, null, null, "Select Folder", "Select");

    }
}

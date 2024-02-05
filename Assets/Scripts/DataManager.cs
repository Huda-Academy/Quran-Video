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
    [SerializeField] private TMP_InputField surahsPath;


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

    public void LoadSurahs()
    {
        string path = surahsPath.text;

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Path is empty or null.");
            return;
        }

        if (!System.IO.Directory.Exists(path))
        {
            Debug.LogError($"Directory does not exist: {path}");
            return;
        }

        string[] mp3Files = System.IO.Directory.GetFiles(path, "*.mp3");

        foreach (string file in mp3Files)
        {
            string fileName = System.IO.Path.GetFileName(file);
            Debug.Log(fileName);
        }
    }

    // TODO
    // Front and Back panels animation
    // Create placeholders for the Surah name and three details lines
    // Add Next and Previous Surahs button
    // Play animation when buttons pressed
    // Load the SVGs appropriate in appropriate placeholders
    // Play the file audio
    // Add audio visualisation
    // Add progress bar??
    // Add timer?
    // Add main sequence for cinematics
    // Add save cinematics

}

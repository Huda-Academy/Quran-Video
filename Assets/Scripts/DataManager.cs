using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.VectorGraphics;


public class DataManager : MonoBehaviour
{
    // SerializeField for the DropDown
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown surahDropdown;
    [SerializeField] private TMP_Dropdown qariDropdown;
    [SerializeField] private TMP_InputField surahsPath;

    [Header("Placeholders")]
    [SerializeField] private GameObject surahTitle;

    private GameObject currentSurahTitle = null;

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

        LoadSurahTitle();
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

    public void LoadNextSurah()
    {
        int currentSurah = surahDropdown.value;
        int nextSurah = currentSurah + 1;

        if (nextSurah >= surahDropdown.options.Count)
        {
            return;
        }

        surahDropdown.value = nextSurah;
        LoadSurahTitle();
    }

    public void LoadPreviousSurah()
    {
        int currentSurah = surahDropdown.value;
        int previousSurah = currentSurah - 1;

        if (previousSurah < 0)
        {
            return;
        }

        surahDropdown.value = previousSurah;
        LoadSurahTitle();
    }

    public void LoadSurahTitle()
    {
        int surahIndex = surahDropdown.value;
        // Instantiate Prefab from "SVG\Surahs" folder
        // Load the SVG file based on the surahIndex

        Addressables.LoadAssetAsync<GameObject>($"Assets/SVG/Surahs/{surahIndex + 1}.svg").Completed += LoadingSurahTitle_Completed;

    }

    private void LoadingSurahTitle_Completed(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load Surah Title SVG");
            return;
        }

        if (currentSurahTitle != null)
        {
            Destroy(currentSurahTitle);
        }

        GameObject surahSVG = obj.Result;
        // Set the surahSVG transform to be
        // Anchor top right
        // Pos X = -225
        // Pos Y = -55
        // Width = 350
        // Height = 80
        RectTransform surahRectTransform = surahSVG.GetComponent<RectTransform>();
        surahRectTransform.anchorMin = new Vector2(1, 1);
        surahRectTransform.anchorMax = new Vector2(1, 1);
        surahRectTransform.pivot = new Vector2(0.5f, 0.5f);
        surahRectTransform.anchoredPosition = new Vector2(-225, -55);
        surahRectTransform.sizeDelta = new Vector2(350, 80);
        surahRectTransform.localScale = Vector3.one;

        // Change SVG Image color to black
        SVGImage surahImage = surahSVG.GetComponent<SVGImage>();
        surahImage.color = Color.black;


        currentSurahTitle = Instantiate(surahSVG, surahTitle.transform, false);
    }

    // TODO
    // Front and Back panels animation
    // Add Mugawwad or Murattal to Sheikh's name when applicable
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

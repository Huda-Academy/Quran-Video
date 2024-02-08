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
    [SerializeField] private GameObject DetailsLine1;
    [SerializeField] private GameObject DetailsLine2;
    [SerializeField] private GameObject DetailsLine3;

    #region Private Fields
    Surah[] surahs = null;
    Qari[] qaris = null;

    Surah currentSurah = null;


    private GameObject currentSurahTitle = null;
    private GameObject currentJuzz = null;
    private GameObject currentOrigin = null;
    private GameObject currentAyaDigit1 = null;
    private GameObject currentAyaDigit2 = null;
    private GameObject currentAyaDigit3 = null;
    private GameObject currentAyat = null;
    private GameObject currentQari = null;

    #endregion

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

        surahs = JsonHelper.FromJson<Surah>(surahJson.text);
        qaris = JsonHelper.FromJson<Qari>(qariJson.text);
        currentSurah = surahs[0];

        // Populate the dropdown with the surahs
        foreach (Surah surah in surahs)
        {
            surahDropdown.options.Add(new TMP_Dropdown.OptionData($"{surah.number} - {surah.name}"));
        }

        foreach (Qari qari in qaris)
        {
            qariDropdown.options.Add(new TMP_Dropdown.OptionData(qari.name));
        }

        LoadSurahDetails();
    }

    public void LoadSurahFiles()
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
        LoadSurahDetails();
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
        LoadSurahDetails();
    }

    private void LoadSurahDetails()
    {
        currentSurah = surahs[surahDropdown.value];
        Debug.Log($"Surah: {currentSurah.name}");
        Debug.Log($"Origin: {currentSurah.origin}");
        Debug.Log($"Ayat: {currentSurah.ayat}");

        LoadSurahTitle(currentSurah.number);
        LoadJuzz(currentSurah.juzz);
    }

    private void LoadSurahTitle(int surahNumber)
    {
        // Instantiate Prefab from "SVG\Surahs" folder
        // Load the SVG file based on the surahNumber

        Addressables.LoadAssetAsync<GameObject>($"Assets/SVG/Surahs/{surahNumber}.svg").Completed +=
        (AsyncOperationHandle<GameObject> obj) =>
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

            // Display Surah Title
            GameObject surahTitleSVG = obj.Result;
            RectTransform surahRectTransform = surahTitleSVG.GetComponent<RectTransform>();
            surahRectTransform.anchorMin = new Vector2(1, 1);
            surahRectTransform.anchorMax = new Vector2(1, 1);
            surahRectTransform.pivot = new Vector2(0.5f, 0.5f);
            surahRectTransform.anchoredPosition = new Vector2(-225, -55);
            surahRectTransform.sizeDelta = new Vector2(350, 80);
            surahRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black
            SVGImage surahImage = surahTitleSVG.GetComponent<SVGImage>();
            surahImage.color = Color.black;

            currentSurahTitle = Instantiate(surahTitleSVG, surahTitle.transform, false);
        };
    }

    private void LoadJuzz(int juzz)
    {
        // Instantiate Prefab from "SVG\Juzz" folder
        // Load the SVG file based on the Juzz Number

        Addressables.LoadAssetAsync<GameObject>($"Assets/SVG/Juzz/{juzz}.svg").Completed +=
        (AsyncOperationHandle<GameObject> obj) =>
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to load Juzz SVG");
                return;
            }

            if (currentJuzz != null)
            {
                Destroy(currentJuzz);
            }

            // Display Juzz
            GameObject juzzSVG = obj.Result;
            RectTransform juzzRectTransform = juzzSVG.GetComponent<RectTransform>();
            juzzRectTransform.anchorMin = new Vector2(1, 1);
            juzzRectTransform.anchorMax = new Vector2(1, 1);
            juzzRectTransform.pivot = new Vector2(0.5f, 0.5f);
            juzzRectTransform.anchoredPosition = new Vector2(-364, -45);
            juzzRectTransform.sizeDelta = new Vector2(728, 80);
            juzzRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black
            SVGImage juzzImage = juzzSVG.GetComponent<SVGImage>();
            juzzImage.color = Color.black;

            currentJuzz = Instantiate(juzzSVG, DetailsLine1.transform, false);
        };
    }

    // TODO
    // Front and Back panels animation
    // Create placeholders for the Surah name and three details lines
    // Play animation when buttons pressed
    // Load the SVGs appropriate in appropriate placeholders
    // Play the file audio
    // Add audio visualisation
    // Add progress bar??
    // Add timer?
    // Add main sequence for cinematics
    // Add save cinematics

}

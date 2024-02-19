using System.Collections.Generic;
using UnityEngine;
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
    Qari currentQari = null;

    private GameObject currentSurahTitleSVG = null;
    private GameObject currentJuzzSVG = null;
    private GameObject currentRevelationSVG = null;
    private List<GameObject> currentAyaDigitSVGs = new List<GameObject>();
    private GameObject currentAyatSVG = null;
    private GameObject currentQariSVG = null;

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
        currentQari = qaris[0];

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

    public void LoadSurahDetails()
    {
        currentSurah = surahs[surahDropdown.value];

        LoadSurahTitle(currentSurah.number);
        LoadJuzz(currentSurah.juzz);
        LoadDetails(currentSurah.revelation, currentSurah.ayat);
        LoadQari(qaris[qariDropdown.value].name);
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

            if (currentSurahTitleSVG != null)
            {
                Destroy(currentSurahTitleSVG);
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

            currentSurahTitleSVG = Instantiate(surahTitleSVG, surahTitle.transform, false);
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

            if (currentJuzzSVG != null)
            {
                Destroy(currentJuzzSVG);
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

            currentJuzzSVG = Instantiate(juzzSVG, DetailsLine1.transform, false);
        };
    }


    private void LoadDetails(int revelation, int ayat)
    {

        // Destroy all the previous SVGs
        if (currentRevelationSVG != null)
        {
            Destroy(currentRevelationSVG);
        }

        foreach (GameObject digit in currentAyaDigitSVGs)
        {
            Destroy(digit);
        }

        if (currentAyatSVG != null)
        {
            Destroy(currentAyatSVG);
        }


        currentAyaDigitSVGs = new List<GameObject>();

        int currentX = 0;

        Dictionary<int, string> revelations = new Dictionary<int, string>
        {
            { 1, "Makkeyah" },
            { 2, "Madaniyah" }
        };

        // Width
        // Makkeyah = 100
        // Madaniyah = 110

        Dictionary<int, int> revelationWidths = new Dictionary<int, int>
        {
            { 1, 100 },
            { 2, 110 }
        };

        string ayatString = ayat.ToString();

        string ayatWord;
        int ayatWidth;
        int ayatMargin;

        // The mionimum number of Ayat in a Surah is 3
        if (ayat > 2 && ayat < 11)
        {
            ayatWord = "Ayat";
            ayatWidth = 100;
            ayatMargin = 40;
        }
        else
        {
            ayatWord = "Aya";
            ayatWidth = 55;
            ayatMargin = 35;
        }


        GameObject revelationSVG = Addressables.LoadAssetAsync<GameObject>($"Assets/SVG/Words/{revelations[revelation]}.svg").WaitForCompletion();
        GameObject dashSVG = GameObject.Find("Dash");
        dashSVG.GetComponent<SVGImage>().enabled = true;
        GameObject ayatSVG = Addressables.LoadAssetAsync<GameObject>($"Assets/SVG/Words/{ayatWord}.svg").WaitForCompletion();

        // Display Revelation
        RectTransform revelationRectTransform = revelationSVG.GetComponent<RectTransform>();
        revelationRectTransform.anchorMin = new Vector2(1, 1);
        revelationRectTransform.anchorMax = new Vector2(1, 1);
        revelationRectTransform.pivot = new Vector2(0.5f, 0.5f);
        revelationRectTransform.anchoredPosition = new Vector2(-5 - revelationWidths[revelation] / 2, -45);
        revelationRectTransform.sizeDelta = new Vector2(revelationWidths[revelation], 80);
        revelationRectTransform.localScale = Vector3.one;

        // Change SVG Image color to black
        SVGImage revelationImage = revelationSVG.GetComponent<SVGImage>();
        revelationImage.color = Color.black;


        RectTransform dashRectTransform = dashSVG.GetComponent<RectTransform>();
        dashRectTransform.anchoredPosition = new Vector2(-35 - revelationWidths[revelation], -35);

        currentRevelationSVG = Instantiate(revelationSVG, DetailsLine2.transform, false);

        //Dash width is 60
        currentX += revelationWidths[revelation] + 50;

        for (int i = ayatString.Length - 1; i >= 0; i--)
        {
            GameObject digitSVG = Addressables.LoadAssetAsync<GameObject>($"Assets/SVG/Numbers/{ayatString[i]}.svg").WaitForCompletion();
            int posX = -30 - currentX;

            // Display Digit
            RectTransform digitRectTransform = digitSVG.GetComponent<RectTransform>();
            digitRectTransform.anchorMin = new Vector2(1, 1);
            digitRectTransform.anchorMax = new Vector2(1, 1);
            digitRectTransform.pivot = new Vector2(0.5f, 0.5f);
            digitRectTransform.anchoredPosition = new Vector2(posX, -45);
            digitRectTransform.sizeDelta = new Vector2(30, 45);
            digitRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black
            SVGImage digitImage = digitSVG.GetComponent<SVGImage>();
            digitImage.color = Color.black;

            currentAyaDigitSVGs.Add(Instantiate(digitSVG, DetailsLine2.transform, false));

            // Digit width is 30
            currentX += 30;
        }

        RectTransform ayatRectTransform = ayatSVG.GetComponent<RectTransform>();
        ayatRectTransform.anchorMin = new Vector2(1, 1);
        ayatRectTransform.anchorMax = new Vector2(1, 1);
        ayatRectTransform.pivot = new Vector2(0.8f, 0.5f);
        ayatRectTransform.anchoredPosition = new Vector2(-ayatMargin - currentX, -45);
        ayatRectTransform.sizeDelta = new Vector2(ayatWidth, 80);
        ayatRectTransform.localScale = Vector3.one;

        // Change SVG Image color to black
        SVGImage ayatImage = ayatSVG.GetComponent<SVGImage>();
        ayatImage.color = Color.black;

        currentAyatSVG = Instantiate(ayatSVG, DetailsLine2.transform, false);

        int TotalWidth = currentX + ayatMargin + ayatWidth;

        DetailsLine2.GetComponent<RectTransform>().sizeDelta = new Vector2(TotalWidth, 0);
    }

    private void LoadQari(string qariName)
    {
        // Instantiate Prefab from "SVG\Qari" folder
        // Load the SVG file based on the qariId

        Addressables.LoadAssetAsync<GameObject>($"Assets/SVG/Qaris/{qariName}.svg").Completed +=
        (AsyncOperationHandle<GameObject> obj) =>
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to load Qari SVG");
                return;
            }

            if (currentQariSVG != null)
            {
                Destroy(currentQariSVG);
            }

            // Display Qari
            GameObject qariSVG = obj.Result;
            RectTransform qariRectTransform = qariSVG.GetComponent<RectTransform>();
            qariRectTransform.anchorMin = new Vector2(1f, 1f);
            qariRectTransform.anchorMax = new Vector2(1f, 1f);
            qariRectTransform.pivot = new Vector2(0.5f, 0.5f);
            qariRectTransform.anchoredPosition = new Vector2(-364, -45);
            qariRectTransform.sizeDelta = new Vector2(728, 80);
            qariRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black
            SVGImage qariImage = qariSVG.GetComponent<SVGImage>();
            qariImage.color = Color.black;

            currentQariSVG = Instantiate(qariSVG, DetailsLine3.transform, false);
        };
    }

}

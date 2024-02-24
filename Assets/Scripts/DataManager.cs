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
    [SerializeField] private TMP_InputField inputPath;
    [SerializeField] private TMP_InputField outputPath;

    [Header("Placeholders")]
    [SerializeField] private GameObject surahTitle;
    [SerializeField] private GameObject DetailsLine1;
    [SerializeField] private GameObject DetailsLine2;
    [SerializeField] private GameObject DetailsLine3;

    [Header("Qari Portrait")]
    [SerializeField] private GameObject qariPortrait;

    #region Private Fields
    Surah[] surahs = null;
    Qari[] qaris = null;

    Surah currentSurah = null;
    Qari currentQari = null;


    private GameObject currentSurahTitleSVG = null;
    private GameObject currentSurahTitleSVGShadow = null;

    private GameObject currentJuzzSVG = null;
    private GameObject currentJuzzSVGShadow = null;

    private GameObject currentRevelationSVG = null;
    private GameObject currentRevelationSVGShadow = null;

    private List<GameObject> currentAyaDigitSVGs = new List<GameObject>();
    private GameObject currentAyatSVG = null;
    private GameObject currentAyatSVGShadow = null;
    private GameObject currentQariSVG = null;
    private GameObject currentQariSVGShadow = null;

    private string basePath = @"D:\Quran";

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
        LoadQari();
    }

    public void LoadSurahFiles()
    {
        string path = inputPath.text;

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
                Destroy(currentSurahTitleSVGShadow);
            }

            // Display Surah Title
            GameObject surahTitleSVG = obj.Result;
            RectTransform surahRectTransform = surahTitleSVG.GetComponent<RectTransform>();
            surahRectTransform.anchorMin = new Vector2(1, 1);
            surahRectTransform.anchorMax = new Vector2(1, 1);
            surahRectTransform.pivot = new Vector2(0.5f, 0.5f);
            surahRectTransform.anchoredPosition = new Vector2(-240, -50);
            surahRectTransform.sizeDelta = new Vector2(480, 90);
            surahRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black
            SVGImage surahImage = surahTitleSVG.GetComponent<SVGImage>();
            surahImage.color = Color.black;

            currentSurahTitleSVG = Instantiate(surahTitleSVG, surahTitle.transform, false);

            //Display same SVG x+2, y+2 with alpha = 0.5 as drop shadow
            RectTransform surahShadowRectTransform = surahTitleSVG.GetComponent<RectTransform>();
            surahShadowRectTransform.anchorMin = new Vector2(1, 1);
            surahShadowRectTransform.anchorMax = new Vector2(1, 1);
            surahShadowRectTransform.pivot = new Vector2(0.5f, 0.5f);
            surahShadowRectTransform.anchoredPosition = new Vector2(-238.5f, -51.5f);
            surahShadowRectTransform.sizeDelta = new Vector2(480, 90);
            surahShadowRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black with Alpha = 0.5
            SVGImage surahShadowImage = surahTitleSVG.GetComponent<SVGImage>();
            surahShadowImage.color = new Color(0, 0, 0, 0.5f);

            currentSurahTitleSVGShadow = Instantiate(surahTitleSVG, surahTitle.transform, false);
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
                Destroy(currentJuzzSVGShadow);
            }

            // Display Juzz
            GameObject juzzSVG = obj.Result;
            RectTransform juzzRectTransform = juzzSVG.GetComponent<RectTransform>();
            juzzRectTransform.anchorMin = new Vector2(1, 1);
            juzzRectTransform.anchorMax = new Vector2(1, 1);
            juzzRectTransform.pivot = new Vector2(0.5f, 0.5f);
            juzzRectTransform.anchoredPosition = new Vector2(-375, -45);
            juzzRectTransform.sizeDelta = new Vector2(750, 90);
            juzzRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black
            SVGImage juzzImage = juzzSVG.GetComponent<SVGImage>();
            juzzImage.color = Color.black;

            currentJuzzSVG = Instantiate(juzzSVG, DetailsLine1.transform, false);

            // Display same SVG x+1.5, y+1.5 with alpha = 0.5 as drop shadow
            RectTransform juzzShadowRectTransform = juzzSVG.GetComponent<RectTransform>();
            juzzShadowRectTransform.anchorMin = new Vector2(1, 1);
            juzzShadowRectTransform.anchorMax = new Vector2(1, 1);
            juzzShadowRectTransform.pivot = new Vector2(0.5f, 0.5f);
            juzzShadowRectTransform.anchoredPosition = new Vector2(-373.5f, -46.5f);
            juzzShadowRectTransform.sizeDelta = new Vector2(750, 90);
            juzzShadowRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black with Alpha = 0.5
            SVGImage juzzShadowImage = juzzSVG.GetComponent<SVGImage>();
            juzzShadowImage.color = new Color(0, 0, 0, 0.5f);

            currentJuzzSVGShadow = Instantiate(juzzSVG, DetailsLine1.transform, false);
        };
    }

    private void LoadDetails(int revelation, int ayat)
    {

        // Destroy all the previous SVGs
        if (currentRevelationSVG != null)
        {
            Destroy(currentRevelationSVG);
            Destroy(currentRevelationSVGShadow);
        }

        foreach (GameObject digit in currentAyaDigitSVGs)
        {
            Destroy(digit);
        }

        if (currentAyatSVG != null)
        {
            Destroy(currentAyatSVG);
            Destroy(currentAyatSVGShadow);
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
            { 1, 120 },
            { 2, 130 }
        };

        string ayatString = ayat.ToString();

        string ayatWord;
        int ayatWidth;
        int ayatMargin;

        // The mionimum number of Ayat in a Surah is 3
        if (ayat > 2 && ayat < 11)
        {
            ayatWord = "Ayat";
            ayatWidth = 120;
            ayatMargin = 10;
        }
        else
        {
            ayatWord = "Aya";
            ayatWidth = 65;
            ayatMargin = 5;
        }


        GameObject revelationSVG = Addressables.LoadAssetAsync<GameObject>($"Assets/SVG/Words/{revelations[revelation]}.svg").WaitForCompletion();
        GameObject ayatSVG = Addressables.LoadAssetAsync<GameObject>($"Assets/SVG/Words/{ayatWord}.svg").WaitForCompletion();

        // Dash already exists and no need to keep instantiating it. Just show it
        GameObject dashSVG = GameObject.Find("Dash");
        dashSVG.GetComponent<SVGImage>().enabled = true;

        RectTransform dashRectTransform = dashSVG.GetComponent<RectTransform>();
        dashRectTransform.anchoredPosition = new Vector2(-35 - revelationWidths[revelation], -45);

        // Display Revelation
        RectTransform revelationRectTransform = revelationSVG.GetComponent<RectTransform>();
        revelationRectTransform.anchorMin = new Vector2(1, 1);
        revelationRectTransform.anchorMax = new Vector2(1, 1);
        revelationRectTransform.pivot = new Vector2(0.5f, 0.5f);
        revelationRectTransform.anchoredPosition = new Vector2(-5 - revelationWidths[revelation] / 2, -47);
        revelationRectTransform.sizeDelta = new Vector2(revelationWidths[revelation], 95);
        revelationRectTransform.localScale = Vector3.one;

        // Change SVG Image color to black
        SVGImage revelationImage = revelationSVG.GetComponent<SVGImage>();
        revelationImage.color = Color.black;

        currentRevelationSVG = Instantiate(revelationSVG, DetailsLine2.transform, false);

        // Show the shadow of the SVG
        RectTransform revelationShadowRectTransform = revelationSVG.GetComponent<RectTransform>();
        revelationShadowRectTransform.anchorMin = new Vector2(1, 1);
        revelationShadowRectTransform.anchorMax = new Vector2(1, 1);
        revelationShadowRectTransform.pivot = new Vector2(0.5f, 0.5f);
        revelationShadowRectTransform.anchoredPosition = new Vector2(-3.5f - revelationWidths[revelation] / 2, -48.5f);
        revelationShadowRectTransform.sizeDelta = new Vector2(revelationWidths[revelation], 95);
        revelationShadowRectTransform.localScale = Vector3.one;

        // Change SVG Image color to black with Alpha = 0.5
        SVGImage revelationShadowImage = revelationSVG.GetComponent<SVGImage>();
        revelationShadowImage.color = new Color(0, 0, 0, 0.5f);

        currentRevelationSVGShadow = Instantiate(revelationSVG, DetailsLine2.transform, false);

        //Dash width is 60
        currentX += revelationWidths[revelation] + 60;

        for (int i = ayatString.Length - 1; i >= 0; i--)
        {
            GameObject digitSVG = Addressables.LoadAssetAsync<GameObject>($"Assets/SVG/Numbers/{ayatString[i]}.svg").WaitForCompletion();
            int posX = -30 - currentX;

            // Display Digit
            RectTransform digitRectTransform = digitSVG.GetComponent<RectTransform>();
            digitRectTransform.anchorMin = new Vector2(1, 1);
            digitRectTransform.anchorMax = new Vector2(1, 1);
            digitRectTransform.pivot = new Vector2(0.5f, 0.5f);
            digitRectTransform.anchoredPosition = new Vector2(posX, -50);
            digitRectTransform.sizeDelta = new Vector2(40, 50);
            digitRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black
            SVGImage digitImage = digitSVG.GetComponent<SVGImage>();
            digitImage.color = Color.black;

            currentAyaDigitSVGs.Add(Instantiate(digitSVG, DetailsLine2.transform, false));

            // Show the shadow of the SVG
            RectTransform digitShadowRectTransform = digitSVG.GetComponent<RectTransform>();
            digitShadowRectTransform.anchorMin = new Vector2(1, 1);
            digitShadowRectTransform.anchorMax = new Vector2(1, 1);
            digitShadowRectTransform.pivot = new Vector2(0.5f, 0.5f);
            digitShadowRectTransform.anchoredPosition = new Vector2(posX + 2, -52);
            digitShadowRectTransform.sizeDelta = new Vector2(40, 50);
            digitShadowRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black with Alpha = 0.5
            SVGImage digitShadowImage = digitSVG.GetComponent<SVGImage>();
            digitShadowImage.color = new Color(0, 0, 0, 0.5f);

            currentAyaDigitSVGs.Add(Instantiate(digitSVG, DetailsLine2.transform, false));

            // Digit width is 30
            currentX += 30;
        }

        RectTransform ayatRectTransform = ayatSVG.GetComponent<RectTransform>();
        ayatRectTransform.anchorMin = new Vector2(1, 1);
        ayatRectTransform.anchorMax = new Vector2(1, 1);
        ayatRectTransform.pivot = new Vector2(0.8f, 0.5f);
        ayatRectTransform.anchoredPosition = new Vector2(-30 - ayatMargin - currentX, -47);
        ayatRectTransform.sizeDelta = new Vector2(ayatWidth, 90);
        ayatRectTransform.localScale = Vector3.one;

        // Change SVG Image color to black
        SVGImage ayatImage = ayatSVG.GetComponent<SVGImage>();
        ayatImage.color = Color.black;

        currentAyatSVG = Instantiate(ayatSVG, DetailsLine2.transform, false);

        // Show the shadow of the SVG
        RectTransform ayatShadowRectTransform = ayatSVG.GetComponent<RectTransform>();
        ayatShadowRectTransform.anchorMin = new Vector2(1, 1);
        ayatShadowRectTransform.anchorMax = new Vector2(1, 1);
        ayatShadowRectTransform.pivot = new Vector2(0.8f, 0.5f);
        ayatShadowRectTransform.anchoredPosition = new Vector2(-28.5f - ayatMargin - currentX, -48.5f);
        ayatShadowRectTransform.sizeDelta = new Vector2(ayatWidth, 90);
        ayatShadowRectTransform.localScale = Vector3.one;

        // Change SVG Image color to black with Alpha = 0.5
        SVGImage ayatShadowImage = ayatSVG.GetComponent<SVGImage>();
        ayatShadowImage.color = new Color(0, 0, 0, 0.5f);

        currentAyatSVGShadow = Instantiate(ayatSVG, DetailsLine2.transform, false);

        int TotalWidth = revelationWidths[revelation] + 60 + (ayatString.Length * 40) + ayatWidth;

        DetailsLine2.GetComponent<RectTransform>().sizeDelta = new Vector2(TotalWidth, 0);
    }

    public void LoadQari()
    {
        currentQari = qaris[qariDropdown.value];
        LoadQariDetails();
    }

    private void LoadQariDetails()
    {
        // Instantiate Prefab from "SVG\Qari" folder
        // Load the SVG file based on the qariId

        Addressables.LoadAssetAsync<GameObject>($"Assets/SVG/Qaris/{currentQari.name}.svg").Completed +=
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
                Destroy(currentQariSVGShadow);
            }

            // Display Qari
            GameObject qariSVG = obj.Result;
            RectTransform qariRectTransform = qariSVG.GetComponent<RectTransform>();
            qariRectTransform.anchorMin = new Vector2(1f, 1f);
            qariRectTransform.anchorMax = new Vector2(1f, 1f);
            qariRectTransform.pivot = new Vector2(0.5f, 0.5f);
            qariRectTransform.anchoredPosition = new Vector2(-375, -50);
            qariRectTransform.sizeDelta = new Vector2(750, 90);
            qariRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black
            SVGImage qariImage = qariSVG.GetComponent<SVGImage>();
            qariImage.color = Color.black;

            currentQariSVG = Instantiate(qariSVG, DetailsLine3.transform, false);

            // Display same SVG x+1.5, y+1.5 with alpha = 0.5 as drop shadow
            RectTransform qariShadowRectTransform = qariSVG.GetComponent<RectTransform>();
            qariShadowRectTransform.anchorMin = new Vector2(1f, 1f);
            qariShadowRectTransform.anchorMax = new Vector2(1f, 1f);
            qariShadowRectTransform.pivot = new Vector2(0.5f, 0.5f);
            qariShadowRectTransform.anchoredPosition = new Vector2(-373.5f, -51.5f);
            qariShadowRectTransform.sizeDelta = new Vector2(750, 90);
            qariShadowRectTransform.localScale = Vector3.one;

            // Change SVG Image color to black with Alpha = 0.5
            SVGImage qariShadowImage = qariSVG.GetComponent<SVGImage>();
            qariShadowImage.color = new Color(0, 0, 0, 0.5f);

            currentQariSVGShadow = Instantiate(qariSVG, DetailsLine3.transform, false);
        };

        //Load Qair's Portrait
        Addressables.LoadAssetAsync<Sprite>($"Assets/Images/Qaris/{currentQari.portrait}.png").Completed +=
        (AsyncOperationHandle<Sprite> obj) =>
        {
            qariPortrait.GetComponent<SpriteRenderer>().sprite = obj.Result;
        };

        inputPath.text = $"{basePath}\\{currentQari.fullName}";
        outputPath.text = $"{basePath}\\{currentQari.fullName}\\Video";

    }

}

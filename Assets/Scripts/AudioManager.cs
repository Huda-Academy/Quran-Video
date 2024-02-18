using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.ExceptionServices;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

    TimerManager timerManager;
    Task timerTask;

    [SerializeField]
    GameObject audioBarsContainer;
    [SerializeField]
    GameObject audioBar;

    GameObject[] audioBars = new GameObject[8];

    float[] _samples = new float[512];
    float[] _freqBands = new float[8];

    [SerializeField]
    TMP_Dropdown surahDropdown;

    [SerializeField]
    TMP_InputField QuranPath;

    // TMP BUtton reference
    [SerializeField]
    TextMeshProUGUI pauseButton;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        timerManager = GetComponent<TimerManager>();
    }

    private void Start()
    {
        DisplayAudioBars();
    }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            GetSpectrumAudioSource();
            MakeFrequencyBands();
            UpdateAudioBars();
        }
    }

    public async void PlayAudio()
    {
        int surahIndex = surahDropdown.value + 1;
        string surahIndexPadded = surahIndex.ToString().PadLeft(3, '0');

        // Search for files that start with the surah index in the path
        string audioFile = System.IO.Directory.GetFiles(QuranPath.text, surahIndexPadded + "*").FirstOrDefault();

        AudioClip audioClip = await GetAudioClip(audioFile);

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            timerTask = timerManager.StartTimer(audioClip.length);
            pauseButton.text = "Pause";
        }
        else
        {
            Debug.LogError("Audio Clip is null");
        }
    }

    private async Task<AudioClip> GetAudioClip(string filePath)
    {
        try
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
            {
                // Send the request and wait for a response
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError(www.error);
                    return null;
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    return clip;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public void PauseAudio()
    {

        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            timerManager.PauseTimer(true);
            pauseButton.text = "Resume";
        }
        else
        {
            audioSource.UnPause();
            timerManager.PauseTimer(false);
            pauseButton.text = "Pause";
        }
    }

    private void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    private void MakeFrequencyBands()
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 8)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;

            _freqBands[i] = average;
        }
    }

    private void DisplayAudioBars()
    {
        // initial AudioBar Tranform
        // x=-0.46, y=-0.5, z=-0.1
        // Scale = x=0.05, y=frequency/1, z=1
        // Add margin of 0.07

        for (int i = 0; i < 8; i++)
        {
            if (audioBars[i] == null)
            {
                audioBars[i] = Instantiate(audioBar, audioBarsContainer.transform);
                audioBars[i].transform.localPosition = new Vector3(-0.46f + (0.15f * i), -0.5f, -0.1f);
            }
            audioBars[i].transform.localScale = new Vector3(0.12f, 0.1f, 1);
        }
    }

    private void UpdateAudioBars()
    {
        for (int i = 0; i < 8; i++)
        {
            audioBars[i].transform.localScale = new Vector3(0.12f, Mathf.Clamp(Mathf.Lerp(0.1f, 2.5f, _freqBands[i]), 0.1f, 1f), 1);
        }
    }
}

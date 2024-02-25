using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;
using NAudio.Wave;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

    public float ClipLength => audioSource.clip.length;

    TimerManager timerManager;
    // Task timerTask;
    // CancellationTokenSource tokenSource;

    [SerializeField]
    GameObject audioBarsContainer;
    [SerializeField]
    GameObject audioBar;

    GameObject[] audioBars = new GameObject[8];
    [SerializeField]
    float barUpdateSpeed = 110.0f;

    float[] _samples = new float[512];
    float[] _freqBands = new float[8];

    [SerializeField]
    TMP_Dropdown surahDropdown;

    [SerializeField]
    TMP_InputField QuranPath;

    // TMP BUtton reference
    [SerializeField]
    TextMeshProUGUI pauseButton;

    [SerializeField]
    Slider progressBar;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        timerManager = GetComponent<TimerManager>();
    }

    private void Start()
    {
        DisplayAudioBars();
        UpdateProgress(0);
    }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            GetSpectrumAudioSource();
            MakeFrequencyBands();
            UpdateAudioBars();
            UpdateProgress(audioSource.time / audioSource.clip.length);
        }
    }

    public async void PlayAudio()
    {
        audioSource.Stop();

        await LoadAudioFile();

        audioSource.Play();
        pauseButton.text = "Pause";
    }

    public async Task LoadAudioFile()
    {
        int surahIndex = surahDropdown.value + 1;
        string surahIndexPadded = surahIndex.ToString().PadLeft(3, '0');

        // Search for files that start with the surah index in the path
        string audioFile = Directory.GetFiles(QuranPath.text, surahIndexPadded + "*").FirstOrDefault();

        audioSource.clip = await GetAudioClip(audioFile);
    }

    private async Task<AudioClip> GetAudioClip(string filePath)
    {
        try
        {
            // Read file bytes asynchronously
            byte[] data = await File.ReadAllBytesAsync(filePath);
            AudioClip clip = NAudioPlayer.FromMp3Data(data);
            return clip;
            // using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
            // {
            //     // Send the request and wait for a response
            //     await www.SendWebRequest();

            //     if (www.result == UnityWebRequest.Result.ConnectionError)
            //     {
            //         Debug.LogError(www.error);
            //         return null;
            //     }
            //     else
            //     {
            //         // AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

            //         return clip;
            //     }
            // }
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
        float[] _samplesLeft = new float[512];
        float[] _samplesRight = new float[512];

        audioSource.GetSpectrumData(_samplesLeft, 0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(_samplesRight, 1, FFTWindow.Blackman);

        for (int i = 0; i < 512; i++)
        {
            _samples[i] = (_samplesLeft[i] + _samplesRight[i]) / 2;
        }
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

            _freqBands[i] = average * 10;
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
            float new_y = Mathf.Lerp(_freqBands[i], audioBars[i].transform.localScale.y, barUpdateSpeed * Time.deltaTime);
            audioBars[i].transform.localScale = new Vector3(0.12f, Mathf.Clamp(new_y, 0.1f, 1f), 1);
        }
    }

    private void UpdateProgress(float progress)
    {
        // Set the progress bar value
        progressBar.value = progress;
    }

    public void SeekAudio()
    {
        if (audioSource.clip != null)
        {
            audioSource.time = progressBar.value * audioSource.clip.length;
            timerManager.SetTime(audioSource.clip.length - audioSource.time);
        }
    }
}

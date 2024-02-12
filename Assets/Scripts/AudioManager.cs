using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Threading.Tasks;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

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
    }

    public async void PlayAudio()
    {
        int surahIndex = surahDropdown.value + 1;
        string surahIndexPadded = surahIndex.ToString().PadLeft(3, '0');

        // Search for files that start with the surah index in the path
        string[] files = System.IO.Directory.GetFiles(QuranPath.text, surahIndexPadded + "*");

        AudioClip audioClip = await GetAudioClip(files[0]);

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
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
            pauseButton.text = "Resume";
        }
        else
        {
            audioSource.UnPause();
            pauseButton.text = "Pause";
        }

        // if (getAudioClipCoroutine != null)
        // {
        //     StopCoroutine(getAudioClipCoroutine);
        // }
    }
}

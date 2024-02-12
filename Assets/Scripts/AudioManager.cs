using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    Coroutine getAudioClipCoroutine;


    [SerializeField]
    TMP_Dropdown surahDropdown;

    [SerializeField]
    TMP_InputField QuranPath;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio()
    {
        int surahIndex = surahDropdown.value + 1;
        string surahIndexPadded = surahIndex.ToString().PadLeft(3, '0');

        // Search for files that start with the surah index in the path
        string[] files = System.IO.Directory.GetFiles(QuranPath.text, surahIndexPadded + "*");

        Debug.Log(surahDropdown.options[surahDropdown.value].text);

        getAudioClipCoroutine = StartCoroutine(GetAudioClip(files[0]));

    }

    IEnumerator GetAudioClip(string filePath)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(www.error);
            }
            else
            {

                audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.Play();
            }
        }
    }

    public void PauseAudio()
    {

        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }

        if (getAudioClipCoroutine != null)
        {
            StopCoroutine(getAudioClipCoroutine);
        }
    }
}

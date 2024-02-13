using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;


[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    AudioClip audioClip;

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

    public void PlayAudio()
    {
        int surahIndex = surahDropdown.value + 1;
        string surahIndexPadded = surahIndex.ToString().PadLeft(3, '0');

        // Search for files that start with the surah index in the path
        string[] files = System.IO.Directory.GetFiles(QuranPath.text, surahIndexPadded + "*");

        RunJob(files[0]);

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (audioSource != null)
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

    }

    // A method to create and run the job
    public void RunJob(string filePath)
    {
        // Create a NativeArray of the MP3Job struct
        // Initialize the struct with the file path and the audio type
        GetQuranAudioJob getAudioJob = new GetQuranAudioJob
        {
            filePath = new NativeArray<char>(filePath.ToCharArray(), Allocator.Persistent),
            audioData = new NativeArray<float>(1, Allocator.Persistent)
        };

        // Create a job handle by scheduling the job
        JobHandle jobHandle = getAudioJob.Schedule();

        // Complete the job
        jobHandle.Complete();
        Debug.Log("Is Complete? " + jobHandle.IsCompleted);

        float[] temp = getAudioJob.audioData.ToArray();
        Debug.Log("Native Array: " + getAudioJob.audioData.Length);
        Debug.Log("Float Array: " + temp.Length);

        audioClip = AudioClip.Create("Quran Audio", getAudioJob.audioData.Length, 1, 44100, false);
        audioClip.SetData(getAudioJob.audioData.ToArray(), 0);

        // dispose of the NativeArray
        getAudioJob.filePath.Dispose();
        getAudioJob.audioData.Dispose();
    }
}

[BurstCompile]
public struct GetQuranAudioJob : IJob
{
    public NativeArray<char> filePath;
    public NativeArray<float> audioData;
    public async void Execute()
    {
        // Get the audio file
        try
        {
            byte[] audioBytes = await System.IO.File.ReadAllBytesAsync(new string(filePath.ToArray()));
            float[] floatArr = new float[audioBytes.Length / 4];
            for (int i = 0; i < floatArr.Length; i++)
            {
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(audioBytes, i * 4, 4);
                floatArr[i] = BitConverter.ToSingle(audioBytes, i * 4);
            }

            audioData = new NativeArray<float>(floatArr, Allocator.Persistent);
            Debug.Log("Inside Job: " + audioData.Length);

        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
}

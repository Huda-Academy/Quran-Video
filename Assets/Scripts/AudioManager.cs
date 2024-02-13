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
    public async void RunJob(string filePath)
    {
        NativeReference<NativeArray<float>> audioBuffer = new NativeReference<NativeArray<float>>(Allocator.Persistent);
        // Create a NativeArray of the MP3Job struct
        // Initialize the struct with the file path and the audio type
        GetQuranAudioJob getAudioJob = new GetQuranAudioJob
        {
            filePath = new NativeArray<char>(filePath.ToCharArray(), Allocator.Persistent),
            audioData = audioBuffer
        };

        // Create a job handle by scheduling the job
        JobHandle jobHandle = getAudioJob.Schedule();

        // Complete the job
        jobHandle.Complete();
        Debug.Log("Is Complete? " + jobHandle.IsCompleted);

        Debug.Log("Native Array: " + audioBuffer.Value.Length);

        audioClip = AudioClip.Create("Quran Audio", audioBuffer.Value.Length, 1, 44100, false);
        audioClip.SetData(audioBuffer.Value.ToArray(), 0);

        // dispose of the NativeArray
        getAudioJob.filePath.Dispose();
        audioBuffer.Value.Dispose();
        audioBuffer.Dispose();
    }
}

[BurstCompile]
public struct GetQuranAudioJob : IJob
{
    public NativeArray<char> filePath;
    public NativeReference<NativeArray<float>> audioData;
    public void Execute()
    {
        // Get the audio file
        byte[] audioBytes = System.IO.File.ReadAllBytesAsync(new string(filePath.ToArray())).GetAwaiter().GetResult();
        float[] floatArr = new float[audioBytes.Length / 4];
        for (int i = 0; i < floatArr.Length; i++)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(audioBytes, i * 4, 4);
            floatArr[i] = BitConverter.ToSingle(audioBytes, i * 4);
        }
        audioData.Value = new NativeArray<float>(floatArr, Allocator.Temp);
        Debug.Log("Inside Job: " + audioData.Value.Length);
    }
}

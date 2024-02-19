using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Recorder;

public class CinematicsManager : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    RecorderController recorderController;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
    }

    public void PlayCinematic(string cinematicName)
    {
        // Play the cinematic
        director.Play();
    }

    public void RecordQuranClip()
    {
        if (recorderController != null && recorderController.IsRecording())
        {
            recorderController.StopRecording();
            return;
        }

        // Record the cinematic

        MovieRecorderSettings videoRecorderSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        RecorderControllerSettings controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();


        videoRecorderSettings.Enabled = true;

        videoRecorderSettings.CapFrameRate = false;
        videoRecorderSettings.FrameRate = 60.0f;

        videoRecorderSettings.ImageInputSettings = new GameViewInputSettings
        {
            OutputWidth = 1920,
            OutputHeight = 1080
        };

        videoRecorderSettings.AudioInputSettings.PreserveAudio = true;

        // Set output file path
        videoRecorderSettings.OutputFile = "D:\\Temp\\Testing";

        RecorderOptions.VerboseMode = false;

        controllerSettings.AddRecorderSettings(videoRecorderSettings);
        controllerSettings.SetRecordModeToTimeInterval(0, audioManager.ClipLength);
        controllerSettings.FrameRate = 60.0f;

        recorderController = new RecorderController(controllerSettings);
        recorderController.PrepareRecording();
        recorderController.StartRecording();

        director.Play();

    }

}

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



    public void PlayCinematic(string cinematicName)
    {
        // Play the cinematic
        Debug.Log("Playing cinematic: " + cinematicName);
        director.Play();
    }

    public void RecordQuranClip()
    {
        if (recorderController != null && recorderController.IsRecording())
        {
            recorderController.StopRecording();
            Debug.Log("Recording stopped");
            return;
        }

        // Record the cinematic
        Debug.Log("Recording cinematic");

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
        videoRecorderSettings.OutputFile = "D:\\Temp\\MyVideo";

        RecorderOptions.VerboseMode = true;

        controllerSettings.AddRecorderSettings(videoRecorderSettings);
        controllerSettings.SetRecordModeToTimeInterval(0, 10);
        controllerSettings.FrameRate = 60.0f;

        recorderController = new RecorderController(controllerSettings);
        recorderController.PrepareRecording();
        recorderController.StartRecording();

        director.Play();

    }

}

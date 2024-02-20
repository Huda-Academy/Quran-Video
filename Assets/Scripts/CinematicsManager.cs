using System.IO;
using TMPro;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class CinematicsManager : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] GameObject controlPanel;
    [SerializeField] TMP_Dropdown surahDropdown;
    [SerializeField] TMP_InputField QuranOutputPath;

    RecorderController recorderController;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
    }

    public void PlayCinematic()
    {
        // Play the cinematic
        director.Play();
    }

    public async void RecordQuranClip()
    {
        if (recorderController != null && recorderController.IsRecording())
        {
            recorderController.StopRecording();
            return;
        }

        //Prepare Audio
        await audioManager.LoadAudioFile();
        audioManager.SeekAudio();


        // Prepare settings
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
        videoRecorderSettings.OutputFile = Path.Combine(QuranOutputPath.text, surahDropdown.options[surahDropdown.value].text);

        //Create Directory if it doesn't exist
        if (!Directory.Exists(QuranOutputPath.text))
        {
            Directory.CreateDirectory(QuranOutputPath.text);
        }

        RecorderOptions.VerboseMode = false;

        controllerSettings.AddRecorderSettings(videoRecorderSettings);
        controllerSettings.SetRecordModeToTimeInterval(0, audioManager.ClipLength + 1.5f);
        controllerSettings.FrameRate = 60.0f;

        // Hide Control panel. Record stopping should be with HotKey
        controlPanel.SetActive(false);

        recorderController = new RecorderController(controllerSettings);
        recorderController.PrepareRecording();
        recorderController.StartRecording();

        // Play animations
        director.Play();

        // Wait for animation to finish then play audio
        await Awaitable.WaitForSecondsAsync(1.5f);
        audioManager.PlayAudio();

        // Wait for audio to finish then show control panel
        while (recorderController.IsRecording())
        {
            await Awaitable.WaitForSecondsAsync(1);
        }

        controlPanel.SetActive(true);
    }

    public void StopRecording(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (recorderController != null && recorderController.IsRecording())
        {
            recorderController.StopRecording();
            audioManager.PauseAudio();
        }
    }
}

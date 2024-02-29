using System.IO;
using TMPro;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class CinematicsManager : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] GameObject controlPanel;
    [SerializeField] TMP_Dropdown surahDropdown;
    [SerializeField] TMP_InputField QuranOutputPath;

    RecorderController recorderController;
    AudioManager audioManager;
    DataManager dataManager;

    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
        dataManager = GetComponent<DataManager>();
    }

    public void PlayCinematic()
    {
        // Play the cinematic
        director.Play();
    }

    public async void RecordClip()
    {
        await RecordQuranClip();
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

    public async void Export()
    {
        while (true)
        {
            await RecordQuranClip();
            if (surahDropdown.value == surahDropdown.options.Count - 1) // Last Surah
            {
                break;
            }
            dataManager.LoadNextSurah();
        }
    }

    private async Task RecordQuranClip()
    {
        if (recorderController != null && recorderController.IsRecording())
        {
            recorderController.StopRecording();
            return;
        }

        //Prepare Audio
        await audioManager.LoadAudioFile();
        audioManager.SeekAudio();
        audioManager.barUpdateSpeed = 23.0f; // Adjusted bar update speed for recording


        // Prepare settings
        MovieRecorderSettings videoRecorderSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        RecorderControllerSettings controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();

        videoRecorderSettings.Enabled = true;
        videoRecorderSettings.CapFrameRate = false;
        videoRecorderSettings.FrameRate = 30.0f;

        videoRecorderSettings.ImageInputSettings = new GameViewInputSettings
        {
            // 4K Resolution
            // OutputWidth = 3840,
            // OutputHeight = 2160

            // 2K Resolution
            OutputWidth = 2560,
            OutputHeight = 1440

            // 1080p Resolution
            // OutputWidth = 1920,
            // OutputHeight = 1080

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
        controllerSettings.FrameRate = 30.0f;

        // Hide Control panel. Record stopping should be with HotKey
        controlPanel.SetActive(false);

        recorderController = new RecorderController(controllerSettings);
        recorderController.PrepareRecording();
        recorderController.StartRecording();

        // Prepare scene
        audioManager.Reset();


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

}

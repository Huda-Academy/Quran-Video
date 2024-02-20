using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;
    private float _time;
    private bool _isPaused = true;

    // Start is called before the first frame update
    void Start()
    {
        ResetTime();
    }

    // Update is called once per frame
    public void ResetTime()
    {
        timeText.text = "00:00";
        PauseTimer(true);
    }

    public void PauseTimer(bool isPaused)
    {
        _isPaused = isPaused;
    }

    public void SetTime(float time)
    {
        _time = time;

        int hours = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt(time % 3600 / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        if (hours > 0)
        {
            timeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
        else
        {
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    [Obsolete]
    public async Task StartTimer(float time, CancellationToken token)
    {
        _time = time;
        SetTime(_time);
        _isPaused = false;

        while (_time > 0 && !token.IsCancellationRequested)
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(1, token);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (_isPaused)
            {
                continue;
            }

            _time--;
            SetTime(_time);
        }

        ResetTime();
    }
}

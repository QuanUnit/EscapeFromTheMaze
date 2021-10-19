using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class Timer : MonoBehaviour, IPropertyChangeNotifier
{
    public event Action<object> PropertyOnChanged;
    public UnityEvent FinishTimer;
    public string Time => string.Format("{0:D2}:{1:D2}", _seconds / 60, _seconds % 60);
    
    [SerializeField] private int _startTime;
    [SerializeField] private bool _reverse;

    private int _seconds;

    private Coroutine _timerRuntime;

    public void Launch(int startTime, bool reverse = false)
    {
        _startTime = startTime;
        _reverse = reverse;
        Launch();
    }

    public void Launch()
    {
        _seconds = _startTime;
        PropertyOnChanged?.Invoke(Time);
        _timerRuntime = StartCoroutine(TimerTick());
    }

    public void Pause()
    {
        if(_timerRuntime != null) StopCoroutine(_timerRuntime);
    }

    public void Stop()
    {
        if(_timerRuntime != null) StopCoroutine(_timerRuntime);
        _timerRuntime = null;
        _seconds = _startTime;
    }

    private IEnumerator TimerTick()
    {
        while (true)
        {
            if (_reverse) _seconds--;
            else _seconds++;

            if (_seconds <= 0)
            {
                _seconds = 0;
                PropertyOnChanged?.Invoke(Time);
                FinishTimer?.Invoke();
                Stop();
                break;
            }
                
            PropertyOnChanged?.Invoke(Time);
            yield return new WaitForSecondsRealtime(1);
        }
    }

}

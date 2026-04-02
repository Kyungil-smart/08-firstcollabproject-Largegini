using UnityEngine;

public class Timer
{
    
    private float _timeStart;
    private float _timeCurrent;
    private float _timeEnd;

    public bool IsEnabled { get; private set; }
    
    public Timer(float maxtime)
    {
        ResetTimer(maxtime);
    }

    public void UpdateTimer()
    {
        _timeCurrent = Time.time - _timeStart;
        if (_timeCurrent >= _timeEnd)
        {
            EndTimer();
        }
    }

    // 타이머 초기화
    public void ResetTimer(float timeMax)
    {
        _timeStart = Time.time;
        _timeCurrent = 0;
        _timeEnd = timeMax;
        IsEnabled = false;
    }

    // 타이머 종료
    private void EndTimer()
    {
        _timeCurrent = _timeEnd;
        IsEnabled = true;
    }
}

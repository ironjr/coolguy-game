using UnityEngine;

/// <summary>
/// SimpleTimer supports basic behaviour of a timer.
/// Delegate Alarms can also be set, so that the timer calls for the
/// alarms after it expires. The alarms are called only once.
/// </summary>
public class SimpleTimer : MonoBehaviour
{
    /// <summary>
    /// Default timer value.
    /// </summary>
    public float SetTime = 1.0f;

    /// <summary>
    /// Delegate type of alarm methods.
    /// </summary>
    public delegate void AlarmDelegate();

    /// <summary>
    /// Alarm delegates are executed when the timer expires, is
    /// active, and the field IsAlarmEnabled is true.
    /// </summary>
    public AlarmDelegate Alarms;

    /// <summary>
    /// Enable/disable the execution of the alarm delegates Alarms
    /// when the timer expires.
    /// </summary>
    public bool IsAlarmEnabled = true;

    private float _timeLeft = 0.0f;
    private bool _isActive = true;
    private bool _haveAlarmRung = false;

    public bool IsOver
    {
        get
        {
            return _isActive && (_timeLeft <= 0.0f);
        }
    }

    public bool IsActive
    {
        get
        {
            return _isActive;
        }
    }

    void Update()
    {
        if (_isActive)
        {
            if (_timeLeft > 0.0f)
            {
                _timeLeft -= Time.deltaTime;
            }
            else if (IsAlarmEnabled && !_haveAlarmRung && Alarms != null)
            {
                _haveAlarmRung = true;
                Alarms();
            }
        }
	}

    #region Interfaces
    /// <summary>
    /// ResetTimer only resets the timer and does not activate it.
    /// Resets timer to SetTime.
    /// </summary>
    public void ResetTimer()
    {
        _timeLeft = SetTime;
        _haveAlarmRung = false;
    }

    /// <summary>
    /// ResetTimer only resets the timer and does not activate it.
    /// Resets timer to the designated new value in seconds.
    /// </summary>
    /// <param name="newSetTime">Newly set timer value.</param>
    public void ResetTimer(float newSetTime)
    {
        _timeLeft = SetTime = newSetTime;
        _haveAlarmRung = false;
    }

    /// <summary>
    /// Activate/deactivate the timer.
    /// Also resets the timer when activates.
    /// </summary>
    /// <param name="active">True for activate/false for deactivate.</param>
    public void SetActive(bool active)
    {
        _isActive = active;
        if (active)
        {
            ResetTimer();
        }
    }

    /// <summary>
    /// Activate/deactivate the timer.
    /// May resume the timer to the previous state where the timer was deactivated.
    /// </summary>
    /// <param name="active">Set true for activate/false for deactivate.</param>
    /// <param name="resumeTimer">True for resume the timer/false for reset.</param>
    public void SetActive(bool active, bool resumeTimer)
    {
        _isActive = active;
        if (active)
        {
            if (!resumeTimer)
            {
                ResetTimer();
            }
            else
            {
                _haveAlarmRung = false;
            }
        }
    }
    #endregion // Interfaces
}

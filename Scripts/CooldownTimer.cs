using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A cooldown timer calculate the elpase time between the last timer 
/// reset and the desired time to trigger an action when the target is reached
/// </summary>
public class CoolDownTimer
{
    /// <summary>
    /// Continues to run the timer while true
    /// </summary>
    public bool IsRunning { get; set; }

    /// <summary>
    /// Current target in seconds to reach
    /// </summary>
    [SerializeField]
    float m_target = 0f;

    /// <summary>
    /// Tracks the time that has passed since last timer reset
    /// </summary>
    float m_deltaTime = 0f;

    /// <summary>
    /// The time passed since last reset
    /// </summary>
    public float Deltatime { get { return m_deltaTime; } }

    /// <summary>
    /// How many seconds to increase the target by
    /// </summary>
    [SerializeField]
    float m_frequency = 1f;

    /// <summary>
    /// Returns the current progress as a value from 0 to 1
    /// </summary>
    public float Progress { get { return m_deltaTime / m_frequency; } }

    /// <summary>
    /// Sets the timer frequency 
    /// </summary>
    public float Frequency
    {
        get { return m_frequency; }
        set {
            m_frequency = Mathf.Abs(value);
            m_target = m_frequency;
        }
    }

    /// <summary>
    /// Delegate for when cooldown is completed
    /// </summary>
    public delegate void OnTimerCompleted();

    /// <summary>
    /// The aciton to trigger when the cooldown reaches its target
    /// </summary>
    public OnTimerCompleted OnTimerCompletedAction
    {
        protected get;
        set;
    }

    /// <summary>
    /// Delegate for when the timer is increased
    /// </summary>
    public delegate void OnTimerIcremented(float progress);

    /// <summary>
    /// What to notify when the timer increases
    /// </summary>
    public OnTimerIcremented OnTimerIncremented
    {
        protected get;
        set;
    }

    /// <summary>
    /// Constructor
    /// Sets the frequency at which to reset the timer
    /// </summary>
    public CoolDownTimer(float delay)
    {
        Frequency = delay;
    }

    /// <summary>
    /// Triggers the cooldown to increase
    /// While the run timer flag is true it will increase the timer
    /// once per frame until it reaches its target then it resets and trigger
    /// any actions bound to this timer
    /// </summary>
    public IEnumerator TimerRoutine()
    {
        IsRunning = true;

        // Not 100% 
        while (Progress < 1f) {
            yield return new WaitForEndOfFrame();
            IncrementTimer();
        }

        IsRunning = false;
        TimeCompleted();
    }

    /// <summary>
    /// Increases the current deltatime and triggers delegates for timer increased
    /// </summary>
    void IncrementTimer()
    {
        m_deltaTime += Time.deltaTime;
        if (OnTimerIncremented != null) {
            OnTimerIncremented(Progress);
        }
    }

    /// <summary>
    /// Resets the timer and triggers any actions bound to this timer
    /// </summary>
    void TimeCompleted()
    {
        if (OnTimerCompletedAction != null) {
            OnTimerCompletedAction();
        }
    }

    /// <summary>
    /// Resets the target the cooldown timer needs to reach
    /// </summary>
    public void ResetTimer()
    {
        m_deltaTime = 0f;
        m_target = m_frequency;
    }

    /// <summary>
    /// Stops the timer routine
    /// </summary>
    public void StopTimer()
    {
        IsRunning = false;
    }
}

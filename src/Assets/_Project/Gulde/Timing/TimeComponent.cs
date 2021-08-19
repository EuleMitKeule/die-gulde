using System;
using System.Collections;
using Gulde.Input;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gulde.Timing
{
    [ExecuteAlways]
    public class TimeComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        [SuffixLabel("min / s")]
        [MinValue(0)]
        public float TimeSpeed { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue(0)]
        public int MinYear { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue(0)]
        [MaxValue("MorningHour")]
        public int MinHour { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue("MinHour")]
        [MaxValue("EveningHour")]
        public int MorningHour { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue("MorningHour")]
        [MaxValue("MaxHour")]
        public int EveningHour { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue(0)]
        [MaxValue(23)]
        public int MaxHour { get; set; }

        [OdinSerialize]
        [BoxGroup("Info")]
        [MinValue(0)]
        [MaxValue(59)]
        public int Minute { get; set; }

        [OdinSerialize]
        [BoxGroup("Info")]
        [MinValue("MinHour")]
        [MaxValue("MaxHour")]
        public int Hour { get; set; }

        [OdinSerialize]
        [BoxGroup("Info")]
        [MinValue("MinYear")]
        public int Year { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public bool IsRunning => TimeCoroutine != null;

        public event EventHandler Morning;
        public event EventHandler Evening;
        public event EventHandler<TimeEventArgs> YearTicked;
        public event EventHandler<TimeEventArgs> WorkingHourTicked;

        Controls Controls { get; set; }
        Coroutine TimeCoroutine { get; set; }
        public event EventHandler<TimeEventArgs> TimeChanged;

        void Awake()
        {
            Locator.Time = this;
            StartTime();

            Controls = new Controls();

            Controls.DefaultMap.MorningAction.performed -= OnMorningActionPerformed;
            Controls.DefaultMap.EveningAction.performed -= OnEveningActionPerformed;
            Controls.DefaultMap.PauseAction.performed -= OnPauseActionPerformed;
            Controls.DefaultMap.MorningAction.performed += OnMorningActionPerformed;
            Controls.DefaultMap.EveningAction.performed += OnEveningActionPerformed;
            Controls.DefaultMap.PauseAction.performed += OnPauseActionPerformed;
            Controls.DefaultMap.Enable();
        }

        void Start()
        {
            YearTicked?.Invoke(this, new TimeEventArgs(Minute, Hour, Year));
        }

        void OnApplicationQuit()
        {
            ResetTime();
            StopTime();
        }

        void OnMorningActionPerformed(InputAction.CallbackContext ctx)
        {
            Morning?.Invoke(this, EventArgs.Empty);
        }

        void OnEveningActionPerformed(InputAction.CallbackContext ctx)
        {
            Evening?.Invoke(this, EventArgs.Empty);
        }

        void OnPauseActionPerformed(InputAction.CallbackContext ctx)
        {
            ToggleTime();
        }

        public void ResetTime()
        {
            Minute = 0;
            Hour = MinHour;
            Year = MinYear;
        }

        public void ToggleTime()
        {
            if (TimeCoroutine != null) StopTime();
            else StartTime();
        }

        public void StartTime() => TimeCoroutine ??= StartCoroutine(TimeRoutine());

        public void StopTime()
        {
            if (TimeCoroutine != null) StopCoroutine(TimeCoroutine);
            TimeCoroutine = null;
        }

        IEnumerator TimeRoutine()
        {
            while (Hour < MaxHour)
            {
                yield return new WaitForSeconds(1 / TimeSpeed);

                Minute += 1;

                var hourChanged = Minute >= 60;
                Hour += hourChanged ? 1 : 0;

                Minute %= 60;

                if (hourChanged)
                {
                    if (Hour >= MorningHour && Hour <= EveningHour) WorkingHourTicked?.Invoke(this,
                        new TimeEventArgs(Minute, Hour, Year));
                }

                TimeChanged?.Invoke(this, new TimeEventArgs(Minute, Hour, Year));

                if (Hour == MorningHour && Minute == 0) Morning?.Invoke(this, EventArgs.Empty);
                if (Hour == EveningHour && Minute == 0) Evening?.Invoke(this, EventArgs.Empty);
            }

            Year += 1;
            Hour = MinHour;
            Minute = 0;

            TimeChanged?.Invoke(this, new TimeEventArgs(Minute, Hour, Year));
            YearTicked?.Invoke(this, new TimeEventArgs(Minute, Hour, Year));

            StopTime();
        }
    }
}
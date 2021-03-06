using System;
using System.Collections;
using System.Timers;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using static UnityEngine.Time;

namespace GuldeLib.Timing
{
    public class TimeComponent : SerializedMonoBehaviour
    {

        /// <summary>
        /// Gets or sets the normal time speed.
        /// </summary>
        [OdinSerialize]
        [BoxGroup("Settings")]
        [SuffixLabel("min / s")]
        [MinValue(1)]
        public float NormalTimeSpeed { get; set; }

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

        [ShowInInspector]
        [BoxGroup("Info")]
        [MinValue(0)]
        [MaxValue(59)]
        public int Minute { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        [MinValue("MinHour")]
        [MaxValue("MaxHour")]
        public int Hour { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        [MinValue("MinYear")]
        public int Year { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        [SuffixLabel("min / s")]
        public float TimeSpeed { get; set; } = 5f;

        [ShowInInspector]
        [BoxGroup("Info")]
        public float TimeScale => timeScale * (TimeSpeed / NormalTimeSpeed);

        [ShowInInspector]
        [BoxGroup("Info")]
        public bool IsRunning => TimeCoroutine != null;

        [ShowInInspector]
        [BoxGroup("Info")]
        public bool IsWorkingHour => Hour >= MorningHour && Hour < EveningHour;

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public bool AutoAdvance { get; set; }

        public event EventHandler Morning;
        public event EventHandler Evening;
        public event EventHandler<TimeEventArgs> YearTicked;
        public event EventHandler<TimeEventArgs> WorkingHourTicked;
        public event EventHandler<TimeEventArgs> MinuteTicked;
        public event EventHandler<InitializedEventArgs> Initialized;

        Coroutine TimeCoroutine { get; set; }
        public event EventHandler<TimeEventArgs> TimeChanged;

        void Start()
        {
            Initialized?.Invoke(this, new InitializedEventArgs());
            StartTime();
            YearTicked?.Invoke(this, new TimeEventArgs(Minute, Hour, Year));
        }

        public void SetTime(int minute = -1, int hour = -1, int year = -1)
        {
            if (minute >= 0) Minute = Mathf.Clamp(minute, 0, 59);
            if (hour >= 0) Hour = Mathf.Clamp(hour, MinHour, MaxHour);
            if (year >= 0) Year = Mathf.Clamp(year, MinYear, 2021);
        }

        public void ResetTime()
        {
            this.Log("Time resetting");

            Minute = 0;
            Hour = MinHour;
            Year = MinYear;
        }

        public void ToggleTime()
        {
            if (TimeCoroutine != null) StopTime();
            else StartTime();
        }

        public void StartTime()
        {
            this.Log("Time starting");
            TimeSpeed = NormalTimeSpeed;
            TimeCoroutine ??= StartCoroutine(TimeRoutine());
        }

        public void StopTime()
        {
            this.Log("Time stopping");

            if (TimeCoroutine != null) StopCoroutine(TimeCoroutine);
            TimeCoroutine = null;

            if (AutoAdvance) StartTime();
        }

        IEnumerator TimeRoutine()
        {
            while (Hour < MaxHour)
            {
                if (timeScale == 0f) yield return new WaitForTimeUnpaused();

                var timeStep = 1f / (TimeSpeed * timeScale);
                yield return new WaitForTimeElapsed(timeStep);

                Minute += 1;
                MinuteTicked?.Invoke(this, new TimeEventArgs(Minute, Hour, Year));

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

        public class InitializedEventArgs : EventArgs
        {
        }

        public class TimeEventArgs : EventArgs
        {
            public TimeEventArgs(int minute, int hour, int year)
            {
                Minute = minute;
                Hour = hour;
                Year = year;
            }

            public int Minute { get; }
            public int Hour { get; }
            public int Year { get; }

        }

        public class WaitForEvening : CustomYieldInstruction
        {
            bool IsEvening { get; set; }

            public override bool keepWaiting => !IsEvening;

            public WaitForEvening()
            {
                Locator.Time.Evening += OnEvening;
            }

            void OnEvening(object sender, EventArgs e)
            {
                IsEvening = true;
            }
        }

        public class WaitForMinuteTicked : CustomYieldInstruction
        {
            bool MinuteTicked { get; set; }

            public override bool keepWaiting => !MinuteTicked;

            public WaitForMinuteTicked()
            {
                Locator.Time.MinuteTicked += OnMinuteTicked;
            }

            void OnMinuteTicked(object sender, TimeEventArgs e)
            {
                MinuteTicked = true;
            }
        }

        public class WaitForMorning : CustomYieldInstruction
        {
            bool IsMorning { get; set; }

            public override bool keepWaiting => !IsMorning;

            public WaitForMorning()
            {
                Locator.Time.Morning += OnMorning;
            }

            void OnMorning(object sender, EventArgs e)
            {
                IsMorning = true;
            }
        }

        public class WaitForTimeElapsed : CustomYieldInstruction
        {
            public override bool keepWaiting => !TimerElapsed;
            bool TimerElapsed { get; set; }
            Timer Timer { get; }

            public WaitForTimeElapsed(float seconds)
            {
                Timer = new Timer(seconds * 1000);
                Timer.Elapsed += OnTimerElapsed;
                Timer.Start();
            }

            void OnTimerElapsed(object sender, ElapsedEventArgs e)
            {
                TimerElapsed = true;
                Timer.Dispose();
            }
        }

        public class WaitForTimeUnpaused : CustomYieldInstruction
        {
            public override bool keepWaiting => Time.timeScale == 0f;
        }

        public class WaitForWorkingHourTicked : CustomYieldInstruction
        {
            bool WorkingHourTicked { get; set; }

            public override bool keepWaiting => !WorkingHourTicked;

            public WaitForWorkingHourTicked()
            {
                Locator.Time.WorkingHourTicked += OnWorkingHourTicked;
            }

            void OnWorkingHourTicked(object sender, TimeEventArgs e)
            {
                WorkingHourTicked = true;
            }
        }

        public class WaitForYearTicked : CustomYieldInstruction
        {
            bool HasYearTicked { get; set; }

            public override bool keepWaiting => !HasYearTicked;

            public WaitForYearTicked()
            {
                Locator.Time.YearTicked += OnYearTicked;
            }

            void OnYearTicked(object sender, TimeEventArgs e)
            {
                HasYearTicked = true;
            }
        }
    }
}
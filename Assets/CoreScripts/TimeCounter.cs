using UnityEngine;

namespace Assets.CoreScripts
{
    public class TimeCounter
    {
        private float _timeToCount;
        private float _startedTime;
        private float _pausedTime;
        private float _accumulatedPauseTime;
        private bool _isPaused;

        public TimeCounter(float timeToCount)
        {
            Reset(timeToCount);
        }

        public float RemainingTime
        {
            get
            {
                var timeInFuture = _startedTime + _timeToCount;
                var currentTime = _isPaused ? _pausedTime : Time.time;
                return Mathf.Max(timeInFuture - currentTime + _accumulatedPauseTime, 0);
            }
        }

        public void SetPaused(bool isPaused)
        {
            if (!_isPaused && isPaused)
            {
                _pausedTime = Time.time;
            }
            else if (_isPaused && !isPaused)
            {
                _accumulatedPauseTime += Time.time - _pausedTime;
            }
            _isPaused = isPaused;
        }

        public void Reset(float timeToCount)
        {
            _timeToCount = timeToCount;
            _startedTime = Time.time;
            _pausedTime = 0;
            _accumulatedPauseTime = 0;
            _isPaused = false;
        }
    }
}
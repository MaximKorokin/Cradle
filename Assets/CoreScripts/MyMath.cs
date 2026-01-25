using System;
using UnityEngine;

namespace Assets.CoreScripts
{
    public static class MyMath
    {
        public static T InterpolateAbsolute<T>(
            T previousValue,
            T nextvalue,
            float previousTime,
            float nextTime,
            float currentTime)
        {
            var middleTime = previousTime + nextTime / 2;
            return currentTime > middleTime ? nextvalue : previousValue;
        }

        public static T Interpolate<T>(
            T previousValue,
            T nextvalue,
            float previousTime,
            float nextTime,
            float currentTime,
            Func<T, T, float, T> lerpFunc)
        {
            if (previousTime == nextTime)
                return previousValue;

            float lerpTime = (currentTime - previousTime) / (nextTime - previousTime);
            return lerpFunc(previousValue, nextvalue, lerpTime);
        }

        public static float CalculateVerticalVelocity(float height, float gravity, float gravityScale = 1)
        {
            float velocity = Mathf.Sqrt(-2 * gravity * height * gravityScale); // 2 - kinematic constant
            return velocity;
        }
    }
}
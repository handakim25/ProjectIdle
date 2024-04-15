using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gust.Utility
{
    // reference : https://easings.net/
    public static class Interpolate
    {
        public delegate float InterpolateFunc(float start, float dist, float elapsedTime, float duration);

        public enum EaseType
        {
            Linear,
            EaseInSine,
            EaseOutSine,
        }

        public static InterpolateFunc GetEase(EaseType easeType)
        {
            return easeType switch 
            {
                EaseType.Linear => Linear,
                EaseType.EaseInSine => EaseInSine,
                EaseType.EaseOutSine => EaseOutSine,
                _ => Linear,
            };
        }

        /// <summary>
        /// Ease Func에 따라서 Interpolate를 진행한다.
        /// </summary>
        /// <param name="ease">Ease 함수</param>
        /// <param name="start">시작값</param>
        /// <param name="dist">결과값까지의 거리</param>
        /// <param name="elapsedTime">Ease 진행 시간</param>
        /// <param name="duration">Ease 지속 시간</param>
        /// <returns></returns>
        public static float Ease(InterpolateFunc ease, float start, float dist, float elapsedTime, float duration)
        {
            return ease(start, dist, elapsedTime, duration);
        }

        /// <summary>
        /// Linear Interpolate
        /// </summary>
        /// <param name="start"></param>
        /// <param name="dist"></param>
        /// <param name="elapsedTime"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static float Linear(float start, float dist, float elapsedTime, float duration)
        {
            if(elapsedTime > duration) {elapsedTime = duration;}
            return dist * elapsedTime / duration + start;
        }

        public static float EaseInSine(float start, float dist, float elapsedTime, float duration)
        {
            if(elapsedTime > duration) {elapsedTime = duration;}
            // 1 - cos(x / 2), x : progress
            // distance(1 - cos(progess / 2))
            return start + dist - dist * Mathf.Cos((elapsedTime / duration) * (Mathf.PI / 2));
        }

        public static float EaseOutSine(float start, float dist, float elapsedTime, float duration)
        {
            if(elapsedTime > duration) {elapsedTime = duration;}
            // sin(x / 2), x : progress
            return start + dist * Mathf.Sin((elapsedTime / duration) * (Mathf.PI / 2));
        }
    }
}

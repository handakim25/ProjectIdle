using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gust.Utility;

namespace Gust.Audio
{
    public sealed partial class SoundManager : MonoSingleton<SoundManager>, IManager
    {
        /// <summary>
        /// 현재 재생 중인 Audio Source의 정보
        /// Play를 진행 중일 때마다 여기에 정보 저장
        /// </summary>
        /// <remarks>
        /// 새로운 SoundPlayInfo를 할당하는 이유는 복잡도를 줄이기 위함
        /// </remarks>
        private class SoundPlayInfo
        {
            public FadeType FadeType = FadeType.None;
            private float Duration = 0f;
            private float FadeTimer = 0f;
            private float StartVolume = 0f; // 0~1f
            private float EndVolume = 0f; // 0~1f
            public Interpolate.EaseType EaseType = Interpolate.EaseType.None;
            public AudioSource TargetAudioSource = null;

            public bool IsFade => FadeType != FadeType.None;
            public bool IsPlaying => TargetAudioSource != null && TargetAudioSource.isPlaying;

            public SoundPlayInfo(AudioSource audioSource)
            {
                TargetAudioSource = audioSource;
            }

            /// <summary>
            /// Delta 시간에 따라 Fade를 진행한다. Fade가 종료되었을 경우 AudioSource를 Stop한다.
            /// </summary>
            /// <param name="delta"></param>
            public void DoFade(float delta)
            {
                if(TargetAudioSource == null) {return;}
                if(!IsFade) {return;}

                FadeTimer += delta;
                if(FadeTimer > Duration) {FadeTimer = Duration;}

                float volume = Interpolate.Ease(Interpolate.GetEase(EaseType), StartVolume, EndVolume - StartVolume, FadeTimer, Duration);
                TargetAudioSource.volume = volume;

                if(FadeTimer >= Duration && FadeType == FadeType.FadeOut)
                {
                    TargetAudioSource.Stop();
                }
            }

            /// <summary>
            /// Fade Out: 소리가 점점 작아지는 효과
            /// </summary>
            /// <param name="duration"></param>
            /// <param name="easeType"></param>
            public void FadeOut(float duration, Interpolate.EaseType easeType)
            {
                FadeType = FadeType.FadeOut;
                Duration = duration;
                FadeTimer = 0f;
                StartVolume = TargetAudioSource.volume;
                EndVolume = 0f;
                EaseType = easeType;
            }

            /// <summary>
            /// Fade In: 소리가 점점 커지는 효과
            /// </summary>
            /// <param name="duration"></param>
            /// <param name="endVolume"></param>
            /// <param name="easeType"></param>
            public void FadeIn(float duration, float endVolume, Interpolate.EaseType easeType)
            {
                FadeType = FadeType.FadeIn;
                Duration = duration;
                FadeTimer = 0f;
                StartVolume = 0f;
                EndVolume = endVolume;
                EaseType = easeType;
            }
        }
    }
}

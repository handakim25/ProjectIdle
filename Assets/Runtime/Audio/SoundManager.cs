using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gust.Utility;

namespace Gust.Audio
{
    /// <summary>
    /// Audio Manager. BGM, SE 등을 재생한다.
    /// Audio를 재생하기 위해서는 초기화가 진행이 되어야 한다. Start에서 초기화를 진행하므로 최소한 Scene에서 Start를 지나야 한다.
    /// </summary>
    public sealed partial class SoundManager : MonoSingleton<SoundManager>, IManager
    {
        public enum FadeType
        {
            None,
            FadeIn,
            FadeOut,
        }

        [Header("Audio Settings")]
        [SerializeField] private int _maxSEChannel = 5;
        [SerializeField] private int _maxUIChannel = 5;

        private AudioSource[] _bgmAudioSources;
        private AudioSource[] _seAudioSources;
        private AudioSource[] _uiAudioSources;

        // BGM
        private SoundPlayInfo _curPlayInfo; // 현재 재생 중인 BGM 정보
        private SoundPlayInfo _lastPlayInfo; // 이전에 재생 중이었던 BGM 정보

        public float CurBgmVolume
        {
            get
            {
                if(_curPlayInfo == null || _curPlayInfo.TargetAudioSource.isPlaying == false)
                {
                    return 0f;
                }
                return _curPlayInfo.TargetAudioSource.volume;
            }
        }

        public bool IsPlayingBGM => _curPlayInfo != null && _curPlayInfo.TargetAudioSource.isPlaying;

        // SE
        private float[] _seStartTimes;

        // UI
        private float[] _uiStartTimes;

        private bool _init = false;
        public bool IsInit => _init;

        public float Progress => 1.0f;

        [Header("Test Settings")]
        // 추후에 Resource System 구축
        [SerializeField] private AudioClip _testAudio;
        [SerializeField] private AudioClip _fateInAudio;

        private void Start()
        {
            InitAudio();
        }

        /// <summary>
        /// 현재 재생에 따라, BGM State를 추적한다.
        /// </summary>
        private void Update()
        {
            // @To-Do
            // Do Fade
            // Fade 중이라면 Fade를 진행한다. 일반 재생의 경우는 Fade 중이 아니므로 아무런 동작도 하지 않는다.
            if(_curPlayInfo != null)
            {
                _curPlayInfo.DoFade(Time.deltaTime);
                if(_curPlayInfo.TargetAudioSource.isPlaying == false)
                {
                    _curPlayInfo = null;
                }
            }
            if(_lastPlayInfo != null)
            {
                _lastPlayInfo.DoFade(Time.deltaTime);
                if(_lastPlayInfo.TargetAudioSource.isPlaying == false)
                {
                    _lastPlayInfo = null;
                }
            }
        }

        public void InitAudio()
        {
            if(_init)
            {
                return;
            }
            _init = true;

            _bgmAudioSources = new AudioSource[2];
            CreateAudioSources(transform, "BGM", _bgmAudioSources, 2);
            foreach(var bgmAudioSource in _bgmAudioSources)
            {
                bgmAudioSource.outputAudioMixerGroup = null;
            }

            _seAudioSources = new AudioSource[_maxSEChannel];
            CreateAudioSources(transform, "SE", _seAudioSources, _maxSEChannel);
            foreach(var seAudioSource in _seAudioSources)
            {
                seAudioSource.outputAudioMixerGroup = null;
            }
            _seStartTimes = new float[_maxSEChannel];

            _uiAudioSources = new AudioSource[_maxUIChannel];
            CreateAudioSources(transform, "UI", _uiAudioSources, _maxUIChannel);
            foreach(var uiAudioSource in _uiAudioSources)
            {
                uiAudioSource.ignoreListenerPause = true;
                uiAudioSource.outputAudioMixerGroup = null;
            }
            _uiStartTimes = new float[_maxUIChannel];
        }

        private void CreateAudioSources(Transform parent, string name, AudioSource[] audioSources, int count)
        {
            for(int i = 0; i < count; ++i)
            {
                var go = new GameObject($"{name}_{i}", typeof(AudioSource));
                go.transform.SetParent(parent);
                audioSources[i] = go.GetComponent<AudioSource>();
            }
        }

        #region BGM
        
        /// <summary>
        /// 현재 재생 중인 BGM을 중지하고 새로운 BGM을 재생한다.
        /// 만약 같은 음악일 경우는 아무런 동작도 하지 않는다.
        /// Fade 중일 경우는 Fade를 중지하고 새로운 BGM을 재생한다.
        /// </summary>
        /// <param name="key">Asset Path</param>
        /// <param name="volume"></param>
        /// <param name="loop"></param>
        public void PlayBGM(string key, float volume = 1.0f, bool loop = true)
        {
            // @To-Do : Check is same BGM

            // Fade를 정지하든, 새로운 BGM을 재생하든 B를 정지하고 A를 재생하면 된다.
            _bgmAudioSources[1].Stop();
            PlaySound(_bgmAudioSources[0], _testAudio, volume, loop);
            _curPlayInfo = new SoundPlayInfo(_bgmAudioSources[0]);
        }

        /// <summary>
        /// 현재 재생 중인 BGM을 중지한다.
        /// 만약 BGM이 재생 중이 아니라면 아무런 동작도 하지 않는다.
        /// Fade 중이라면 Fade 또한 중지한다.
        /// </summary>
        public void StopBGM()
        {
            // 굳이 확인할 것 없이 둘 다 정지시키면 된다.
            _bgmAudioSources[0].Stop();
            _bgmAudioSources[1].Stop();
            _curPlayInfo = null;
            _lastPlayInfo = null;
        }

        /// <summary>
        /// 현재 재생 중인 BGM을 Fade Out한다.
        /// </summary>
        /// <param name="duration"></param>
        /// <remarks>
        /// - Fade 없이 Play 중일 경우 : Fade를 진행한다.
        /// - FadeIn 진행 중일 경우 : Fade In을 중지하고 Fade Out을 진행한다. 이 경우 현재 Volume을 기준으로 Fade Out을 진행한다.
        /// - FadeTo 진행 중일 경우 : Fade Out은 그대로 진행(lastPlayInfo), Fade In(curPlayInfo)을 Fade Out으로 전환
        /// </remarks>
        public void FadeOut(float duration, Interpolate.EaseType easeType = Interpolate.EaseType.Linear)
        {
            _curPlayInfo?.FadeOut(duration, easeType);
        }

        /// <summary>
        /// 새로운 BGM을 Fade In한다. 기존 BGM은 멈춘다. 기존 BGM을 FadeOut으로 진행하는 함수는 FadeTo이다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="duration"></param>
        /// <param name="volume"></param>
        public void FadeIn(string key, float duration, float volume = 1.0f, Interpolate.EaseType easeType = Interpolate.EaseType.Linear)
        {
            // PlayBGM이므로 어디에 재생 중인지 확인할 필요가 없다.
            StopBGM();
            PlaySound(_bgmAudioSources[0], _testAudio, 0f, true);
            _curPlayInfo = new SoundPlayInfo(_bgmAudioSources[0]);
            _curPlayInfo.FadeIn(duration, volume, Interpolate.EaseType.Linear);
        }

        /// <summary>
        /// 현재 재생 중인 BGM을 Fade Out하고, 새로운 BGM을 Fade In한다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="duration"></param>
        /// <param name="volume"></param>
        /// <remarks>
        /// - BGM이 없을 경우 Fade In 진행
        /// - Fade 중일 경우는 Fade Out 중지, Fade In을 Fade Out, 새로운 BGM을 Fade In으로 진행
        /// - BGM이 재생 중일 경우는 재생 중 BGM을 Fade Out, 새로운 BGM을 Fade In으로 진행
        /// </remarks>
        public void FadeTo(string key, float duration, float volume = 1.0f, Interpolate.EaseType easeType = Interpolate.EaseType.Linear)
        {
            // 아무 BGM도 재생 되지 않을 경우는 Fade In을 진행한다.
            if(IsPlayingBGM == false)
            {
                FadeIn(key, duration, volume);
                return;
            }

            _lastPlayInfo?.TargetAudioSource.Stop();

            // 현재 최소한 하나의 BGM은 재생 중이므로 _curPlayInfo는 null이 아니다.
            _lastPlayInfo = _curPlayInfo;
            AudioSource otherAudioSource = _lastPlayInfo.TargetAudioSource == _bgmAudioSources[0] ? _bgmAudioSources[1] : _bgmAudioSources[0];

            PlaySound(otherAudioSource, _fateInAudio, volume);
            _curPlayInfo = new SoundPlayInfo(otherAudioSource);

            _lastPlayInfo.FadeOut(duration, easeType);
            _curPlayInfo.FadeIn(duration, volume, easeType);
        }
        
        #endregion BGM

        #region SE
        
        /// <summary>
        /// Effect Sound를 재생한다. Sound 채널 여분이 없을 경우는 가장 오래된 SE를 중지하고 재생한다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="volume"></param>
        public void PlayEffectSound(string key, float volume)
        {
            PlaySoundInArr(_seAudioSources, _seStartTimes, volume);
        }

        #endregion SE

        #region UI

        /// <summary>
        /// UI Sound를 재생한다. Sound 채널 여분이 없을 경우는 가장 오래된 UI Sound를 중지하고 재생한다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="volume"></param>
        public void PlayUISound(string key, float volume)
        {
            PlaySoundInArr(_uiAudioSources, _uiStartTimes, volume);
        }
        
        #endregion UI

        /// <summary>
        /// Audio Source를 실질적으로 적용하기 위한 함수
        /// </summary>
        /// <param name="targetAudioSource"></param>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        /// <param name="loop"></param>
        private void PlaySound(AudioSource targetAudioSource, AudioClip clip, float volume = 1.0f, bool loop = false)
        {
            targetAudioSource.Stop();
            targetAudioSource.clip = clip;
            targetAudioSource.volume = volume;
            targetAudioSource.loop = loop;
            targetAudioSource.Play();
        }

        private void PlaySoundInArr(AudioSource[] audioSources, float[] startTimes, float volume)
        {
            int channel = -1;
            for (int i = 0; i < audioSources.Length; ++i)
            {
                if (audioSources[i].isPlaying == false)
                {
                    channel = i;
                    break;
                }
            }

            // SE 채널이 모두 사용 중일 경우 가장 오래된 SE를 중지하고 재생한다.
            if (channel == -1)
            {
                float longestTime = 0f;
                channel = 0;
                for (int i = 0; i < audioSources.Length; ++i)
                {
                    if (startTimes[i] > longestTime)
                    {
                        longestTime = startTimes[i];
                        channel = i;
                    }
                }
                audioSources[channel].Stop();
            }

            PlaySound(audioSources[channel], _testAudio, volume);
            startTimes[channel] = Time.time;
        }
    }
}

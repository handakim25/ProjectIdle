using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using Gust.Utility;

namespace Gust.Audio
{
    /// <summary>
    /// Audio Manager. BGM, SE 등을 재생한다.
    /// Audio를 재생하기 위해서는 초기화가 진행이 되어야 한다. Start에서 초기화를 진행하므로 최소한 Scene에서 Start를 지나야 한다.
    /// </summary>
    public sealed partial class SoundManager : MonoSingleton<SoundManager>, IManager, IGameSettingLoad
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

        [Header("Audio Mixer Param")]
        [SerializeField] private string _audioMixerPath = "Assets/Audio/MainAudioMixer.mixer";
        [SerializeField] private string _masterGroupName = "Master";
        [SerializeField] private string _bgmGroupName = "BGM";
        [SerializeField] private string _seGroupName = "Effect";
        [SerializeField] private string _uiGroupName = "UI";
        [SerializeField] private string _masterVolumeParam = "MasterVolume";
        [SerializeField] private string _bgmVolumeParam = "BgmVolume";
        [SerializeField] private string _seVolumeParam = "EffectVolume";
        [SerializeField] private string _uiVolumeParam = "UiVolume";

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

            // Create Audio Channels
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

            // Load Audio Mixer
            ResourceManager.Instance.LoadAsset<AudioMixer>(_audioMixerPath, InitAudioMixer);
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


        #region Mixer
        
        public enum VolumeType
        {
            Master,
            BGM,
            SFX,
            UI,
        }

        private AudioMixer _mixer;

        private void InitAudioMixer(AudioMixer mixer)
        {
            _mixer = mixer;
            if(_mixer == null)
            {
                Debug.LogError($"Cannot load AudioMixer: {_audioMixerPath}");
                return;
            }

            foreach(AudioSource audioSource in _bgmAudioSources)
            {
                audioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(_bgmGroupName)[0];
            }
            foreach(AudioSource audioSource in _seAudioSources)
            {
                audioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(_seGroupName)[0];
            }
            foreach(AudioSource audioSource in _uiAudioSources)
            {
                audioSource.outputAudioMixerGroup = _mixer.FindMatchingGroups(_uiGroupName)[0];
            }
        }

        /// <summary>
        /// Volume 설정
        /// </summary>
        /// <param name="type">설정할 Volume의 Type</param>
        /// <param name="ratio">[0,1]</param>
        public void SetVolume(VolumeType type, float ratio)
        {
            if(_mixer == null)
            {
                return;
            }

            // 참조
            // https://johnleonardfrench.com/the-right-way-to-make-a-volume-slider-in-unity-using-logarithmic-conversion/
            // https://forum.unity.com/threads/audiomixer-setfloat-doesnt-work-on-awake.323880/
            // https://ko.wikipedia.org/wiki/%EB%8D%B0%EC%8B%9C%EB%B2%A8

            // Audio Mixer에서 0db는 최대 출력을 의마한다.
            // Power는 V^2/R이므로 최종 식은 20 * log10(V1/V2)이다.

            // ratio는 선형적으로 움직이지만 volume은 로그 단위로 움직인다.
            ratio = Mathf.Clamp(ratio, 0.0001f, 1f); // Log10(ratio) : -4~0, 최소가 -80이므로 -4 밑으로 내려가는 값은 필요 없다.
            float volume = Mathf.Log10(ratio) * 20;

            // Note
            // ref : https://docs.unity3d.com/ScriptReference/Audio.AudioMixer.SetFloat.html
            // Awake, OnEnable, RuntimeInitializeLoadType.AfterSceneLoad에서 SetFloat 호출하지 말 것
            _mixer.SetFloat(GetVolumeParam(type), volume);
        }

        /// <summary>
        /// Volume을 반환
        /// </summary>
        /// <param name="type"></param>
        /// <returns>[0,1], Mixer가 없을 경우 0f를 반환</returns>
        public float GetVolume(VolumeType type)
        {
            if(_mixer == null)
            {
                return 0f;
            }

            _mixer.GetFloat(GetVolumeParam(type), out float volume);
            return Mathf.Pow(10, volume / 20);
        }

        private string GetVolumeParam(VolumeType type)
        {
            return type switch
            {
                VolumeType.Master => _masterVolumeParam,
                VolumeType.BGM => _bgmVolumeParam,
                VolumeType.SFX => _seVolumeParam,
                VolumeType.UI => _uiVolumeParam,
                _ => _masterGroupName,
            };
        }

        #endregion Mixer

        // Implement IGameSettingLoad
        public void LoadSetting(GameSetting setting)
        {
            SetVolume(VolumeType.Master, setting.MasterVolume);
            SetVolume(VolumeType.BGM, setting.BGMVolume);
            SetVolume(VolumeType.SFX, setting.SFXVolume);
            SetVolume(VolumeType.UI, setting.UIVolume);
        }
    }
}

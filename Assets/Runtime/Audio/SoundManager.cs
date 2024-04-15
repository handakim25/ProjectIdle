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
    public sealed class SoundManager : MonoSingleton<SoundManager>, IManager
    {
        [Header("Audio Settings")]
        [SerializeField] private int _maxSEChannel = 5;

        private AudioSource[] _bgmAudioSources;
        private AudioSource[] _seAudioSources;

        // Fade var
        private BGMState _bgmState = BGMState.None;

        private bool _init = false;
        public bool IsInit => _init;

        public float Progress => 1.0f;

        [Header("Test Settings")]
        // 추후에 Resource System 구축
        [SerializeField] private AudioClip _testAudio;
        [SerializeField] private AudioClip _fateInAudio;

        private enum BGMState
        {
            None, // BGM이 재생 중이 아님
            PlayA, // BGM_A가 재생 중
            PlayB, // BGM_B가 재생 중
            AtoB, // BGM_A에서 BGM_B로 Fade 중
            BtoA, // BGM_B에서 BGM_A로 Fade 중
        }

        private void Start()
        {
            InitAudio();
        }

        /// <summary>
        /// 현재 재생에 따라, BGM State를 추적한다.
        /// </summary>
        private void Update()
        {
            // @Note
            // 코루틴을 사용해도 되지만 현재는 단순하게 구현한다.
            // Check BGM State : Fade 상태는 처음 설정에서 진행된다. 이하의 상태 체크는 Fade의 진행 결과를 추적하는 과정이다.
            // A가 재생 중이고, B가 재생 중이 아니라면 A가 재생 중이라고 판단
            if(_bgmAudioSources[0].isPlaying && _bgmAudioSources[1].isPlaying == false)
            {
                _bgmState = BGMState.PlayA;
            }
            // B가 재생 중이고, A가 재생 중이 아니라면 B가 재생 중이라고 판단
            else if(_bgmAudioSources[0].isPlaying == false && _bgmAudioSources[1].isPlaying)
            {
                _bgmState = BGMState.PlayB;
            }
            // A, B 둘다 재생 중이지 않으면 None
            else if(_bgmAudioSources[0].isPlaying == false && _bgmAudioSources[1].isPlaying == false)
            {
                _bgmState = BGMState.None;
            }
            else
            {
                _bgmState = BGMState.None;
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
            _seAudioSources = new AudioSource[_maxSEChannel];
            CreateAudioSources(transform, "SE", _seAudioSources, _maxSEChannel);
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
        }

        /// <summary>
        /// 현재 재생 중인 BGM을 Fade Out한다. 재생 중이지 않을 경우는 아무런 동작도 하지 않는다.
        /// </summary>
        /// <param name="duration"></param>
        public void FadeOut(float duration, Interpolate.EaseType easeType = Interpolate.EaseType.Linear)
        {
            
        }

        public void FadeIn(string key, float duration, float volume = 1.0f)
        {

        }

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
    }
}

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

        private AudioSource[] _bgmAudioSource;
        private AudioSource[] _seAudioSources;

        // Fade var
        /// <summary>
        /// 현재 재생 중이거나 아니면 Fade 중인 BGM의 Audio Source
        /// </summary>
        private AudioSource _mainAudioSource;

        private bool _init = false;
        public bool IsInit => _init;

        public float Progress => 1.0f;

        [Header("Test Settings")]
        // 추후에 Resource System 구축
        private AudioClip _testAudio;
        private AudioClip _fateInAudio;

        private void Start()
        {
            InitAudio();
        }

        public void InitAudio()
        {
            if(_init)
            {
                return;
            }
            _init = true;

            _bgmAudioSource = new AudioSource[2];
            CreateAudioSources(transform, "BGM", _bgmAudioSource, 2);
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
        /// 만약 같은 
        /// </summary>
        /// <param name="key">Asset Path</param>
        /// <param name="volume"></param>
        /// <param name="loop"></param>
        public void PlayBGM(string key, float volume = 1.0f, bool loop = true)
        {
            
        }

        /// <summary>
        /// 현재 재생 중인 BGM을 중지한다.
        /// </summary>
        public void StopBGM()
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
            // targetAudioSource.clip = clip;
            // targetAudioSource.volume = volume;
            // targetAudioSource.loop = loop;
            // targetAudioSource.Play();
        }
    }
}

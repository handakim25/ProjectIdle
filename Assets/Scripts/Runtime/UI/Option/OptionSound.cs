using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Gust.Audio;

namespace Gust.UI.Option
{
    /// <summary>
    /// Option Menu에서 Sound 설정을 담당하는 Class
    /// 활성화 될 때 Sound 설정값을 읽어오고 비활성화 될 때 저장한다.
    /// Sound 변경은 Slider의 Value가 변경될 때 이루어진다.
    /// </summary>
    public class OptionSound : MonoBehaviour
    {
        [Header("UI Components")]
        [Tooltip("Slider : [0,100], Volume : [0.0f, 1.0f]")]
        [SerializeField] private OptionSliderButtonWithText _masterVolumeSlider;
        [Tooltip("Slider : [0,100], Volume : [0.0f, 1.0f]")]
        [SerializeField] private OptionSliderButtonWithText _bgmVolumeSlider;
        [Tooltip("Slider : [0,100], Volume : [0.0f, 1.0f]")]
        [SerializeField] private OptionSliderButtonWithText _sfxVolumeSlider;
        [Tooltip("Slider : [0,100], Volume : [0.0f, 1.0f]")]
        [SerializeField] private OptionSliderButtonWithText _uiVolumeSlider;

        [SerializeField] private CustomToggleButton _muteButton;
        [SerializeField] private CustomToggleButton _playBackgroundButton;

        // @To-Do
        // Option 설정 중에 설정이 Sound를 재생해서 volume에 대해서 테스트할 수 있도록 한다.

        [Header("UI Check Sounds")]
        [SerializeField] private string _masterVolumeCheckSound;
        [SerializeField] private string _bgmVolumeCheckSound;
        [SerializeField] private string _sfxVolumeCheckSound;
        [SerializeField] private string _uiVolumeCheckSound;

        // Setting은 하나의 Instance를 통해서 관리되고 있다. 이를 수정하고 Game Manager에 Save를 요청한다.
        private GameSetting _setting;

        private void Start()
        {
            _setting = GameManager.Instance.GameSetting;
            if(_setting == null)
            {
                Debug.LogError("Could not find GameSetting in GameManager");
                return;
            }

            if(_masterVolumeSlider != null)
            {
                _masterVolumeSlider.onValueChanged.AddListener(OnMaterVolumeChanged);
                _masterVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.MasterVolume);
            }
            
            if(_bgmVolumeSlider != null)
            {
                _bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
                _bgmVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.BGMVolume);
            }

            if(_sfxVolumeSlider != null)
            {
                _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
                _sfxVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.SFXVolume);
            }

            if(_uiVolumeSlider != null)
            {
                _uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);
                _uiVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.UIVolume);
            }

            if(_muteButton != null)
            {
                _muteButton.PointerClickEvent.AddListener(() => SoundManager.Instance.Mute = !_muteButton.IsSelected);
            }
            if(_playBackgroundButton != null)
            {
                // @To-Do
                // Play Background를 통해서 Background Sound를 재생하거나 중지한다.
                // _playBackgroundButton.PointerClickEvent.AddListener(() => SoundManager.Instance.PlayBackground = !_playBackgroundButton.IsSelected);
            }
        }

        private void OnEnable()
        {
            // Awake 상황에서 Setting을 얻어오는 것이 아니라 Start에서 Setting을 얻기 위함이다.
            // 만약, Start에서 Setting을 획득하지 않았을 경우 이 또한, 잘못된 동작이기에 작동하지 않는다.
            if(_setting == null)
            {
                return;
            }

            if(_masterVolumeSlider != null)
            {
                _masterVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.MasterVolume);
            }
            if(_bgmVolumeSlider != null)
            {
                _bgmVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.BGMVolume);
            }
            if(_sfxVolumeSlider != null)
            {
                _sfxVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.SFXVolume);
            }
            if(_uiVolumeSlider != null)
            {
                _uiVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.UIVolume);
            }
        }

        private void OnDisable()
        {
            GameManager.Instance.SaveGameSetting();
        }

        // Slider는 [0,100], Volume은 [0.0f, 1.0f]로 변환하여 저장

        private void OnMaterVolumeChanged(float value)
        {
            _setting.MasterVolume = SoundUtility.PercentageToRatio(value);
            SoundManager.Instance.MasterVolume = _setting.MasterVolume;
        }

        private void OnBGMVolumeChanged(float value)
        {
            _setting.BGMVolume = SoundUtility.PercentageToRatio(value);
            SoundManager.Instance.BGMVolume = _setting.BGMVolume;
        }

        private void OnSFXVolumeChanged(float value)
        {
            _setting.SFXVolume = SoundUtility.PercentageToRatio(value);
            SoundManager.Instance.SFXVolume = _setting.SFXVolume;
        }

        private void OnUIVolumeChanged(float value)
        {
            _setting.UIVolume = SoundUtility.PercentageToRatio(value);
            SoundManager.Instance.UIVolume = _setting.UIVolume;
        }
    }
}
